using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Enemy Fields
    [Header("Enemy Fields")]
    public NavMeshAgent Agent;
    public Transform Player;
    [SerializeField] private Renderer EnemyMesh;
    private Material EnemyMaterial;
    [SerializeField] private HealthHandler EnemyHealthHandler;
    public Animator AnimatorRef;
    public bool DestroyOnDeath;
    public float MaxSpeed = 1;
    private bool _inControl = true;
    private bool _isDying = false;
    public HealthHandler EnemyHealthHandlerGet { get => EnemyHealthHandler; }

    // Patroling
    [Header("Patroling")]
    public bool CanPatrol = false;
    public float PatrolRange;
    public float MaxWaitTime;
    private Vector3 _startingPoint;
    private Vector3 WalkPoint;
    private bool _walkPointSet;
    private bool _reachedPoint;

    // Attacking
    [Header("Attacking")]
    public float AttackCoolDownTime;
    public bool AlreadyAttacked;
    public bool HitSuccess;
    public bool AttackWithinSight;

    // States
    [Header("States")]
    public bool AlwaysTrackPlayer;
    public bool CanChase = true;
    public float SightRange;
    public float AttackRange;
    private bool PlayerInSightRange;
    private bool _playerInAttackRange;
    private float _playerDistanceNoHeight;
    private float _playerHeightDistance;
    public bool PlayerInAttackRange { get => _playerInAttackRange; }

    // Special Behaviours
    [Header("Special Behaviours")]
    public bool CanRetreat;
    public float RetreatRange = 5;
    public float RetreatSpeed = 1;
    public bool CanLunge;
    public float LungeForce = 0.2f;
    [SerializeField] private float _lungeLength = 0.25f;
    private float _lungeLengthTimer;
    private bool _prepLunging;
    private bool _isLunging;
    [SerializeField] private bool _isBuffer;
    public bool IsLunging { get => _isLunging; }
    public bool IsBuffer { get => _isBuffer; }

    // Field Of View
    [Header("FOV")]
    [Range(0, 360)]
    public int ViewAngle = 90;
    public bool UseCanSeePlayer;
    private bool _canSeePlayer;

    // Enemy Avoidance Steering
    [Header("Avoidance Steering")]
    public bool CanAvoid;
    public float AvoidanceRadius = 2.5f;
    public float AvoidanceSpeed = 1;
    public LayerMask EnemyLayer;
    public Collider EnemyCollider;
    private List<Collider> _collidingEnemies = new List<Collider>();

    // Collectables
    [Header("Collectable")]
    public GameObject CollectableQPrefab;
    public GameObject CollectableRPrefab;
    private int collecNum = 0;

    // KnockBack
    [Header("Knockback")]
    [SerializeField] bool _isKnockbackable = true;
    [SerializeField] private float _knockbackForceModifier = 1;
    private bool _knockbacked;
    private float _knockbackForce;
    private float _knockbackTimer;
    private Vector3 _knockbackDirection;
    public bool IsKnockbackable { get => _isKnockbackable; }
    public float KnockbackForceModifier { get => _knockbackForceModifier; }

    // Physics
    [Header("Gravity")]
    [SerializeField] private bool _gravityEnabled = true;
    [SerializeField] private Vector3 _groundCheckOffset;
    [SerializeField] private float _groundRadius = 0.5f;
    [SerializeField] private float _gravityModifier = 10f;
    [SerializeField] private LayerMask _groundLayer;
    private AgentLinkMover _agentLinkMover;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isJumpingLink;

    // Sounds
    [Header("Sounds")]
    [SerializeField] private AudioSource DeathAudio;

    // Dodging
    [Header("Dodging")]
    [SerializeField] private bool _sightedByPlayer;
    [SerializeField] private bool _shotAtByPlayer;
    private Camera _playerCamera;
    private PlayerInfo _playerInfo;
    [SerializeField] private bool _canDodge = true;
    [SerializeField] private bool _teleportDodge;
    private bool _isDodging;
    private bool _triedDodging;
    private bool _succededDodging;
    [SerializeField] private float _dodgeCooldown = 1;
    [SerializeField] private float _dodgeLength = 0.1f;
    private float _dodgeCooldownTimer;
    private float _dodgeLengthTimer;
    [Range(0, 10)]
    [SerializeField] private float _dodgeForceMin = 0.3f;
    [Range(0, 10)]
    [SerializeField] private float _dodgeForceMax = 0.5f;
    private float _dodgeForceTemp;
    [Range(0, 100)]
    [SerializeField] private int _dodgeChance = 50;
    private int _dodgeChanceTemp;
    private int _dodgeDir;
    [SerializeField] private GameObject _dodgeEffect;
    public bool SuccededDodging { get => _succededDodging; }

    [Header("Debug")]
    public bool ShowRanges;
    public bool ShowFOV;
    public bool ShowAvoidance;
    public bool ShowGrounded;


    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        MaxSpeed = Agent.speed;
        if (!Player) Player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerCamera = Camera.main;
        _playerInfo = Player.GetComponent<PlayerInfo>();
        EnemyHealthHandler.OnDeathOccured += EnemyHealthHandler_OnDeathOccured;
        _startingPoint = transform.position;
        EnemyMaterial = EnemyMesh.material;
        if (TryGetComponent(out AgentLinkMover linkMover)) _agentLinkMover = linkMover;
    }

    private void Update()
    {
        RunEnemy();
    }

    public void RunEnemy()
    {
        if (Time.timeScale != 0)
        {
            CanSeePlayer();
            InSightOfPlayer();
            ShotAt();

            HandleControlState();

            Gravity();
            HandleKnockback();

            HandleHitSuccess();

            AnimationHandler();
        }
    }

    private void HandleControlState()
    {
        if (_inControl && ((_isGrounded && _gravityEnabled && !_isJumpingLink) || !_gravityEnabled))
        {
            Agent.enabled = true;
            Agent.isStopped = false;

            CalculateDistances();

            StateHandler();

            DodgeHandler();
        }
        else if (!_inControl && ((_isGrounded && _gravityEnabled) || !_gravityEnabled))
        {
            Agent.isStopped = true;
        }
        else
        {
            Agent.enabled = false;
        }
    }

    private void HandleHitSuccess()
    {
        if (HitSuccess)
        {
            HitSuccess = false;
            Invoke("RegisterIndicator", 0);
        }
    }

    private void CalculateDistances()
    {
        Vector3 tempPlayerPos = Player.transform.position;
        Vector3 tempEnemyPos = transform.position;

        _playerHeightDistance = Vector3.Distance(new Vector3(0, tempPlayerPos.y, 0), new Vector3(0, tempEnemyPos.y, 0));

        tempPlayerPos.y = 0;
        tempEnemyPos.y = 0;

        _playerDistanceNoHeight = Vector3.Distance(tempPlayerPos, tempEnemyPos);
    }

    private bool CheckRange(float rangeToCheck)
    {
        if (_playerDistanceNoHeight <= rangeToCheck)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StateHandler()
    {
        if (AlwaysTrackPlayer)
        {
            PlayerInSightRange = true;
        }
        else
        {
            PlayerInSightRange = CheckRange(SightRange);
        }
        _playerInAttackRange = CheckRange(AttackRange) && _canSeePlayer;

        Patroling(!PlayerInSightRange && !_playerInAttackRange);
        ChasePlayer(PlayerInSightRange && !_playerInAttackRange);
        AttackPlayer(_playerInAttackRange && PlayerInSightRange, false);
        Retreat();
        SteerAvoidance();
    }

    virtual public void Patroling(bool patroling)
    {
        if (CanPatrol)
        {
            if (patroling)
            {
                Agent.speed = MaxSpeed / 2;
                if (!_walkPointSet)
                {
                    SearchWalkPoint();
                    _walkPointSet = true;
                }

                if (_walkPointSet && !_reachedPoint)
                    Agent.SetDestination(WalkPoint);

                Vector3 distanceToWalkPoint = transform.position - WalkPoint;

                if (distanceToWalkPoint.magnitude < 1f)
                {
                    _walkPointSet = false;
                    if (!_reachedPoint)
                    {
                        _reachedPoint = true;
                        StartCoroutine(WaitInPlace());
                    }
                }
            }
            else
            {
                Agent.speed = MaxSpeed;
                _startingPoint = transform.position;
                SearchWalkPoint();
            }
        }
    }

    IEnumerator WaitInPlace()
    {
        float randTime = Random.Range(0, MaxWaitTime);
        yield return new WaitForSeconds(randTime);
        _reachedPoint = false;
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-PatrolRange, PatrolRange);
        float randomX = Random.Range(-PatrolRange, PatrolRange);

        WalkPoint = new Vector3(_startingPoint.x + randomX, _startingPoint.y, _startingPoint.z + randomZ);
    }


    private void Gravity()
    {
        _isGrounded = Physics.CheckSphere(transform.position + _groundCheckOffset, _groundRadius, _groundLayer);

        if (_gravityEnabled && !_isJumpingLink)
        {

            if (_isGrounded && _velocity.y <= 0)
            {
                _velocity.y = 0;
            }
            else
            {
                _velocity.y -= _gravityModifier * Time.deltaTime;
                transform.position += _velocity * Time.deltaTime;
            }
        }

        JumpingLink();
    }

    private void JumpingLink()
    {
        if (_agentLinkMover != null) _isJumpingLink = _agentLinkMover.IsJumping;
    }


    public void Knockback(float duration, float force, Vector3 dir)
    {
        if (!_knockbacked)
        {
            _knockbacked = true;
            _knockbackForce = force;
            _knockbackTimer = duration;
            _knockbackDirection = dir;
        }
    }

    private void HandleKnockback()
    {
        if (_knockbacked)
        {
            _inControl = false;
            if (_knockbackTimer > 0)
            {
                _knockbackTimer -= Time.deltaTime;
                Vector3 knockParams = _knockbackDirection * _knockbackForce * Time.deltaTime;
                Agent.Move(knockParams);
            }
            else
            {
                _knockbacked = false;
            }
        }
        else
        {
            if (!_isDying) _inControl = true;
            _knockbackForce = 0;
            _knockbackTimer = 0;
            _knockbackDirection = Vector3.zero;
        }
    }

    private void ToggleHPBarState()
    {
        if (PlayerInSightRange)
        {
            EnemyHealthHandler.ToggleHealthBar(true);
        }
        else
        {
            EnemyHealthHandler.ToggleHealthBar(false);
        }
    }

    private void InSightOfPlayer()
    {
        Vector3 screenPoint = _playerCamera.WorldToViewportPoint(transform.position);
        //Debug.Log(screenPoint);
        if (screenPoint.z > 0 && screenPoint.x > 0.40f && screenPoint.x < 0.60f && screenPoint.y > 0.30f && screenPoint.y < 0.80f)
        {
            _sightedByPlayer = true;
        }
        else
        {
            _sightedByPlayer = false;
        }
    }

    private void ShotAt()
    {
        if (_sightedByPlayer)
        {
            _shotAtByPlayer = _playerInfo.IsAttacking;
        }
        else
        {
            _shotAtByPlayer = false;
        }
    }

    private void CanSeePlayer()
    {
        if (UseCanSeePlayer)
        {
            _canSeePlayer = false;

            Vector3 directionToPlayer = (Player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, directionToPlayer) <= ViewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer, out hit, SightRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        _canSeePlayer = true;
                    }
                }
            }
        }
        else
        {
            _canSeePlayer = true;
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDeg)
    {
        angleInDeg += eulerY;
        return new Vector3(Mathf.Sin(angleInDeg * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDeg * Mathf.Deg2Rad));
    }

    virtual public void ChasePlayer(bool chasing)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (CanChase)
        {
            if (chasing)
            {
                Agent.SetDestination(Player.position);
                if (!AlreadyAttacked && AttackWithinSight && _canSeePlayer)
                {
                    AttackSequence();
                }
            }
        }
    }

    private void Retreat()
    {
        if (CanRetreat)
        {
            if (_playerDistanceNoHeight < RetreatRange)
            {
                Agent.Move(transform.forward * -1 * (0.01f * RetreatSpeed));
            }
        }
    }

    private void SteerAvoidance()
    {
        if (CanAvoid && !_playerInAttackRange && !CheckRange(AvoidanceRadius))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, AvoidanceRadius, EnemyLayer);

            _collidingEnemies.Clear();
            foreach (var item in colliders)
            {
                if (item != EnemyCollider) _collidingEnemies.Add(item);
            }

            if (_collidingEnemies.Count > 0)
            {
                if (!_collidingEnemies[0].transform.parent.GetComponent<EnemyAI>().PlayerInAttackRange)
                {
                    Vector3 directionFromNeighbor = (transform.position - _collidingEnemies[0].transform.position).normalized;
                    directionFromNeighbor.y = 0;
                    Agent.Move(directionFromNeighbor * (0.01f * AvoidanceSpeed));
                }
            }
        }
    }


    virtual public void AttackPlayer(bool attacking, bool changeAttack)
    {
        if (!gameObject.activeSelf)
        {
            return;
        }
        if (attacking)
        {
            //Make sure enemy doesn't move
            Agent.SetDestination(transform.position);

            FaceTarget();

            if (!changeAttack)
            {
                if (!AlreadyAttacked && _canSeePlayer)
                {
                    AttackSequence();
                }
            }
        }
    }

    public delegate void Callback();

    public IEnumerator ResetAttack(float attackCoolDown, Callback boolToReset)
    {
        //Debug.Log("Reseting");
        yield return new WaitForSeconds(attackCoolDown);
        boolToReset();
        //Debug.Log("Done");
    }

    virtual public void AttackPatternController()
    {

    }

    private void AttackSequence()
    {
        AttackPatternController();
        AlreadyAttacked = true;
        StartCoroutine(ResetAttack(AttackCoolDownTime, delegate () { AlreadyAttacked = false; }));
        ////Debug.Log("Attacked");
    }

    IEnumerator DelayedAttack(float delay)
    {
        if (!AlreadyAttacked)
        {
            yield return new WaitForSeconds(delay);
            AttackSequence();
        }
    }

    private void FaceTarget()
    {
        Vector3 lookPos = (Player.position - transform.position).normalized;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * Agent.angularSpeed);
    }

    private void DodgeHandler()
    {
        if (_canDodge)
        {
            if (_shotAtByPlayer && _dodgeCooldownTimer >= _dodgeCooldown && !_isDodging)
            {
                _isDodging = true;
            }
            else
            {
                _dodgeCooldownTimer += Time.deltaTime;
            }

            if (_isDodging)
            {
                _dodgeLengthTimer += Time.deltaTime;
                if (_dodgeLengthTimer <= _dodgeLength)
                {
                    if (_teleportDodge)
                    {
                        if (!_triedDodging)
                        {
                            _triedDodging = true;
                            _dodgeChanceTemp = Random.Range(0, 100);
                            _dodgeDir = Random.Range(0, 2);
                            _dodgeForceTemp = Random.Range(_dodgeForceMin, _dodgeForceMax);
                            if (_dodgeChanceTemp <= _dodgeChance)
                            {
                                _succededDodging = true;
                                CreateDodgeVFX();
                                DodgeController(_dodgeDir);
                                StartCoroutine(DodgeVFXDelay());
                            }
                        }
                    }
                    else
                    {
                        if (!_triedDodging)
                        {
                            _triedDodging = true;
                            _dodgeChanceTemp = Random.Range(0, 100);
                            _dodgeDir = Random.Range(0, 2);
                            _dodgeForceTemp = Random.Range(_dodgeForceMin, _dodgeForceMax);
                        }
                        if (_dodgeChanceTemp <= _dodgeChance)
                        {
                            _succededDodging = true;
                            DodgeController(_dodgeDir);
                            if (_dodgeEffect != null) _dodgeEffect.SetActive(true);
                        }
                    }
                }
                else
                {
                    _dodgeLengthTimer = 0;
                    _dodgeCooldownTimer = 0;
                    _isDodging = false;
                    _triedDodging = false;
                    _succededDodging = false;
                    if (_dodgeEffect != null) _dodgeEffect.SetActive(false);
                }
            }

            LungeHandler();
        }
    }

    virtual public void DodgeController(int dir)
    {
        switch (dir)
        {
            case 0:
                Agent.Move(transform.right * _dodgeForceTemp);
                break;
            case 1:
                Agent.Move(transform.right * -1 * _dodgeForceTemp);
                break;
            default:
                break;
        }
    }

    IEnumerator DodgeVFXDelay()
    {
        yield return new WaitForEndOfFrame();
        CreateDodgeVFX();
    }

    private void CreateDodgeVFX()
    {
        if (_dodgeEffect != null)
        {
            var vfx = Instantiate(_dodgeEffect, transform.position, Quaternion.identity);
            vfx.SetActive(true);
            Destroy(vfx, 1.5f);
        }
    }

    private void LungeHandler()
    {
        if (CanLunge)
        {
            if (_succededDodging)
            {
                _prepLunging = true;
            }
            else
            {
                if (_prepLunging)
                {
                    _prepLunging = false;
                    _isLunging = true;

                    StartCoroutine(DelayedAttack(_lungeLength / 2));
                }
            }

            if (_isLunging)
            {
                _lungeLengthTimer += Time.deltaTime;
                if (_lungeLengthTimer <= _lungeLength)
                {
                    LungeController();
                }
                else
                {
                    _lungeLengthTimer = 0;
                    _isLunging = false;
                }
            }
        }
    }

    private void LungeController()
    {
        Agent.Move(transform.forward * LungeForce);
    }

    virtual public void AnimationHandler()
    {

    }

    // Registering To Player Indicator
    private void RegisterIndicator()
    {
        //if (!DamageIndicatorSystem.CheckIfObjInSight(transform))
        //{
        DamageIndicatorSystem.CreateIndicator(transform);
        //}
    }

    private void EnemyHealthHandler_OnDeathOccured(object sender, System.EventArgs e)
    {
        EnemyDeath();
    }

    public virtual void EnemyDeath()
    {
        if (!_isDying)
        {
            _isDying = true;
            /*if (name[0] == 'Q')
            {
                GameObject reward = Instantiate(CollectableQPrefab, transform.position, Quaternion.identity);
                GameManager.xpIncrease("Q");
            }
            else
            {
                GameObject reward = Instantiate(CollectableRPrefab, transform.position, Quaternion.identity);
                GameManager.xpIncrease("Q");
            }*/
            if (DestroyOnDeath)
            {
                _inControl = false;
                DeathAudio.Play();
                Destroy(gameObject, 0.7f);
            }
            else
            {

                if (AnimatorRef != null) AnimatorRef.enabled = false;
                _inControl = false;
                EnemyCollider.enabled = false;
                DeathAudio.Play();
                EnemyHealthHandler.ToggleHealthBar(false);
                GameManager.IncreaseKills();

                StartCoroutine(DelayDeath());
                StartCoroutine(DissolveDelay());
                AlreadyAttacked = true;

            }
        }
    }
    IEnumerator DissolveDelay()
    {
        yield return new WaitForSeconds(1f);
        float DissolveValue = -1f;
        while (DissolveValue <= -0.1f)
        {
            DissolveValue = Mathf.Lerp(EnemyMaterial.GetFloat("_DissolveIntensity"), 0f, Time.deltaTime);
            EnemyMaterial.SetFloat("_DissolveIntensity", DissolveValue);

            yield return null;
        }
    }

    IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        EnemyHealthHandler._healthSystem.RefillHealth();
        EnemyMaterial.SetFloat("_DissolveIntensity", -1f);
        EnemyCollider.enabled = true;
        if (AnimatorRef != null) AnimatorRef.enabled = true;
        _inControl = true;
        EnemyHealthHandler.ToggleHealthBar(true);
        _isDying = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (ShowRanges)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, SightRange);
        }

        if (ShowFOV && Player)
        {

            Vector3 leftViewAngle = DirectionFromAngle(transform.eulerAngles.y, -ViewAngle / 2);
            Vector3 rightViewAngle = DirectionFromAngle(transform.eulerAngles.y, ViewAngle / 2);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + leftViewAngle * SightRange);
            Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rightViewAngle * SightRange);
            if (ViewAngle < 180)
            {
                Gizmos.DrawLine(transform.position + Vector3.up + transform.forward * SightRange, transform.position + Vector3.up + leftViewAngle * SightRange);
                Gizmos.DrawLine(transform.position + Vector3.up + transform.forward * SightRange, transform.position + Vector3.up + rightViewAngle * SightRange);
            }
            else
            {
                Vector3 lViewAngle = DirectionFromAngle(transform.eulerAngles.y, -180 / 2);
                Vector3 rViewAngle = DirectionFromAngle(transform.eulerAngles.y, 180 / 2);
                Gizmos.DrawLine(transform.position + Vector3.up + transform.forward * SightRange, transform.position + Vector3.up + lViewAngle * SightRange);
                Gizmos.DrawLine(transform.position + Vector3.up + transform.forward * SightRange, transform.position + Vector3.up + rViewAngle * SightRange);
                Gizmos.DrawLine(transform.position + Vector3.up + lViewAngle * SightRange, transform.position + Vector3.up + leftViewAngle * SightRange);
                Gizmos.DrawLine(transform.position + Vector3.up + rViewAngle * SightRange, transform.position + Vector3.up + rightViewAngle * SightRange);
            }

            if (_canSeePlayer)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }
            Gizmos.DrawRay(transform.position + Vector3.up, (Player.position - transform.position).normalized * SightRange);
        }

        if (ShowAvoidance)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, AvoidanceRadius);
        }

        if (ShowGrounded)
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (_isGrounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(transform.position + _groundCheckOffset, _groundRadius);
        }
    }
}
