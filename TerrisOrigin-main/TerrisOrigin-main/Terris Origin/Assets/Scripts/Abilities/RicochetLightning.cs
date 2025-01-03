using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetLightning : MonoBehaviour
{
    private bool finishedHit = false;
    public List<GameObject> AllEnemies = new List<GameObject>();
    public GameObject closestEnemy;
    public GameObject FisrtEnemyHit;
    public float speed = 5;
    public bool GotHit = false;
    public bool EnemyInRange;
    public float CheckRange = 5;
    [SerializeField] private LayerMask EnemyLayer;
    [SerializeField] private Collider EnemyCollider;
    [SerializeField] private Rigidbody rb;
    //[SerializeField] protected GameObject ballGFX;

    public int HitTimes = 0;

    private float _timerBetweenHits = 0;
    private float _timeForHits = 1f;
    private bool _resetValues = false;

    private void OnEnable()
    {
        AllEnemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    /*private void OnDisable()
    {
        ResetValues();
    }*/

    // Update is called once per frame
    void Update()
    {
        if (_timerBetweenHits < _timeForHits)
        {
            if (GotHit && !finishedHit)
            {
                CheckCloseEnemy();
            }
            else
            {
                _timerBetweenHits += Time.deltaTime;
            }
        }
        else
        {
            _resetValues = true;
        }
        if (HitTimes > 2 || _resetValues)
        {
            ResetValues();
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            rb.velocity = Vector3.zero;
            //ballGFX.SetActive(false);
            GotHit = true;
            FisrtEnemyHit = other.gameObject;
            EnemyCollider = other;
            finishedHit = false;
        }
    }

    private void CheckCloseEnemy()
    {
        EnemyInRange = Physics.CheckSphere(transform.position, CheckRange, EnemyLayer);
        finishedHit = true;
        switch (HitTimes)
        {
            case 0:
                if (AllEnemies.Contains(FisrtEnemyHit))
                {
                    AllEnemies.Remove(FisrtEnemyHit);
                }
                closestEnemy = ClosestEnemy();
                if (AllEnemies.Count > 0)
                    StartCoroutine(RicochetBetweenEnemies());
                HitTimes++;
                _timerBetweenHits = 0;
                break;
            case 1:
                if (AllEnemies.Contains(FisrtEnemyHit))
                {
                    AllEnemies.Remove(FisrtEnemyHit);
                }
                closestEnemy = ClosestEnemy();
                if (AllEnemies.Count > 0)
                    StartCoroutine(RicochetBetweenEnemies());
                HitTimes++;
                _timerBetweenHits = 0;
                break;
            case 2:
                if (AllEnemies.Contains(FisrtEnemyHit))
                {
                    AllEnemies.Remove(FisrtEnemyHit);
                }
                closestEnemy = ClosestEnemy();
                //StartCoroutine(RicochetBetweenEnemies());
                HitTimes++;
                _timerBetweenHits = 0;
                GotHit = false;
                //ballGFX.SetActive(true);
                break;
        }
    }

    private GameObject ClosestEnemy()
    {
        GameObject closestHere = null;
        float leastDistance = 50;

        foreach (GameObject enemy in AllEnemies)
        {
            float distanceHere = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceHere < leastDistance)
            {
                leastDistance = distanceHere;
                closestHere = enemy;
            }
        }

        return closestHere;
    }

    private Vector3 GetDirectionToClosestEnemy()
    {
        Vector3 dir = closestEnemy.transform.parent.position - transform.position;
        dir.y = 0;
        return dir.normalized;
    }

    IEnumerator RicochetBetweenEnemies()
    {
        float distance = Vector3.Distance(transform.position, closestEnemy.transform.position);
        while (distance >= 0.1 || distance <= -0.1)
        {
            distance = Vector3.Distance(transform.position, closestEnemy.transform.position);
            transform.position += GetDirectionToClosestEnemy() * speed;
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, CheckRange);
    }

    public void ResetValues()
    {
        _timerBetweenHits = 0;
        _resetValues = false;
        GotHit = false;
        HitTimes = 0;
        finishedHit = false;
        EnemyInRange = false;
        FisrtEnemyHit = null;
        EnemyCollider = null;
        //ballGFX.SetActive(true);
        this.enabled = false;
        AllEnemies = new List<GameObject>();
    }
}
