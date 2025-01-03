using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GOB_AI : EnemyHenchman
{
    [Header("G.O.B Components")]
    [SerializeField] protected Transform _armPivot;
    [SerializeField] private BoxCollider2D _hookCollider;
    [SerializeField] private Transform _carryingTransform;
    [SerializeField] private Animator _hookAnimator;
    [SerializeField] private GameObject _stealHighlighter;
    [SerializeField] private SpriteRenderer _stealWeaponIcon;
    [SerializeField] private AudioClip _gobbedAC;
    private Player_Controller _playerController;
    

    [Header("Drops")]
    [SerializeField] private GunBase _carriedWeapon;
    [SerializeField] private Color _originalColor;
    private Vector3 _dropGunScale = new(2.5f, 2.5f, 1.0f);

    [Header("States")]
    [SerializeField] private bool _isHooking;
    [SerializeField] private bool _hasWeapon;

    private IEnumerator _stealthModeRoutine;
    private IEnumerator _attemptHookRoutine;

    [Header("Gizmo")]
    private SpriteRenderer _carriedSprite;

    protected override void Update()
    {
        base.Update();

        if (_isChasing && !_hasWeapon)
        {
            ChasePlayer();
        }
        else if (_hasWeapon && _isAlive)
        {
            _hookAnimator.SetBool("CanHook", false);
            _stealthModeRoutine = StealthMode(0.35f, 1);
            StartCoroutine(_stealthModeRoutine);
            RunAway();
        }
        else if (_isPatroling)
        {
            Patrol();
        }
        else
        {
            _hookAnimator.SetBool("CanHook", true);
            _attemptHookRoutine = AttemptHook();
            StartCoroutine(_attemptHookRoutine);
        }

        if (_playerTarget != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTarget.transform.position);
            _currentTarget = _playerTarget.transform.position;

            if (distanceToPlayer <= _agent.stoppingDistance)
                _isChasing = false;
            else
                _isChasing = true;
        }

        transform.rotation = Quaternion.identity;
    }

    private IEnumerator AttemptHook()
    {
        _isHooking = true;
        yield return new WaitForSeconds(0.5f);
        _isHooking = false;
        _attemptHookRoutine = null;
    }
    private IEnumerator StealthMode(float targetOpacity, float duration)
    {
        List<Color> originalColors = new List<Color>();
        foreach (SpriteRenderer renderer in _spriteRenderers)
        {
            originalColors.Add(renderer.color);
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            for (int i = 0; i < _spriteRenderers.Count; i++)
            {
                Color currentColor = Color.Lerp(originalColors[i], new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, targetOpacity), elapsedTime / duration);

                _spriteRenderers[i].color = currentColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < _spriteRenderers.Count; i++)
        {
            _spriteRenderers[i].color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, targetOpacity);
        }

        _stealthModeRoutine = null;
    }

    private void RunAway()
    {
        float detectionRadius = 5f;

        if (Vector3.Distance(transform.position, _playerController.transform.position) <= detectionRadius)
        {
            Vector3 directionToPlayer = transform.position - _playerController.transform.position;
            Vector3 runAwayPosition = transform.position + directionToPlayer.normalized * 10f;

            _agent.SetDestination(runAwayPosition);
        }
    }

    protected override void AimAtTarget()
    {
/*        if (!_hasWeapon || !_playerTarget)
        {
            base.AimAtTarget();
            return;
        }*/

        base.AimAtTarget(_armPivot, _currentTarget);
    }
    protected override void ChasePlayer()
    {
        _hookAnimator.SetBool("CanHook", false);
        base.ChasePlayer();
    }
    protected override void HenchmanSOInitialization()
    {
        AI_SO_GOB gobData = _so as AI_SO_GOB;
        _bulletDamage = gobData.AttackDamage;
        _damageColor = gobData.DamageColor;
        _focusToGivePlayer = gobData.FocusToGivePlayer;
        _knockBackTime = gobData.KnockBackTime;
        _timeBetweenAttacks = gobData.TimeBetweenAttacks;
        _spotPlayerAC = gobData.SpotPlayerAC;
        _hurtAC = gobData.HurtAC;
        _spawnAC = gobData.SpawnAC;
        _idleAC = gobData.IdleAC;
        _patrolingAC = gobData.PatrolingAC;
        _attackAC = gobData.AttackAC;
        _deathAC = gobData.DeathAC;
        _endDeathAC = gobData.EndDeathAC;
        _itemDropAC = gobData.ItemDropAC;
        _attackRange = gobData.AttackRange;
        _maxHealth = gobData.MaxHealth;
        _moveSpeed = gobData.MoveSpeed;
    }
    protected override void DetectPlayer(Collider2D collision)
    {
        base.DetectPlayer(collision);
        _playerController = _playerTarget.GetComponent<Player_Controller>();

        if (!_hookCollider.enabled || !_isHooking)
            return;

        if (_hookCollider.enabled && _isHooking)
        {
            if (_hasWeapon || !_playerController.CurrentlyEquippedGun)
                return;

            GunBase playerCurrentWeapon = _playerController.CurrentlyEquippedGun;

            if (playerCurrentWeapon is GunPrimary)
            {
                if (_playerController.SideArm)
                    _playerController.SwapGuns();
                else
                    _playerController.HostlerGun();

                _playerController.PrimaryGun = null;
                _playerController.CurrentlyEquippedGun = _playerController.SideArm;
            }
            else if (playerCurrentWeapon is GunSideArm)
            {
                if (_playerController.PrimaryGun)
                    _playerController.SwapGuns();
                else
                    _playerController.HostlerGun();

                _playerController.SideArm = null;
                _playerController.CurrentlyEquippedGun = _playerController.PrimaryGun;

            }


            if (playerCurrentWeapon)
            {
                _carriedWeapon = playerCurrentWeapon;
                EventManager.InvokeUpdateStatusEffect(false, "WEAPON STOLEN!", _gobbedAC);
                _stealWeaponIcon.sprite = playerCurrentWeapon.SR.sprite;
                _stealHighlighter.SetActive(true);
                _carriedWeapon.SR.enabled = true;
                Transform weaponTransform = _carriedWeapon.transform;
                weaponTransform.position = _carryingTransform.position;
                weaponTransform.rotation = Quaternion.identity;
                weaponTransform.SetParent(_carryingTransform);
                _hasWeapon = true;
                _agent.stoppingDistance = 0;
                _isChasing = false;
                playerCurrentWeapon.CanFire = false;
                playerCurrentWeapon.enabled = false;
                _spriteRenderers.Add(playerCurrentWeapon.SR);
                _carriedSprite = playerCurrentWeapon.SR;
            }
        }
    }

    public override void Die(EnemyBase currentEnemy)
    {
        //StopAllCoroutines();

        if (_carriedWeapon)
        {
            _carriedWeapon.transform.SetParent(null);
            _carriedWeapon.Pickup.Collider.enabled = true;
            _carriedWeapon.transform.rotation = Quaternion.identity;
            _carriedWeapon.transform.localScale = _dropGunScale;
            _carriedWeapon.SR.enabled = true;
            _carriedWeapon.IsEquipped = false;
            _carriedWeapon.IsFiring = false;
            _carriedWeapon.IsFull = true;
            _carriedWeapon.CanFire = false;
            _carriedWeapon.Pickup.gameObject.SetActive(true);
            _carriedSprite.color = _originalColor;
            _carriedWeapon.enabled = true;
            _carriedWeapon.HighlightVFX.SetActive(true);
            _hasWeapon = false;
            _spriteRenderers.Remove(_carriedWeapon.SR);
            _carriedSprite = null;
        }
        _stealHighlighter.SetActive(false);
        _collider2D.enabled = false;
        _hookCollider.enabled = false;
        _canAttack = false;
        _rb2D.simulated = false;
        _agent.enabled = false;
        _isChasing = false;
        _isPatroling = false;

        _enemyAnimator.SetTrigger("Death");

        EventManager.InvokeGainFocus(_focusToGivePlayer);
        EventManager.InvokeEnemyDeath(this);

        _isAlive = false;
        enabled = false;
    }
}
