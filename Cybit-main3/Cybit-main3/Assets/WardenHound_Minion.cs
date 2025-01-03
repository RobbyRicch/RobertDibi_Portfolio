using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WardenHound_Minion : EnemyBase
{

    [Header("Base Stats")]
    [SerializeField] private float _flightSpeed;
    [SerializeField] private float _attackDMG;

    [Header("Attacks")]
    [SerializeField] private BoxCollider2D _minionCollider;
    [SerializeField] private GameObject _attackCollider;
    [SerializeField] private float _timeToBite;
    [SerializeField] private float _timeInbetweenBites;

    [Header("Components")]
    [SerializeField] private NavMeshAgent _minionAgent;

    [Header("States")]
    [SerializeField] private bool _isAttacking;
    [SerializeField] private bool _isOnCooldown;
    [SerializeField] private bool _canHurtOnTouch;

    private JackalWarden_AI _warden;


    private void InitializeMinion()
    {
        _maxHealth = _so.MaxHealth;
        _currentHealth = _maxHealth;
        _playerTarget = FindAnyObjectByType<Player_Controller>().transform;
        _enemyAnimator.SetBool("IsFacingRight", _isFacingRight);
        _minionAgent.speed = _flightSpeed;
        _damageColor = _so.DamageColor;

        _warden = FindObjectOfType<JackalWarden_AI>();
        if (_warden != null)
        {
            _warden.ActiveHounds.Add(gameObject);
        }

    }

    // Start is called before the first frame update
    protected override void Start()
    {
        InitializeMinion();
        _minionAgent.updateRotation = false;
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (_isChasing)
        {
            ChasePlayer();
        }

        if (_playerTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTarget.transform.position);

            if (distanceToPlayer <= _minionAgent.stoppingDistance)
            {
                _canAttack = true;
                _isChasing = false;
            }
            else
            {
                _isChasing = true;
                _canAttack = false;
            }
        }

        if (_canAttack && !_isAttacking && !_isOnCooldown)
        {
            StartCoroutine(AttackPlayer());
        }

        AimAtTarget();

        transform.rotation = Quaternion.identity;
    }

    private IEnumerator AttackPlayer()
    {
        _isAttacking = true;
        _enemyAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(_timeToBite);
        _attackCollider.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        _attackCollider.SetActive(false);
        _isAttacking = false;
        StartCoroutine(AttackCooldown(_timeInbetweenBites));
    }

    private IEnumerator AttackCooldown(float cd)
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(cd);
        _isOnCooldown = false;
    }

    public override void Die(EnemyBase enemy)
    {
        if (_warden != null)
        {
            if (_warden.ActiveHounds.Contains(gameObject))
                _warden.ActiveHounds.Remove(gameObject);
        }
        Destroy(gameObject, 0.35f);
    }
    private void OnDestroy()
    {
        if (_warden != null)
        {
            _warden._houndsDead++;
        }
    }
}
