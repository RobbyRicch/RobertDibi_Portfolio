using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;

public class MeteorShowerManager : MonoBehaviour
{
    [SerializeField] private GameObject _meteorStrikeObj;
    //[SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _amountToStartSpawning = 1;
    [SerializeField] private int _amountToAddSpawning = 2;
    [SerializeField] private float _timeBetweenSpawns = 2f;
    [SerializeField] private float _timeToGetToLowestTime = 20f;
    private float _timer = 0;
    private float _lifeTimer = 0;

    private Vector3 testpos;
    private void Start()
    {
        //StartCoroutine(LerpSpawn());
        _timer = _timeBetweenSpawns;
    }
    void Update()
    {
        if (_timer < _timeBetweenSpawns)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _timer = 0;
            for (int i = 0; i < _amountToStartSpawning; i++)
            {
                GameObject newOBJ = Instantiate(_meteorStrikeObj, CheckIsAboveGround(GetRandomPosition()), Quaternion.identity);
                Destroy(newOBJ, 5f);
            }
        }

        if (_lifeTimer < 30)
        {
            _lifeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_lifeTimer / 100f);
            _amountToStartSpawning = Mathf.RoundToInt(Mathf.Lerp(1, 20, t));
            _timeBetweenSpawns = Mathf.Lerp(1, 0.1f, t);
        }

    }
    private Vector3 GetRandomPosition()
    {
        if (_sphereCollider == null)
        {
            Debug.LogError("No BoxCollider component found on this GameObject.");
            return Vector3.zero;
        }

        Vector3 boundsSize = _sphereCollider.bounds.size;
        Vector3 randomPoint = new Vector3(
            Random.Range(-boundsSize.x / 2f, boundsSize.x / 2f),
            0,
            Random.Range(-boundsSize.z / 2f, boundsSize.z / 2f)
        );

        return _sphereCollider.transform.TransformPoint(randomPoint);
    }
    private Vector3 CheckIsAboveGround(Vector3 pos)
    {
        if (Physics.Raycast(pos, Vector3.down * 5, _groundLayer))
        {
            return pos;
        }
        return CheckIsAboveGround(GetRandomPosition());
    }
    IEnumerator LerpSpawn()
    {
        while (_amountToStartSpawning < 20)
        {
            yield return new WaitForSeconds(2);
            _amountToStartSpawning = (int)Mathf.Lerp(1, 20, Time.deltaTime / 2);

        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, Vector3.up * 5);
    }
}
