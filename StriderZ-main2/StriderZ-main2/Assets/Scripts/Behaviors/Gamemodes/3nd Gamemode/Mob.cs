using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mob : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject RecourceRef;

    [Header("Death")]
    [SerializeField] private SphereCollider Area;
    private CheckTriggerWithTag checkTag;
    [SerializeField] private ParticleSystem Explosion;
    [SerializeField] private GameObject body;

    private bool _playerInAttackRange = false;
    private Transform _playerToFollow;
    private int rand;
    // Start is called before the first frame update
    private void Start()
    {
        Area.radius = attackRange + 1;
    }
    void OnEnable()
    {
        checkTag = Area.GetComponent<CheckTriggerWithTag>();
        rand = Random.Range(0, 101);
        if (rand >= 0 && rand <= 50)
        {
            foreach (var item in PlayerManager.Instance.AllPlayersAlive)
            {
                if (item.GetComponent<PlayerRole>().Role == PlayerRole.Roles.Encryptor)
                {
                    _playerToFollow = item.transform;
                }
            }

        }
        else if (rand >= 51 && rand <= 70)
        {
            foreach (var item in PlayerManager.Instance.AllPlayersAlive)
            {
                if (item.GetComponent<PlayerRole>().Role == PlayerRole.Roles.Infiltrator1)
                {
                    _playerToFollow = item.transform;
                }
            }
        }
        else if (rand >= 71 && rand <= 90)
        {
            foreach (var item in PlayerManager.Instance.AllPlayersAlive)
            {
                if (item.GetComponent<PlayerRole>().Role == PlayerRole.Roles.Infiltrator2)
                {
                    _playerToFollow = item.transform;
                }
            }
        }
        else if (rand >= 91 && rand <= 100)
        {
            foreach (var item in PlayerManager.Instance.AllPlayersAlive)
            {
                if (item.GetComponent<PlayerRole>().Role == PlayerRole.Roles.Disruptor)
                {
                    _playerToFollow = item.transform;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _playerInAttackRange = CheckRange(attackRange);
        if (_playerInAttackRange)
        {
            AttackSeq();
            agent.SetDestination(transform.position);
        }
        else
        {
            agent.SetDestination(_playerToFollow.position);
        }
    }

    private bool CheckRange(float range)
    {
        if (Physics.CheckSphere(transform.position, attackRange, playerLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void AttackSeq()
    {
        body.SetActive(false);
        Explosion.Play();
        Destroy(gameObject, Explosion.main.duration);
    }

    private void OnDisable()
    {
        PlayerManager.Instance.AllPlayersAlive[checkTag.TriggeredPlayerIndex].GetComponent<PlayerLives>().ReduceLive();
        Instantiate(RecourceRef, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Area.radius = attackRange + 1;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
