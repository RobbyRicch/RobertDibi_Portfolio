using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicEnemySpawner : MonoBehaviour
{
    public bool CanSpawn;
    [SerializeField] private Vector3 _spawnSectorRange = new Vector3(5, 0, 5);
    [SerializeField] private Vector3 _spawnSectorOffset;
    [SerializeField] private float _maxSpawnDelay;
    [SerializeField] private bool _useDelay;
    private List<GameObject> _spawnerEnemyInstances = new List<GameObject>();
    private bool _spawningActivated;


    // Update is called once per frame
    void Update()
    {
        if (CanSpawn)
        {
            ActivateEnemies();
        }
    }


    private void ActivateEnemies()
    {
        if (!_spawningActivated)
        {
            _spawningActivated = true;

            if (_useDelay)
            {
                StartCoroutine(ActivateWithDelay());
            }
            else
            {
                for (int i = 0; i < _spawnerEnemyInstances.Count; i++)
                {
                    _spawnerEnemyInstances[i].SetActive(true);
                }
            }
        }
    }

    IEnumerator ActivateWithDelay()
    {
        for (int i = 0; i < _spawnerEnemyInstances.Count; i++)
        {
            yield return new WaitForSeconds(Random.Range(0, _maxSpawnDelay));
            _spawnerEnemyInstances[i].SetActive(true);
        }
    }

    public GameObject InstantiateEnemy(GameObject enemyPrefab)
    {
        Vector3 randPos = transform.position + _spawnSectorOffset + new Vector3(Random.Range(-_spawnSectorRange.x, _spawnSectorRange.x), Random.Range(-_spawnSectorRange.y, _spawnSectorRange.y), Random.Range(-_spawnSectorRange.z, _spawnSectorRange.z));
        GameObject enemy = Instantiate(enemyPrefab, randPos, enemyPrefab.transform.rotation, transform);
        enemy.SetActive(false);
        _spawnerEnemyInstances.Add(enemy);
        return enemy;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + _spawnSectorOffset, _spawnSectorRange * 2);
    }
}
