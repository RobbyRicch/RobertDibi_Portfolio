using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    [HorizontalLine]

    [Foldout("Enemy Base"), Header("Scriptable Object")]
    [SerializeField] protected AI_SO _so;

    #region Components
    [Foldout("Enemy Base"), Header("Components")]
    [SerializeField] protected GameObject _dropItem;

    [Foldout("Enemy Base")]
    [SerializeField] protected Animator _enemyAnimator;

    [Foldout("Enemy Base")]
    [SerializeField] protected Collider2D _collider2D;

    [Foldout("Enemy Base")]
    [SerializeField] protected Rigidbody2D _rb2D;

    [Foldout("Enemy Base")]
    [SerializeField] protected NavMeshAgent _agent;

    [Foldout("Enemy Base")]
    [SerializeField] protected AudioSource _enemyAudioSource;

    [Foldout("Enemy Base")]
    [SerializeField] protected List<SpriteRenderer> _spriteRenderers;
    public List<SpriteRenderer> SpriteRenderers => _spriteRenderers;

    [Foldout("Enemy Base")]
    [SerializeField] protected Transform _playerTarget;
    public Transform PlayerTarget { get => _playerTarget; set => _playerTarget = value; }
    #endregion

    #region SO Data
    protected AudioClip _spawnAC = null, _idleAC = null, _hurtAC = null, _deathAC = null, _endDeathAC = null, _itemDropAC = null;
    protected Color _damageColor = new();
    protected int _maxHealth = 1;
    protected float _moveSpeed = 3.0f;
    protected int _bulletDamage = 1;
    protected float _attackRange = 2.0f;
    protected float _timeBetweenAttacks = 0.4f;
    protected float _knockBackTime = 0.15f;
    protected float _focusToGivePlayer = 10.0f;
    protected int _currencyToGivePlayer = 5;
    #endregion

    #region Flags
    [Foldout("Enemy Base"), Header("Flags")]
    [SerializeField] public bool _isAlive = true;

    [Foldout("Enemy Base")]
    [SerializeField] protected bool _isFacingRight = true;

    [Foldout("Enemy Base")]
    [SerializeField] protected bool _canAttack = false;

    [Foldout("Enemy Base")]
    [SerializeField] protected bool _isFlashing = false;

    [Foldout("Enemy Base")]
    [SerializeField] protected bool _isChasing = false;
    #endregion


    #region Other Members
    [Foldout("Enemy Base"), Header("Other Members")]
    protected const string _playerTag = "Player";

    [Foldout("Enemy Base")]
    protected float _currentHealth = 0;

    [Foldout("Enemy Base")]
    protected bool _isKnockedBack = false;
    #endregion

    #region Virtual Methods

    protected virtual void Awake()
    {
        _agent.updateRotation = false;
        transform.rotation = Quaternion.identity;
    }
    protected virtual void Start()
    {
        Initialize();
    }
    protected virtual void Update()
    {
        if (!_isAlive)
        {
            _agent.enabled = false;
            this.enabled = false;
            return;
        }

        FlipEnemy();
        AimAtTarget();
    }

    protected virtual IEnumerator HandleKnockback(Vector2 normalizedAttackDirection, float knockBackPower)
    {
        if (_isKnockedBack)
            yield break;

        float elapsedTime = 0f;
        Vector3 originalPos = transform.position;
        Vector3 targetPos = originalPos + (Vector3)normalizedAttackDirection * knockBackPower;

        _isKnockedBack = true;
        while (elapsedTime < _knockBackTime)
        {
            float t = elapsedTime / _knockBackTime;
            float easeOutT = 1 - Mathf.Pow(1 - t, 3);  // Cubic ease-out
            transform.position = Vector3.Lerp(originalPos, targetPos, easeOutT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        _isKnockedBack = false;
    }
    protected virtual IEnumerator DamageFlash()
    {
        _isFlashing = true; // Set the flag to indicate that damage flash is in progress

        // Store original colors
        List<Color> originalColors = new List<Color>();
        foreach (SpriteRenderer renderer in _spriteRenderers)
        {
            originalColors.Add(renderer.color);
            renderer.color = _damageColor;
        }

        yield return new WaitForSeconds(0.1f);

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * 5f; // Adjust the speed of color lerp here
            for (int i = 0; i < _spriteRenderers.Count; i++)
            {
                _spriteRenderers[i].color = Color.Lerp(_damageColor, originalColors[i], elapsedTime);
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

    protected virtual void SOInitialization()
    {
        _spawnAC = _so.SpawnAC;
        _idleAC = _so.IdleAC;
        _hurtAC = _so.HurtAC;
        _deathAC = _so.DeathAC;
        _endDeathAC = _so.EndDeathAC;
        _itemDropAC = _so.ItemDropAC;
        _damageColor = _so.DamageColor;
        _maxHealth = _so.MaxHealth;
        _moveSpeed = _so.MoveSpeed;
        _bulletDamage = _so.AttackDamage;
        _attackRange = _so.AttackRange;
        _knockBackTime = _so.KnockBackTime;
        _focusToGivePlayer = _so.FocusToGivePlayer;
        _timeBetweenAttacks = _so.TimeBetweenAttacks;
        _currencyToGivePlayer = _so.CurrencyToGivePlayer;

    }
    protected virtual void Initialize()
    {
        _agent.updateRotation = false;

        SOInitialization();

        _agent.speed = _moveSpeed;
        _currentHealth = _maxHealth;
        _isAlive = true;
        transform.rotation = Quaternion.identity;
    }

    protected virtual void AimAtTarget()
    {
        if (!_isAlive)
            return;

        Vector3 directionToPlayer = _playerTarget.transform.position - transform.position;
        if (directionToPlayer.x < 0 && _isFacingRight)
        {
            FlipEnemy();
            _enemyAnimator.SetBool("IsFacingRight", false);
            //Debug.Log("flipped left");

        }
        else if (directionToPlayer.x > 0 && !_isFacingRight)
        {
            FlipEnemy();
            _enemyAnimator.SetBool("IsFacingRight", true);
            //Debug.Log("flipped right");
        }
    }
    protected virtual void FlipEnemy()
    {
        if (!_isAlive)
            return;

        _isFacingRight = !_isFacingRight;

        Vector3 characterScale = transform.localScale;
        characterScale.x *= -1;
        transform.localScale = characterScale;
    }
    protected virtual void ChasePlayer()
    {
        if (!_isAlive)
            return;

        if (_playerTarget == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTarget.transform.position);

        if (distanceToPlayer > _attackRange)
        {
            _isChasing = true;
            _agent.isStopped = false; // Ensure the agent is not stopped
            if (_agent != null || _agent.enabled == true)
            {
                _agent.SetDestination(_playerTarget.transform.position);
                _enemyAnimator.SetBool("IsMoving", true);
            }
            else
            {
                _enemyAnimator.SetBool("IsMoving", false);
                _agent.SetDestination(gameObject.transform.position);


            }
        }
        else
        {
            _isChasing = false;
            _agent.isStopped = true; // Stop the agent completely
            _agent.ResetPath(); // Clear the current path
            _enemyAnimator.SetBool("IsMoving", false);
        }
    }
    protected virtual void CheckTask() { }

    public virtual void TakeDamage(Vector2 normalizedAttackDirection, float damage, float knockBackPower)
    {
        if (!_isAlive)
            return;

        _currentHealth -= damage;

        if (!_isKnockedBack)
            StartCoroutine(HandleKnockback(normalizedAttackDirection, knockBackPower));

        if (!_isFlashing)
            StartCoroutine(DamageFlash());

        if (_currentHealth <= 0)
            Die(this);
    }
    public virtual void Die(EnemyBase currentEnemy)
    {
        _collider2D.enabled = false;
        _canAttack = false;
        _rb2D.simulated = false;
        _agent.enabled = false;
        _enemyAudioSource.pitch = Random.Range(0.1f, 2f);
        _enemyAudioSource.PlayOneShot(_deathAC);
        _enemyAnimator.SetTrigger("Death");
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
/*        if (_dropItem != null)
        {
            _dropItem.transform.SetParent(null);
            _dropItem.SetActive(true);
        }*/

        CheckTask();
        EventManager.InvokeGainFocus(_focusToGivePlayer);
        EventManager.InvokeGainCurrency(_currencyToGivePlayer);

        if (!currentEnemy)
            EventManager.InvokeEnemyDeath(this);
        else
            EventManager.InvokeEnemyDeath(currentEnemy);

        _isAlive = false;
        enabled = false;
        return;
    }
    #endregion
}
