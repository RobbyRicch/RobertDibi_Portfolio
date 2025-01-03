using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class TZ_FinalRoomTrigger : MonoBehaviour
{
    [Header("Player Refs")]
    [SerializeField] private Player_Controller _player;
    public Player_Controller Player { get => _player; set => _player = value; }

    [SerializeField] private Transform _playerSpot;

    [Header("Enemy Refs")]
    [SerializeField] private GameObject _enemyGO;
    [SerializeField] private EnemyBase _enemyScript;
    [SerializeField] private NavMeshAgent _enemyAgent;
    [SerializeField] private EntryAnimationManager _entryAnimationManager;
    [SerializeField] private Transform _enemyMoveTo;
    [SerializeField] private float _enemyattack;
    [SerializeField] private float _enemyMoveBack;

    [Header("HUD Refs")]
    [SerializeField] private GameObject _canvasForDamage;
    [SerializeField] private GameObject _canvasForHP;
    [SerializeField] private BoxCollider2D _Canvastrigger;

    [Header("Self")]
    [SerializeField] private BoxCollider2D _boxCollider2d;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(RunCutscene(_enemyattack , _enemyMoveBack));
    }

    private IEnumerator RunCutscene(float timeForEnemy , float timeToMoveBack)
    {
        _boxCollider2d.enabled = false;
        _player.IsInputDisabled = true;
        _player.IsMovementOnlyDisabled = true;
        _player.transform.position = _playerSpot.transform.position;
        yield return new WaitForSeconds(1.5f);
        _enemyGO.SetActive(true);
        yield return new WaitForSeconds(timeForEnemy);
        //_enemyScript.PlayerTarget = null;
        _enemyScript.transform.rotation = Quaternion.identity;
        _enemyAgent.acceleration = 0;
        _enemyAgent.speed = 0;
        _enemyGO.transform.position = _enemyMoveTo.transform.position;
        yield return new WaitForSeconds(timeToMoveBack);
        _player.IsInputDisabled = false;
        _player.IsMovementOnlyDisabled = false;
        //Destroy(_entryAnimationManager);
        //_Canvastrigger.enabled = true;
        _canvasForDamage.SetActive(true);
        yield return new WaitForSeconds(5);
        _canvasForDamage.SetActive(false);
        _canvasForHP.SetActive(true);
    }
}
