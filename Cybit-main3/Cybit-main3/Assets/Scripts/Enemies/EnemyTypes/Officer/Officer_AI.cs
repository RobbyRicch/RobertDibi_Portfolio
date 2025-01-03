using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Officer_AI : EnemyHenchman
{
    [Header("Enemy Data")]
    [SerializeField] protected GameObject _attackPrefab;
    [SerializeField] protected OfficerAttack _attackScript;

    [SerializeField] protected bool _isAttacking = false;

    #region Officer SO Data
    private float _attackDelay = 0.45f;
    private float _attackTime = 0.15f;
    #endregion

    protected override void Start()
    {
        base.Start();
        _attackScript.Damage = _so.AttackDamage;
    }
    protected override void Update()
    {
        base.Update();

        if (_isChasing)
            ChasePlayer();
        else if (_isPatroling)
            Patrol();

        if (_playerTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTarget.transform.position);

            if (distanceToPlayer <= _attackRange)
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

        if (_canAttack)
            AttackPlayer();

        transform.localRotation = Quaternion.identity;
    }

    private IEnumerator MeleeAttack()
    {
        yield return new WaitForSeconds(_attackDelay);
        _attackPrefab.SetActive(true);
        yield return new WaitForSeconds(_attackTime);
        _attackPrefab.SetActive(false);
    }

    protected override void HenchmanSOInitialization()
    {
        base.HenchmanSOInitialization();

        AI_SO_Officer officer = _so as AI_SO_Officer;
        _attackDelay = officer.AttackDelay;
        _attackTime = officer.AttackTime;
    }
    protected override void AttackPlayer()
    {
        if (_isOnCooldown || _playerTarget == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTarget.transform.position);
        if (distanceToPlayer > _attackRange) // if distance is bigger than range, stop attack then return. else do what comes after
        {
            _isAttacking = false;
            return;
        }

        _isAttacking = true;
        _enemyAnimator.SetTrigger("Attack");
        StartCoroutine(MeleeAttack());
        StartCoroutine(AttackCooldown());
    }

    public override void Die(EnemyBase currentEnemy)
    {
        base.Die(this);

        if (Task != null && Task is TaskKill killTask)
            killTask.AddToCurrentKills();
    }
}
