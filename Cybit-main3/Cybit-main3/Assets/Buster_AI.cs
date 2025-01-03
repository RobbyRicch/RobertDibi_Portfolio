using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buster_AI : EnemyHenchman
{
    [Header("Unique Behaviors")]
    [SerializeField] protected Transform _armPivot;
    [SerializeField] protected Transform _attackPoint;
    [SerializeField] protected LayerMask _hitLayers;

    [Header("Unique Attacks - Ranged Attack")]
    [SerializeField] protected Animator _weaponsAnimator;
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected bool _shootLeft;
    [SerializeField] protected float _bulletHeightOffset;
    [Header("Unique Attacks - Rain Attack")]
    [SerializeField] protected int _rainAttackCounter;
    [SerializeField] protected int _rainAttackMax;
    [SerializeField] protected float _timeForRainRelease;
    [SerializeField] protected float _waitForRain;
    [SerializeField] protected GameObject _rainAttackGO;
    [SerializeField] protected GameObject _rainAttackLaunchVFX;

    [Header("Unique Behaviors - Attack States")]
    [SerializeField] protected bool _isCharging;
    [SerializeField] protected bool _isCharged;
    [SerializeField] protected bool _canCharge;
    [SerializeField] protected bool _hasFiredCharged;

    protected EnemyProjectileBullet _projectile;
    protected Color _chargeAttackColor = new();
    protected float _chargeAttackFlashDuration = 1.0f;
    protected float _chargeAttackCooldown = 1.0f;

    protected override void Update()
    {
        if (!_isAlive)
        {
            _agent.enabled = false;
            this.enabled = false;
            return;
        }

        FlipEnemy();
        AimAtTarget();


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
    private IEnumerator RainAttack()
    {
        if (_hasFiredCharged)
            yield break;

        _isCharged = false;
        _rainAttackLaunchVFX.SetActive(true);
        yield return new WaitForSeconds(_timeForRainRelease);

        // Instantiate the rain attack at the player's location
        if (_playerTarget != null)
        {
            Instantiate(_rainAttackGO, _playerTarget.position, Quaternion.identity);
        }
        yield return new WaitForSeconds(_waitForRain);

        _rainAttackCounter = 0;
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
            StartCoroutine(RainAttack());
        }
    }

    protected override void HenchmanSOInitialization()
    {
        base.HenchmanSOInitialization();
        _agent.stoppingDistance = _attackRange;

        AI_SO_Buster buster = _so as AI_SO_Buster;

        _projectile = buster.Projectile;
        _bulletSpeed = buster.ProjectileSpeed;
        _bulletDamage = buster.AttackDamage;
        _chargeAttackColor = buster.ChargeAttackColor;
        _chargeAttackFlashDuration = buster.ChargeAttackFlashDuration;
        _chargeAttackCooldown = buster.ChargeAttackCooldown;
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

            _weaponsAnimator.SetTrigger("Fire");
            Vector2 direction = (_playerTarget.position - _attackPoint.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Vector3 spawnPosition = _attackPoint.position + new Vector3(0, _bulletHeightOffset, 0); // Adjust bulletHeightOffset accordingly
            EnemyProjectileBullet firedBullet = Instantiate(_projectile, spawnPosition, Quaternion.AngleAxis(angle, Vector3.forward)).GetComponent<EnemyProjectileBullet>();
            firedBullet.SetStats(_bulletDamage, _bulletSpeed, direction);
            StartCoroutine(AttackCooldown());

            if (_canCharge)
                _rainAttackCounter++;
        }

        if (_rainAttackCounter == _rainAttackMax)
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

