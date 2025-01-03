using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CommandBot_AI : EnemyHenchman
{
    [Header("Unique Behavior - CommandBot Coliders")]
    [SerializeField] private CircleCollider2D _buffCollider;
    [SerializeField] private BoxCollider2D _searchCollider;

    [Header("Unique Behavior - CommandBot Tentacles")]
    [SerializeField] private GameObject _buffTentacles, _roamingTentacles;

    [Header("Unique Behavior - CommandBot States")]
    [SerializeField] private bool _roaming;
    [SerializeField] private bool _isBuffing;
    [SerializeField] private bool _lookingForFriend;
    [SerializeField] private bool _hasFriend;
    [SerializeField] private bool _isFollowingFriend;


    [Header("Unique Behavior - CommandBot Refs")]
    [SerializeField] private Transform _friendTransform;
    [SerializeField] private GameObject _currentFriend;

    [Header("Unique Behavior - CommandBot Buff")]
    [SerializeField] private List<EnemyHenchman> _surroundingFriends;
    [SerializeField] private int _buffAmount;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (!_isAlive)
        {
            _agent.enabled = false;
            this.enabled = false;
            return;
        }

        if (_roaming)
        {
            RoamingMethod();
        }

        if (_isBuffing && !_buffTentacles.activeInHierarchy && !_roaming)
        {
            BuffMethod();
        }

        if (_isFollowingFriend)
        {
            _agent.SetDestination(_friendTransform.position);
        }

        if (_isFollowingFriend)
        {
            float distanceToFriend = Vector3.Distance(transform.position, _currentFriend.transform.position);

            _currentTarget = _currentFriend.transform.position;
            if (distanceToFriend <= _agent.stoppingDistance)
            {
                _isChasing = false;
                _isBuffing = true;
            }
            else
            {
                _isChasing = true;
            }
        }

        if (_isBuffing)
        {
            foreach (EnemyHenchman Friends in _surroundingFriends)
            {
                if (!Friends.BeingBuffed)
                {
                    Friends.BuffHenchman(_buffAmount);

                }
            }
        }

        if (_playerTarget != null)
        {
            FlipEnemy();
            AimAtTarget();

        }

        _agent.updateRotation = false;
        transform.rotation = Quaternion.identity;
    }

    private void BuffMethod()
    {
        _roaming = false;
        _buffCollider.gameObject.SetActive(true);
        _searchCollider.gameObject.SetActive(false);
        _buffTentacles.SetActive(true);
        _roamingTentacles.SetActive(false);

    }

    private void RoamingMethod()
    {
        _buffTentacles.SetActive(false);
        _roamingTentacles.SetActive(true);
        _searchCollider.enabled = true;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (_lookingForFriend && _searchCollider.enabled && !_hasFriend)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                _roaming = false;
                _hasFriend = true;
                _lookingForFriend = false;
                _currentFriend = collision.gameObject;
                _friendTransform = collision.gameObject.transform;
                _isFollowingFriend = true;
            }
        }

        if (_hasFriend && _isBuffing && _buffCollider.enabled && !_lookingForFriend)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<EnemyHenchman>();
                if (enemy != null && !_surroundingFriends.Contains(enemy))
                {
                    _surroundingFriends.Add(enemy);
                    Debug.Log("Added to surrounding friends: " + enemy.name);
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_hasFriend && _isBuffing && _buffCollider.enabled && !_lookingForFriend)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<EnemyHenchman>();
                if (enemy != null && !_surroundingFriends.Contains(enemy))
                {
                    _surroundingFriends.Add(enemy);
                    Debug.Log("Added to surrounding friends in OnTriggerStay: " + enemy.name);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_hasFriend && _isBuffing && _buffCollider.enabled && !_lookingForFriend)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<EnemyHenchman>();
                if (enemy != null && _surroundingFriends.Contains(enemy))
                {
                    _surroundingFriends.Remove(enemy);
                    enemy.LoseBuff(_buffAmount);
                    Debug.Log("Removed from surrounding friends: " + enemy.name);
                }
            }
        }
    }

    public override void Die(EnemyBase currentEnemy)
    {
        base.Die(currentEnemy);
    }

    public override void TakeDamage(Vector2 normalizedAttackDirection, float damage, float knockBackPower)
    {
        base.TakeDamage(normalizedAttackDirection, damage, knockBackPower);
    }

    protected override void AimAtTarget()
    {
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



    protected override void Awake()
    {
        base.Awake();
    }


    protected override IEnumerator DamageFlash()
    {
        return base.DamageFlash();
    }

    protected override void DetectPlayer(Collider2D collision)
    {
        base.DetectPlayer(collision);
    }

    protected override void FlipEnemy()
    {
        base.FlipEnemy();
    }

    protected override IEnumerator HandleKnockback(Vector2 normalizedAttackDirection, float knockBackPower)
    {
        return base.HandleKnockback(normalizedAttackDirection, knockBackPower);
    }

    protected override void HenchmanSOInitialization()
    {
        base.HenchmanSOInitialization();
    }

    protected override void Initialize()
    {
        base.Initialize();
    }




    protected override void SOInitialization()
    {
        base.SOInitialization();
    }



}
