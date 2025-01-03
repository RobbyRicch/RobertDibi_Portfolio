using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentExitCell_Trigger : MonoBehaviour
{
    [Header("Goal : Trigger Guards On Player Cell Exit")]
    [Header("Guards")]
    [SerializeField] private List<EnemyBase> _guardScripts;
    [SerializeField] private List<NavMeshAgent> _guardAgents;

    [Header("Trigger")]
    [SerializeField] private BoxCollider2D _currentColider;

    [Header("Player Target")]
    [SerializeField] private GameObject _playerGO;



    private void Start()
    {
        Player_Controller playerController = FindObjectOfType<Player_Controller>();
        if (playerController != null)
        {
            _playerGO = playerController.gameObject;
        }
        else
        {
            Debug.LogError("Player_Controller not found in the scene.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (EnemyBase enemyScript in _guardScripts)
            {
                enemyScript.enabled = true;
                enemyScript.PlayerTarget = _playerGO.transform;
            }

            foreach (NavMeshAgent agent in _guardAgents)
            {
                agent.enabled = true;
            }

            _currentColider.enabled = false;
            EventManager.InvokeNewObjective("KILL THE GUARDS");

        }


    }

}
