using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class WaveHolder : MonoBehaviour
{
    [System.Serializable]
    public class WaveEnemies
    {
        [Range(0, 100)]
        public int EnemyPercentage;
        public GameObject EnemyPrefab;
    }

    private bool _activatedWave;
    [SerializeField] private bool _waveActive;
    [SerializeField] private int _spawnEnemyCount;
    private int _totalEnemyCount;
    private int _activeEnemyCount;
    [SerializeField] private Transform _spawnersHolder, _bakedEnemiesHolder;
    [SerializeField] List<WaveEnemies> _waveSetEnemies;
    private DynamicEnemySpawner[] _dynamicSpawners;
    private EnemyAI[] _bakedEnemies;
    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    public bool WaveActive { get => _waveActive; set => _waveActive = value; }
    public int TotalEnemyCount { get => _totalEnemyCount; }
    public int ActiveEnemyCount { get => _activeEnemyCount; }
    public List<WaveEnemies> WaveSetEnemies { get => _waveSetEnemies; }
    public List<GameObject> SpawnedEnemies { get => _spawnedEnemies; }

    private void Awake()
    {
        _dynamicSpawners = _spawnersHolder.GetComponentsInChildren<DynamicEnemySpawner>();
        _bakedEnemies = _bakedEnemiesHolder.GetComponentsInChildren<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_waveActive)
        {
            if (!_activatedWave)
            {
                _activatedWave = true;
                foreach (var spawner in _dynamicSpawners)
                {
                    spawner.CanSpawn = true;
                }

                StartCoroutine(ActivateBakedWithDelay());
            }

            int activeEnemiesAmount = 0;
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy.activeInHierarchy)
                {
                    activeEnemiesAmount++;
                }
            }
            _activeEnemyCount = activeEnemiesAmount;
        }
    }


    public void PrepareWave()
    {
        for (int i = 0; i < _spawnEnemyCount; i++)
        {
            _spawnedEnemies.Add(SpawnEnemy());
            _totalEnemyCount++;
        }

        for (int i = 0; i < _bakedEnemies.Length; i++)
        {
            _spawnedEnemies.Add(_bakedEnemies[i].gameObject);
            _bakedEnemies[i].gameObject.SetActive(false);
            _totalEnemyCount++;
        }
    }

    private GameObject SpawnEnemy()
    {
        int total = 0;
        foreach (var enemyType in _waveSetEnemies)
        {
            total += enemyType.EnemyPercentage;
        }

        float value = total * Random.value;

        int sum = 0;
        foreach (var enemyType in _waveSetEnemies)
        {
            sum += enemyType.EnemyPercentage;
            if (value <= sum)
            {
                if (enemyType.EnemyPrefab != null)
                {
                    int spawnerIndex = Random.Range(0, _dynamicSpawners.Length);

                    GameObject enemy = _dynamicSpawners[spawnerIndex].InstantiateEnemy(enemyType.EnemyPrefab);
                    return enemy;
                }
                break;
            }
        }
        return null;
    }

    // For Baked Enemies ONLY
    IEnumerator ActivateBakedWithDelay()
    {
        for (int i = 0; i < _bakedEnemies.Length; i++)
        {
            yield return new WaitForSeconds(Random.Range(0, 1));
            _bakedEnemies[i].gameObject.SetActive(true);
        }
    }
}
