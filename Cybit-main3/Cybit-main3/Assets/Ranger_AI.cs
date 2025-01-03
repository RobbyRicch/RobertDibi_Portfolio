using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ranger_AI : EnemyHenchman
{
    [Header("Unique Behaviors")]
    [SerializeField] protected Transform _armPivot;
    [SerializeField] protected Transform _attackPoint;
    [SerializeField] protected LayerMask _hitLayers;

    [Header("Unique Attacks - Ranged Attack")]
    [SerializeField] protected Animator _weaponAnimator;
    [SerializeField] protected float _bulletSpeed;

    [Header("Unique Attacks - Charged Attack")]
    [SerializeField] protected float _chargedAttackSpeed;
    [SerializeField] protected int _chargedShotCounter;
    [SerializeField] protected int _chargedShotMax;
    [SerializeField] protected float _timeForChargedRelease;

    [Header("Unique Behaviors - Attack States")]
    [SerializeField] protected bool _isCharging;
    [SerializeField] protected bool _isCharged;
    [SerializeField] protected bool _canCharge;
    [SerializeField] protected bool _hasFiredCharged;

    protected EnemyProjectileBullet _projectile;
    protected EnemyProjectileBullet _chargedProjectile;
    protected Color _chargeAttackColor = new();
    protected float _chargeAttackFlashDuration = 1.0f;
    protected float _chargeAttackCooldown = 1.0f;

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

            _currentTarget = _playerTarget.transform.position;
            if (distanceToPlayer <= _agent.stoppingDistance)
            {
                _canAttack = true;
                _isChasing = false;
            }
            else
            {
                _isChasing = true;
            }
        }

        if (_canAttack && !_isCharged)
        {
            AttackPlayer();
        }
        else if (_isCharged && !_hasFiredCharged)
        {
            StartChargedRangedAttack();
        }
        else if (_hasFiredCharged)
        {
            _isCharged = false;
            StartCoroutine(ResetChargeShot());
        }

        transform.rotation = Quaternion.identity;
    }
    
    private IEnumerator ChargedAttackFlash()
    {
        _isFlashing = true; // Set the flag to indicate that damage flash is in progress

        // Store original colors
        List<Color> originalColors = new List<Color>();
        foreach (SpriteRenderer renderer in _spriteRenderers)
        {
            originalColors.Add(renderer.color);
            renderer.color = _chargeAttackColor;
        }

        yield return new WaitForSeconds(_chargeAttackFlashDuration);

        float elapsedTime = 0f;
        while (elapsedTime < 0.35f)
        {
            elapsedTime += Time.deltaTime * 5f; // Adjust the speed of color lerp here
            for (int i = 0; i < _spriteRenderers.Count; i++)
            {
                _spriteRenderers[i].color = Color.Lerp(_chargeAttackColor, originalColors[i], elapsedTime);
            }
            yield return null;
        }

        // Ensure we set the original color explicitly
        for (int i = 0; i < _spriteRenderers.Count; i++)
        {
            _spriteRenderers[i].color = originalColors[i];
        }

        _isFlashing = false; // Reset the flag after damage flash is complete
    }
    private IEnumerator DelayedChargedRangedAttack()
    {
        if (_hasFiredCharged)
            yield break;

        _isCharged = false;
        _weaponAnimator.SetTrigger("ChargedAttack");
        yield return new WaitForSeconds(_timeForChargedRelease);
        Vector2 direction = (_playerTarget.position - _attackPoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        EnemyProjectileBullet firedBullet = Instantiate(_chargedProjectile, _attackPoint.position, Quaternion.AngleAxis(angle, Vector3.forward));
        firedBullet.SetStats(_bulletDamage, _bulletSpeed, direction);
        firedBullet.transform.localScale = new(firedBullet.transform.localScale.x * 2, firedBullet.transform.localScale.y * 2, firedBullet.transform.localScale.z);
        _chargedShotCounter = 0;
        _canCharge = true;
        _hasFiredCharged = true;
        _canAttack = true;
    }
    private IEnumerator ResetChargeShot()
    {
        _hasFiredCharged = true;
        yield return new WaitForSeconds(_chargeAttackCooldown);
        _hasFiredCharged = false;
        _isCharging = false;
    }
    private void StartChargedRangedAttack()
    {
        if (!_isCharging)
        {
            _isCharging = true;
            StartCoroutine(ChargedAttackFlash());
            StartCoroutine(DelayedChargedRangedAttack());
        }
    }
    
    protected override void HenchmanSOInitialization()
    {
        base.HenchmanSOInitialization();
        _agent.stoppingDistance = _attackRange;

        AI_SO_Ranger ranger = _so as AI_SO_Ranger;

        _projectile = ranger.Projectile;
        _bulletSpeed = ranger.ProjectileSpeed;
        _bulletDamage = ranger.AttackDamage;
        _chargedProjectile = ranger.ChargedProjectile;
        _chargeAttackColor = ranger.ChargeAttackColor;
        _chargeAttackFlashDuration = ranger.ChargeAttackFlashDuration;
        _chargeAttackCooldown = ranger.ChargeAttackCooldown;
    }

    protected override void AimAtTarget()
    {
        base.AimAtTarget(_armPivot, _currentTarget);
    }
    protected override void AttackPlayer()
    {
        if (!_isOnCooldown && !_isCharged && _canAttack)
        {
            RaycastHit2D hit = Physics2D.Raycast(_armPivot.position, _armPivot.right, _attackRange, _hitLayers);

            if (hit && !hit.collider.CompareTag(_playerTag))
            {
                _agent.stoppingDistance = 0.0f;
                return;
            }
            else if (hit)
            {
                _agent.stoppingDistance = _attackRange;
            }

            _weaponAnimator.SetTrigger("Attack");
            Vector2 direction = (_playerTarget.position - _attackPoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            EnemyProjectileBullet firedBullet = Instantiate(_projectile, _attackPoint.position, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<EnemyProjectileBullet>();
            firedBullet.SetStats(_bulletDamage, _bulletSpeed, direction);
            StartCoroutine(AttackCooldown());

            if (_canCharge)
                _chargedShotCounter++;
        }

        if (_chargedShotCounter == _chargedShotMax)
        {
            _isCharged = true;
            _canAttack = false;
        }
        else
        {
            _isCharged = false;
        }
    }

    public override void Die(EnemyBase currentEnemy)
    {
        base.Die(this);

        _armPivot.gameObject.SetActive(false);

        if (Task != null && Task is TaskKill killTask)
            killTask.AddToCurrentKills();
    }
}