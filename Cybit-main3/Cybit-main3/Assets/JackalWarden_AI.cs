using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JackalWarden_AI : EnemyBase
{
    [SerializeField] private GameObject _endUI;
    public GameObject EndUI { get => _endUI; set => _endUI = value; }

    [SerializeField] private BossHUD_Manager _bossHUD;
    public BossHUD_Manager BossHUD { get => _bossHUD; set => _bossHUD = value; }

    [SerializeField] private float _bossDemoVideoDelay = 1.5f;
    [SerializeField] private float _cybitVideoTime = 4.0f;

    [Header("References")]
    [SerializeField] private Transform _armPivot;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _lassoPivot;
    [SerializeField] private Transform _lassoEndPoint;
    [SerializeField] private GameObject _vulnerableStateGO;
    [SerializeField] private GameObject _punchColider;
    [SerializeField] private Animator _cannonArmAnimator;
    [SerializeField] private Animator _lassoArmAnimator;
    [SerializeField] private Vector3 _currentTarget;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _tauntAC, _damageAC;
    public GameObject _lassoGO;
    [SerializeField] private DynamicAudioManager _dynamicAudioManager;
    [SerializeField] private GameObject _worldSpaceTextMesh;

    [SerializeField] private List<GameObject> _activeHounds = new List<GameObject>();
    public List<GameObject> ActiveHounds => _activeHounds;

    [Header("Attack Bank")]
    [SerializeField] private GameObject _hounds;
    [SerializeField] private GameObject _lassoTriggerPrefab;
    [SerializeField] private int _houndAmount;
    [SerializeField] public int _houndsDead;
    [SerializeField] private int _shotsFired;
    [SerializeField] private int _maxShots;
    [SerializeField] private float _lassoSpeed = 2.0f;
    private EnemyProjectileBullet _rangedAttack;

    [Header("Barrier")]
    [SerializeField] private GameObject _barrierGO;
    [SerializeField] private Barrier_System _barrierSystem;
    [SerializeField] private bool _shouldActivateBarrier;
    [SerializeField] private BoxCollider2D _damageCollider;
    private float _ogTimeBarrier;
    private float _maxRepair = 100f;

    [Header("Drops")]
    [SerializeField] private GameObject _hpDrop;
    [SerializeField] private float _focusAmountDrop;

    [Header("VFX Bank")]
    [SerializeField] private GameObject _dmgVFX;
    [SerializeField] private GameObject _deathVFX;
    [SerializeField] private GameObject _barrierRestoreVFX;
    [SerializeField] private GameObject _punchVFX;

    [Header("States")]
    [SerializeField] private bool _isFiring;
    [SerializeField] private bool _barrierActive;
    [SerializeField] private bool _releaseHounds;
    [SerializeField] private bool _houndsHaveReleased;
    [SerializeField] private bool _canFire;
    [SerializeField] private bool _onCooldown;
    [SerializeField] private bool _isAttacking;
    [SerializeField] private bool _isVulnerable;
    [SerializeField] private bool _isReconnecting;
    [SerializeField] public bool _isLassoing;
    [SerializeField] private bool _canLasso;
    [SerializeField] private bool _canMelee;
    [SerializeField] private bool _isPunching;
    [SerializeField] private bool _meleeOnCooldown;
    [SerializeField] public bool _playerHasBeenSnared;
    [SerializeField] private bool _repairCoroutineRunning;
    [SerializeField] private Vector3 _liveLassoTransform;
    [SerializeField] private bool _lassoReleased;


    [Header("Pattern")]
    [SerializeField] private bool _nextAttackBarrage;
    [SerializeField] private bool _nextAttackHounds;
    [SerializeField] private bool _currentAttackBarrage;
    [SerializeField] private bool _currentAttackHounds;

    [Header("Attack Pattern Timing")]
    [SerializeField] private float _timeforWindup;
    [SerializeField] private float _timeforShooting;
    [SerializeField] private float _timeforBarrierReturn;
    [SerializeField] private float _timeforVulnerability;
    [SerializeField] private float _timeLassoIsSpinning;
    [SerializeField] private float _timeToActivatePunchTrigger;
    [SerializeField] private float _timeResetMelee;

    [Header("HP Canisters Timing")]
    [SerializeField] private Transform[] _canisterTransform;
    [SerializeField] private int _transformIndexCanisters;
    [SerializeField] private GameObject _HPCanisterGO;
    [SerializeField] private float _HPCanisterTimer;
    [SerializeField] private bool _shouldDropCanister;

    [Header("Boss Dialogue")]
    [SerializeField] private EnemyDialogueManager _bossDialogueManager;
    [SerializeField] private List<GameObject> _lassoDialogue;
    [SerializeField] private List<GameObject> _deathDialogue;
    [SerializeField] private List<GameObject> _killPlayerDialogue;
    private Vector3 _directionToPlayer;

    [Header("Ending")]
    [SerializeField] private Player_Controller _playerController;
    [SerializeField] private CinemachineVirtualCamera _vCam;

    [SerializeField] private bool _isBossEnabled = false;
    public bool IsBossEnabled { get => _isBossEnabled; set => _isBossEnabled = value; }

    protected override void Start()
    {
        InitializeBoss();
        _agent.updateRotation = false;
        transform.rotation = Quaternion.identity;
        StartCoroutine(LassoCooldown(Random.Range(5, 20)));
        _barrierGO.SetActive(true);
        StartCoroutine(CanisterCooldown(_HPCanisterTimer));
        _ogTimeBarrier = _barrierSystem._timeToRaise;
    }
    protected override void Update()
    {
        if (!_isBossEnabled)
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        if (_lassoReleased && _playerHasBeenSnared)
        {
            _liveLassoTransform = _directionToPlayer;
            _lineRenderer.SetPosition(1, _liveLassoTransform);

        }

        if (_shouldDropCanister && _transformIndexCanisters >= 0 && _transformIndexCanisters < _canisterTransform.Length)
        {
            DropHPCanisters();
        }

        if (!_isVulnerable)
        {
            if (_isChasing)
            {
                ChasePlayer();
            }

            if (_playerTarget != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, _playerTarget.transform.position);
                _currentTarget = _playerTarget.transform.position;
                if (distanceToPlayer <= _agent.stoppingDistance)
                {
                    _canFire = true;
                    _isChasing = false;
                }
                else
                {
                    _isChasing = true;

                }
            }
            AimAtTarget();
            transform.rotation = Quaternion.identity;

            if (_nextAttackBarrage)
            {
                StartCoroutine(PrepareBarrage());
            }

            if (_canFire && _currentAttackBarrage && !_isLassoing && !_isPunching)
            {
                Barrage();
            }

            if (!_isFiring)
            {
                _cannonArmAnimator.Play("JackalCannon_Idle_Anim");
            }

            if (_nextAttackHounds)
            {
                StartCoroutine(PrepareHounds());
            }

            if (_currentAttackHounds)
            {
                Hounds();
            }

            _barrierActive = _barrierSystem._isActive;


            if (_canLasso && !_isLassoing)
            {
                PrepareLasso();
            }

            if (_isLassoing)
            {
                _directionToPlayer = _playerTarget.transform.position - transform.position;
                
            }

/*            if (_isLassoing && !_playerHasBeenSnared)
            {
                StartCoroutine(DeactivateLassoGO(2.5f));
            }*/

            if (_playerHasBeenSnared)
            {
                _agent.stoppingDistance = 3;
                float distanceToPlayer = Vector3.Distance(transform.position, _playerTarget.transform.position);
                _currentTarget = _playerTarget.transform.position;
                if (distanceToPlayer <= _agent.stoppingDistance)
                {
                    _lineRenderer.SetPosition(1, _directionToPlayer);
                    _canMelee = true;
                }
                else
                {
                    _canMelee = false;

                }
            }
            else
            {
                _agent.stoppingDistance = _so.AttackRange;
            }

            if (_canMelee && !_isPunching && !_meleeOnCooldown)
            {
                StartCoroutine(MechaPunch(_timeToActivatePunchTrigger));
            }
        }

        if (_isVulnerable)
        {
            _enemyAnimator.SetBool("isMoving", false);
            _agent.enabled = false;
            _vulnerableStateGO.SetActive(true);
            _isReconnecting = true;
            if (_barrierActive)
            {

                StartCoroutine(_barrierSystem.LowerBarrier(_barrierSystem._timeToLower));
            }
            transform.rotation = Quaternion.identity;

        }
        else
        {
            _vulnerableStateGO.SetActive(false);
        }

        if (_barrierSystem._isActive)
        {
            _damageCollider.enabled = false;
        }
        else
        {
            _damageCollider.enabled = true;
        }

        int initialHoundCount = ActiveHounds.Count;
        /*        _activeHounds.RemoveAll(hound => hound == null);
        */
        _houndsDead += initialHoundCount - ActiveHounds.Count;

        // Boss cannot fire if there are active hounds
        _canFire = ActiveHounds.Count == 0;

        if (_houndsDead == _houndAmount)
        {
            _isVulnerable = true;
            ActiveHounds.Clear();
            _houndsDead = 0;
            PatternCheck();

        }

        if (_isReconnecting)
        {
            StartCoroutine(ReconnectSystems(_timeforVulnerability));
        }


    }

    private void InitializeBoss()
    {
        AI_SO_Boss bossData = _so as AI_SO_Boss;
        _bulletDamage = bossData.AttackDamage;
        _damageColor = bossData.DamageColor;
        _focusToGivePlayer = bossData.FocusToGivePlayer;
        _knockBackTime = bossData.KnockBackTime;
        _timeBetweenAttacks = bossData.TimeBetweenAttacks;
        _hurtAC = bossData.HurtAC;
        _spawnAC = bossData.SpawnAC;
        _idleAC = bossData.IdleAC;
        _deathAC = bossData.DeathAC;
        _endDeathAC = bossData.EndDeathAC;
        _itemDropAC = bossData.ItemDropAC;
        _attackRange = bossData.AttackRange;
        _maxHealth = bossData.MaxHealth;
        _moveSpeed = bossData.MoveSpeed;

        _rangedAttack = bossData.Projectile as EnemyProjectileBullet;
        _currentHealth = _maxHealth;
        _agent.stoppingDistance = _so.AttackRange;
        /*        _bossHUD.InitializeHUD("Jackal", null);
        */
        _bossHUD.BossHPSlider.maxValue = _maxHealth;
        _bossHUD.BossHPSlider.value = _maxHealth;
    }
    private void PrepareLasso()
    {
        _lassoArmAnimator.SetTrigger("StartLasso");
        _isLassoing = true;
        StartCoroutine(LassoDialogue());
        StartCoroutine(LassoAttack(_timeLassoIsSpinning));
    }

    private IEnumerator LassoAttack(float timeLasso)
    {
        yield return new WaitForSeconds(timeLasso);
        _lassoArmAnimator.SetTrigger("ThrowLasso");
        yield return new WaitForSeconds(0.5f);
        _lassoGO.SetActive(true);
        _liveLassoTransform = _directionToPlayer;
        _lineRenderer.SetPosition(1, _liveLassoTransform);
        _lineRenderer.SetPosition(0, _lassoPivot.localPosition);
        Vector3 startPosition = _lassoPivot.localPosition;
        _lassoReleased = true;
        float elapsedTime = 0f;
        while (elapsedTime < 1f / _lassoSpeed)
        {
            _lineRenderer.SetPosition(1, Vector3.Lerp(startPosition, _liveLassoTransform, elapsedTime * _lassoSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Debug.Log(_lineRenderer.GetPosition(1));
        _lassoArmAnimator.ResetTrigger("StartLasso");
        _lassoArmAnimator.ResetTrigger("ThrowLasso");
        _canLasso = false;

        // Retrieve the position from the LineRenderer
        Vector3 lassoEndPosition = _lineRenderer.GetPosition(1);

        // Instantiate the lasso trigger at the lasso end position as a child of _lassoGO
        GameObject lassoTriggerInstance = Instantiate(_lassoTriggerPrefab, lassoEndPosition, Quaternion.identity);
        lassoTriggerInstance.transform.SetParent(_lassoGO.transform, false);
        StartCoroutine(LassoFailSafe(4));
        StartCoroutine(LassoCooldown(Random.Range(5, 20)));

    }
    private IEnumerator MechaPunch(float time)
    {
        _canMelee = false;
        _isPunching = true;
        _cannonArmAnimator.SetTrigger("Punch");
        yield return new WaitForSeconds(time);
        _punchVFX.SetActive(true);
        _punchColider.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        _punchColider.SetActive(false);
        _punchVFX.SetActive(false);
        _cannonArmAnimator.ResetTrigger("Punch");
        _isPunching = false;
        _meleeOnCooldown = true;
        StartCoroutine(ResetMelee(_timeResetMelee));
    }
    private IEnumerator ResetMelee(float time)
    {
        yield return new WaitForSeconds(time);
        _meleeOnCooldown = false;
    }
    private IEnumerator LassoCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        _canLasso = true;
    }
    private IEnumerator ReconnectSystems(float timeToConnect)
    {
        yield return new WaitForSeconds(timeToConnect);
        _isReconnecting = false;
        _isVulnerable = false;
        _agent.enabled = true;

        // Call RaiseBarrier from Barrier_System class
        StartCoroutine(_barrierSystem.RaiseBarrier(_barrierSystem._timeToRaise));
    }

    private IEnumerator PrepareHounds()
    {
        _nextAttackHounds = false;
        _currentAttackHounds = true;
        yield return new WaitForSeconds(_timeforWindup);
        _releaseHounds = true;
    }
    private IEnumerator PrepareBarrage()
    {
        _shotsFired = 0;
        _nextAttackBarrage = false;
        _currentAttackBarrage = true;
        yield return new WaitForSeconds(_timeforWindup);
        _isFiring = true;
        _canFire = true;
    }
    private IEnumerator ReleaseHoundsOneByOne()
    {
        for (int i = 0; i < _houndAmount; i++)
        {
            // Calculate a random position around the boss within a certain radius
            Vector2 spawnPosition = (Vector2)transform.position + Random.insideUnitCircle * 2f; // Adjust the radius as needed

            // Instantiate the hound at the calculated position
            GameObject hound = Instantiate(_hounds, spawnPosition, Quaternion.identity);
            ActiveHounds.Add(hound);
            // Wait for a short delay before releasing the next hound
            yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
        }
    }
    private IEnumerator ShootCooldown()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(_so.TimeBetweenAttacks);
        _onCooldown = false;
    }
    private IEnumerator EndDemo()
    {
        EventManager.InvokeEndGame();
        _playerController = SaveManager.Instance.Player;
        _vCam = _playerController.CurrentVirtualCamera;
        _playerController.LIS.IsLinkCompromised = false;
        yield return new WaitForSeconds(1);

        EventManager.InvokeCutscene(true);
        _vCam.Follow = gameObject.transform;
        Animator _vcamAnimator = _vCam.GetComponent<Animator>();
        _vcamAnimator.SetBool("Zoom", true);
        _playerController.ShowUIAndCrosshair(false);

        yield return new WaitForSeconds(_bossDemoVideoDelay);
        _vcamAnimator.SetBool("Zoom", false);

        _endUI.SetActive(true);
        yield return new WaitForSeconds(_cybitVideoTime);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        EventManager.InvokeCutscene(false);
        SaveManager.Instance.SceneCustomManager.GoToTraining();
        

    }

    public IEnumerator DamageVFX()
    {
        _dmgVFX.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        _dmgVFX.SetActive(false);
    }

    private void Hounds()
    {
        if (_releaseHounds)
        {
            StartCoroutine(ReleaseHoundsOneByOne());
            _releaseHounds = false; // Reset the flag
        }
    }
    private void Barrage()
    {
        if (!_onCooldown)
        {
            Vector2 direction = _firePoint.right;

            EnemyProjectileBullet firedBullet = Instantiate(_rangedAttack, _firePoint.position, Quaternion.identity);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            firedBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            AI_SO_Boss so = _so as AI_SO_Boss;
            firedBullet.SetStats(so.AttackDamage, so.ProjectileSpeed, direction);
            _onCooldown = true;
            StartCoroutine(ShootCooldown());
            _shotsFired++;
            _cannonArmAnimator.SetBool("isAttacking", true);

            if (_shotsFired == _maxShots)
            {
                _cannonArmAnimator.SetBool("isAttacking", false);
                _canFire = false;
                _isFiring = false;
                PatternCheck();
            }
        }
    }
    private void PatternCheck()
    {
        if (_currentAttackBarrage)
        {
            _nextAttackHounds = true;
            _currentAttackBarrage = false;
        }

        if (_currentAttackHounds)
        {
            _nextAttackBarrage = true;
            _currentAttackHounds = false;
        }
    }

    protected override void AimAtTarget()
    {
        Vector3 directionToTarget = _playerTarget.transform.position - transform.position;

        // Flip the enemy if needed
        if (directionToTarget.x < 0 && _isFacingRight)
        {
            FlipEnemy();
            _enemyAnimator.SetBool("IsFacingRight", false);
            Debug.Log("flipped left");
            FlipDialogueText();

        }
        else if (directionToTarget.x > 0 && !_isFacingRight)
        {
            FlipEnemy();
            _enemyAnimator.SetBool("IsFacingRight", true);
            Debug.Log("flipped right");
            FlipDialogueText();

        }


        // Calculate the angle to the target
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;


        // Adjust the arm pivot rotation to aim at the player
        if (_playerTarget != null)
        {
            if (_isFacingRight)
            {
                _armPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                if (_armPivot.localScale.y < 0)
                {
                    _armPivot.localScale = new Vector3(1, 1, 1);
                    _lassoGO.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                }
            }
            else
            {
                _armPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, -angle));
                if (_armPivot.localScale.y > 0)
                {
                    _armPivot.localScale = new Vector3(-1, -1, -1);
                    _lassoGO.transform.localScale = new Vector3(-0.3f, 0.3f, 0.3f);

                }
            }
        }
    }

    private void FlipDialogueText()
    {
        if (_worldSpaceTextMesh != null)
        {
            // Flip the TextMeshPro object based on the enemy's facing direction
            float scaleX = _isFacingRight ? 0.017f : -0.017f;
            _worldSpaceTextMesh.transform.localScale = new Vector3(scaleX, 0.017f, 0.017f);
        }
    }

    protected override void ChasePlayer()
    {
        if (_playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTarget.transform.position);

        if (distanceToPlayer > _agent.stoppingDistance)
        {
            _isChasing = true;
            _agent.isStopped = false; // Ensure the agent is not stopped
            _agent.SetDestination(_playerTarget.transform.position);
            _enemyAnimator.SetBool("isMoving", true);
        }
        else
        {
            _isChasing = false;
            _agent.isStopped = true; // Stop the agent completely
            _enemyAnimator.SetBool("isMoving", false);
            _agent.ResetPath(); // Clear the current path
        }

        // Prevent unwanted rotation
        transform.rotation = Quaternion.identity;
    }
    protected override void FlipEnemy()
    {
        // Toggle the facing flags
        _isFacingRight = !_isFacingRight;

        // Flip the character's scale
        Vector3 characterScale = transform.localScale;
        characterScale.x *= -1;

        transform.localScale = characterScale;

        if (_spriteRenderers[1] != null && _spriteRenderers[2] != null && _isFacingRight)
        {
            _spriteRenderers[1].sortingOrder = 2;
            _spriteRenderers[2].sortingOrder = 0;
        }
        else if (_spriteRenderers[1] != null && _spriteRenderers[2] != null && !_isFacingRight)
        {
            _spriteRenderers[1].sortingOrder = 0;
            _spriteRenderers[1].sortingOrder = 2;
        }
    }

    public override void TakeDamage(Vector2 normalizedAttackDirection, float damage, float knockBackPower)
    {
        base.TakeDamage(normalizedAttackDirection, damage, knockBackPower);
        StartCoroutine(DamageVFX());
        _bossHUD.BossHPSlider.value = _currentHealth;
    }
    public override void Die(EnemyBase enemy)
    {
        //StopAllCoroutines();
        _isBossEnabled = false;
        _isVulnerable = false;
        _deathDialogue[0].SetActive(true);
        _enemyAnimator.SetBool("isMoving", false);
        _agent.enabled = false;
        transform.rotation = Quaternion.identity;
        _deathVFX.SetActive(true);
        _barrierGO.SetActive(false);
        _barrierRestoreVFX.SetActive(false);
        _bossHUD.gameObject.SetActive(false);
        StartCoroutine(EndDemo());
    }

    private void DropHPCanisters()
    {
        if (_transformIndexCanisters < _canisterTransform.Length)
        {
            Vector3 spawnPosition = _canisterTransform[_transformIndexCanisters].position;
            Quaternion spawnRotation = _canisterTransform[_transformIndexCanisters].rotation;

            Debug.Log($"Spawning canister at position: {spawnPosition}");
            Instantiate(_HPCanisterGO, spawnPosition, spawnRotation);
            _transformIndexCanisters++;
            _shouldDropCanister = false;
            StartCoroutine(CanisterCooldown(_HPCanisterTimer));
        }
        else
        {
            Debug.LogWarning("Canister index out of range.");
        }
    }

    private IEnumerator CanisterCooldown(float time)
    {
        _shouldDropCanister = false;
        yield return new WaitForSeconds(time);
        _shouldDropCanister = true;
    }

    private IEnumerator LassoDialogue()
    {
        int random = Random.Range(0, _lassoDialogue.Count);
        _lassoDialogue[random].SetActive(true);
        yield return new WaitForSeconds(10);
        _lassoDialogue[random].SetActive(false);
    }

    private IEnumerator LassoFailSafe(float time)
    {
        yield return new WaitForSeconds(time);
        _lassoGO.SetActive(false);
        _isLassoing = false;
        _canLasso = false;
    }
}

