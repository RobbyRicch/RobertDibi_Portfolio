using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntryAnimationManager : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private EnemyBase _targetEnemy;
    [SerializeField] private Animator _enemyAnimator;
    [SerializeField] private NavMeshAgent _enemyAgent;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private SpriteRenderer _enemyWeapon;

    [Header("Player")]
    [SerializeField] public Player_Controller _playerGO;

    [Header("Should?")]
    [SerializeField] private bool _shouldGiveTarget;
    [SerializeField] private bool _shouldDestroyScript;
    [SerializeField] private bool _shouldActivateEnemy;
    [SerializeField] private bool _shouldActivateNavAgent;
    [SerializeField] private bool _hasWeapon;

    [Header("Coroutine Times")]
    [SerializeField] private float _timeToActivateScript;
    void Start()
    {
        _enemyAnimator.SetTrigger("Entry");
        if (_shouldActivateEnemy)
        {
            StartCoroutine(ActivateEnemy(_timeToActivateScript));
        }

        if (_playerGO == null)
        {
            _playerGO = SaveManager.Instance.Player;

        }
    }

    private IEnumerator ActivateEnemy(float time)
    {
        yield return new WaitForSeconds(time);
        if (_shouldActivateNavAgent)
        {
            _enemyAgent.enabled = true;

        }
        _targetEnemy.enabled = true;
        _enemyAnimator.ResetTrigger("Entry");
        _boxCollider.enabled = true;
        if (_hasWeapon)
        {
            _enemyWeapon.enabled = true;
        }


        if (_shouldGiveTarget && _targetEnemy != null)
        {
            _targetEnemy.PlayerTarget = _playerGO.transform;
        }


        this.enabled = false;
        if (_shouldDestroyScript)
        {
            Destroy(this, 0.45f);
        }
    }
}
