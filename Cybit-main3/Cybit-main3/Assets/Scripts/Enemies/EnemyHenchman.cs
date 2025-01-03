using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyHenchman : EnemyBase
{
    #region Components
    [Header("Henchman Components")]
    [SerializeField] private TaskBase _task;
    public TaskBase Task { get => _task; set => _task = value; }
    #endregion

    #region Patroling
    [Header("Patroling")]
    [SerializeField] protected List<Transform> _patrolRoutes;
    [SerializeField] protected Transform _detectionTr;
    [SerializeField] protected float _detectionRadius;
    [SerializeField] protected float _patrolDelay;

    protected Coroutine _patrolCoroutine;
    protected Vector3 _currentTarget;
    protected int _currentPatrolIndex;
    protected bool _patrolForward;
    #endregion

    #region Henchman Flags
    [Header("Henchman Flags")]
    [SerializeField] protected bool _isPatroling = false;
    [SerializeField] protected bool _isOnCooldown = false;
    [SerializeField] protected bool _isBeingBuffed = false;
    public bool BeingBuffed { get => _isBeingBuffed; set => _isBeingBuffed = value; }

    [SerializeField] protected bool _playerCompromized = false;
    public bool PlayerCompromized { get => _playerCompromized; set => _playerCompromized = value; }
    #endregion

    #region Henchman SO Data
    protected AudioClip _patrolingAC, _attackAC, _spotPlayerAC;
    #endregion

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_playerTag))
        {
            DetectPlayer(collision);
        }
    }

    #region Methods
    protected virtual IEnumerator PatrolRoutine()
    {
        while (_isPatroling)
        {
            _agent.stoppingDistance = 0;

            if (_patrolRoutes.Count == 0)
                yield break;

            // Move to the next patrol point
            _agent.SetDestination(_patrolRoutes[_currentPatrolIndex].position);
            _currentTarget = _agent.destination;

            yield return new WaitUntil(() => _agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance);
            yield return new WaitForSeconds(_patrolDelay);

            // Update patrol index and direction
            if (_patrolForward)
            {
                _currentPatrolIndex++;
                if (_currentPatrolIndex >= _patrolRoutes.Count)
                {
                    _currentPatrolIndex = _patrolRoutes.Count - 1;
                    _patrolForward = false;
                }
            }
            else
            {
                _currentPatrolIndex--;
                if (_currentPatrolIndex < 0)
                {
                    _currentPatrolIndex = 0;
                    _patrolForward = true;
                }
            }

            yield return null; // temp
        }

        _patrolCoroutine = null;
    }
    
    protected override void SOInitialization()
    {
        HenchmanSOInitialization();
    }
    protected override void Initialize()
    {
        base.Initialize();

        if (_isPatroling)
            _detectionTr.localScale = Vector3.one * _detectionRadius;
    }

    protected virtual void AimAtTarget(Transform armPivot, Vector3 currentTarget)
    {
        Vector3 directionToTarget = currentTarget - transform.position;

        // Calculate the angle to the target
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        // Adjust the arm pivot rotation to aim at the player
        if (directionToTarget.x < 0 && _isFacingRight)
        {
            FlipEnemy();
            if (armPivot.localScale.y > 0)
                armPivot.localScale = -Vector3.one;

            _enemyAnimator.SetBool("IsFacingRight", false);
            armPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

            //Debug.Log("flipped left");
        }
        else if (directionToTarget.x > 0 && !_isFacingRight)
        {
            FlipEnemy();
            if (armPivot.localScale.y < 0)
                armPivot.localScale = Vector3.one;

            _enemyAnimator.SetBool("IsFacingRight", true);
            armPivot.localRotation = Quaternion.Euler(new Vector3(0, 0, -angle));

            //Debug.Log("flipped right");
        }
    }
    protected virtual IEnumerator AttackCooldown()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(_timeBetweenAttacks);
        _isOnCooldown = false;
    }
    protected virtual void HenchmanSOInitialization()
    {
        base.SOInitialization();
        //_patrolingAC = _enemyData.PatrolAudioClip;
        //_attackAC = _enemyData.AttackAudioClip;
        //_spotPlayerAC = _enemyData.SpotPlayerAudioClip;
    }
    protected virtual void DetectPlayer(Collider2D collision)
    {
        if (!_isPatroling || !_detectionTr.gameObject.activeInHierarchy)
            return;

        _playerTarget = collision.transform;
        _currentTarget = _playerTarget.position;
        _detectionTr.gameObject.SetActive(false);
        _isPatroling = false;
        _isChasing = true;
        _agent.stoppingDistance = _attackRange;
    }
    protected virtual void Patrol()
    {
        if (_patrolRoutes.Count == 0)
            return;

        _patrolCoroutine ??= StartCoroutine(PatrolRoutine());
    }
    protected virtual void AttackPlayer() { }

    public void BuffHenchman(int amount)
    {
        _maxHealth += amount;
        _currentHealth += amount;
        BeingBuffed = true;
    }
    public void LoseBuff(int amount)
    {
        _maxHealth -= amount;
        _currentHealth -= amount;
        BeingBuffed = false;
    }
    #endregion
}