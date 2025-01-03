using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _spawnerCollisionRange = 5;
    [SerializeField] private Vector3 _spawnSectorRange = new Vector3(5, 0, 5);
    [SerializeField] private Vector3 _spawnSectorOffset;
    [SerializeField] private float _maxSpawnDelay;
    [SerializeField] private float _maxRespawnDelay;
    [SerializeField] private bool UseDelay;
    [SerializeField] private bool CanRespawn;
    private bool _playerInSightRange;

    public bool CanSpawn = true;
    public bool SpawningActivated;

    [SerializeField] private List<GameObject> PrefabsEnemyTypes = new List<GameObject>();
    [SerializeField] private List<int> AmountEnemyTypes = new List<int>();
    private List<GameObject> _allEnemyInstances = new List<GameObject>();
    private List<bool> _isRespawning = new List<bool>();

    private void Awake()
    {
        SetupEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        _playerInSightRange = Physics.CheckSphere(transform.position, _spawnerCollisionRange, _playerLayer);

        if (_playerInSightRange && CanSpawn)
        {
            ActivateEnemies();
        }

        RespawnEnemies();
    }

    private void RespawnEnemies()
    {
        if (SpawningActivated && CanRespawn)
        {
            for (int i = 0; i < _allEnemyInstances.Count; i++)
            {
                if (!_allEnemyInstances[i].activeInHierarchy)
                {
                    if (!_isRespawning[i])
                    {
                        _isRespawning[i] = true;
                        StartCoroutine(RespawnDelay(i));
                    }
                }
            }
        }
    }

    IEnumerator RespawnDelay(int i)
    {
        yield return new WaitForSeconds(Random.Range(0, _maxRespawnDelay));
        Vector3 respawnPos = transform.position + _spawnSectorOffset + new Vector3(Random.Range(-_spawnSectorRange.x, _spawnSectorRange.x), Random.Range(-_spawnSectorRange.y, _spawnSectorRange.y), Random.Range(-_spawnSectorRange.z, _spawnSectorRange.z));
        _allEnemyInstances[i].transform.position = respawnPos;
        _allEnemyInstances[i].SetActive(true);
        _isRespawning[i] = false;
    }

    private void ActivateEnemies()
    {
        if (!SpawningActivated)
        {
            SpawningActivated = true;

            if (UseDelay)
            {
                StartCoroutine(ActivateWithDelay());
            }
            else
            {
                for (int i = 0; i < _allEnemyInstances.Count; i++)
                {
                    _allEnemyInstances[i].SetActive(true);
                }
            }
        }
    }

    IEnumerator ActivateWithDelay()
    {
        for (int i = 0; i < _allEnemyInstances.Count; i++)
        {
            _allEnemyInstances[i].SetActive(true);
            yield return new WaitForSeconds(Random.Range(0, _maxSpawnDelay));
        }
    }

    private void SetupEnemies()
    {
        for (int i = 0; i < PrefabsEnemyTypes.Count; i++)
        {
            for (int g = 0; g < AmountEnemyTypes[i]; g++)
            {
                _allEnemyInstances.Add(InstantiateEnemy(PrefabsEnemyTypes[i]));
                _isRespawning.Add(false);
            }
        }
    }

    private GameObject InstantiateEnemy(GameObject enemyPrefab)
    {
        Vector3 randPos = transform.position + _spawnSectorOffset + new Vector3(Random.Range(-_spawnSectorRange.x, _spawnSectorRange.x), Random.Range(-_spawnSectorRange.y, _spawnSectorRange.y), Random.Range(-_spawnSectorRange.z, _spawnSectorRange.z));
        GameObject enemy = Instantiate(enemyPrefab, randPos, enemyPrefab.transform.rotation, transform);
        enemy.SetActive(false);
        return enemy;
    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + _spawnSectorOffset, _spawnSectorRange * 2);
        if (_playerInSightRange)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(transform.position, _spawnerCollisionRange);
    }
}
