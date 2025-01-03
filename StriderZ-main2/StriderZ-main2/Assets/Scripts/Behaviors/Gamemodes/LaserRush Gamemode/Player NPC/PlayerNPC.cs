using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerNPC : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform Distenation;
    public bool IsReady = false;
    [SerializeField] private float minDistance = 1f;

    private bool _isAlive = true, _isInDanger = false, _isDead = false;
    public bool IsAlive { get => _isAlive; set => _isAlive = value; }
    public bool IsInDanger { get => _isInDanger; set => _isInDanger = value; }
    public bool IsDead { get => _isDead; set => _isDead = value; }
    private void Start()
    {
        Distenation = ArenaScrumbler.Instance.PlayerNPC_Destenation;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 100, -1))
        {
            transform.position = hit.position;
        }
        ArenaScrumbler.Instance.NavLink.endPoint += Vector3.up / 100;
    }

    // Update is called once per frame
    void Update()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 100, -1))
        {
            transform.position = hit.position;
        }
        if (IsReady)
        {
            RunNPCToDestenation();
        }
    }

    private void RunNPCToDestenation()
    {
        agent.enabled = true;
        float distanceToTarget = Vector3.Distance(transform.position, Distenation.position);
        /*if (distanceToTarget <= minDistance)
        {
            agent.SetDestination(transform.position);
        }
        else
        {*/
            agent.SetDestination(Distenation.position + Vector3.up);

        //}
        for (int i = 0; i < agent.path.corners.Length; i++)
            Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.red);
    }
}
