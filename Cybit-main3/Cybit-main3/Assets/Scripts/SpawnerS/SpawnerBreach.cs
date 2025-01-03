using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBreach : SpawnerBase
{
    [Header("Breach Spawner Data")]
    [SerializeField] private EnemyBase[] _prefabs;
    [SerializeField] private List<EnemyBase> _spawnedEnemies = new();
    [SerializeField] private TaskBase _task;
    [SerializeField] private float _spawnRadius = 5f;
    [SerializeField] private float _timeToIncreaseBreachSpawn;
    [SerializeField] private bool _isSpawningOfficer = true;
    [SerializeField] private bool _isSpawningRanger = false;

    private float _originalSpawnRadius;
    private float _originalTimeToIncreaseSpawn;
    private int _originalMaxSpawnAmount;
    private int _originalCurrentSpawned;
    private int _originalTotalSpawned;
    private int _originalGroupSize;
    private float _originalSpawnCooldown;

    private void OnEnable()
    {
        EventManager.OnEnemyDeath += OnEnemyDeath;
        _maxSpawnAmount = _currentSpawned + _groupSize;
        ResetSpawners();
        StartSpawning();
    }

    private void Awake()
    {
        _originalMaxSpawnAmount = _maxSpawnAmount;
        _originalSpawnRadius = _spawnRadius;
        _originalTimeToIncreaseSpawn = _timeToIncreaseBreachSpawn;
        _originalCurrentSpawned = _currentSpawned;
        _originalTotalSpawned = _totalSpawned;
        _originalGroupSize = _groupSize;
        _originalSpawnCooldown = _spawnCooldown;
    }

    private void Update()
    {
        if (_task.IsTaskComplete == true)
        {
            EventManager.OnEnemyDeath -= OnEnemyDeath;
            foreach (EnemyBase enemy in _spawnedEnemies)
            {
                if (enemy)
                    enemy.Die(enemy);
                Destroy(enemy.gameObject);
            }
            _spawnedEnemies.Clear();
        }
        if (_shouldUpgrade)
        {
            StartCoroutine(BreachIncrease());
        }
    }

    private void OnDisable()
    {
        EventManager.OnEnemyDeath -= OnEnemyDeath;
    }

    private void Spawn()
    {
        if (_currentSpawned >= _maxSpawnAmount)
        {
            Debug.Log("Max spawn amount reached, not spawning more.");
            return;
        }

        // Calculate a random point within the spawn radius using polar coordinates
        float randomAngle = Random.Range(0f, Mathf.PI * 2f); // Generate a random angle
        float spawnX = transform.position.x + Random.Range(0f, _spawnRadius) * Mathf.Cos(randomAngle);
        float spawnZ = transform.position.z + Random.Range(0f, _spawnRadius) * Mathf.Sin(randomAngle);
        float spawnY = transform.position.y + Random.Range(-_spawnRadius, _spawnRadius); // Randomize Y coordinate
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

        Debug.Log($"Spawning enemy at position: {spawnPosition}");

        EnemyBase newEnemy = Instantiate(_prefabs[Random.Range(0, _prefabs.Length)], spawnPosition, Quaternion.identity);

        if (newEnemy == null)
        {
            Debug.LogError("Failed to instantiate enemy prefab.");
            return;
        }

        if (newEnemy is EnemyHenchman newHenchman)
        {
            newHenchman.PlayerTarget = _playerController.transform;

            switch (newHenchman)
            {
                case Officer_AI newOfficer:
                    if (_task != null) newOfficer.Task = _task;  // Assign task only if available
                    Debug.Log("Spawning Officer");
                    break;
                case Ranger_AI newRanger:
                    if (_task != null) newRanger.Task = _task;  // Assign task only if available
                    Debug.Log("Spawning Ranger");
                    break;
                default:
                    Debug.Log("Spawning generic Henchman");
                    break;
            }
        }

        _spawnedEnemies.Add(newEnemy);
        _currentSpawned++;
        _totalSpawned++;

        Debug.Log($"Current Spawned: {_currentSpawned}, Total Spawned: {_totalSpawned}");

        if (_maxSpawnAmount < 70)
            _maxSpawnAmount = _currentSpawned + _groupSize;
    }

    private IEnumerator SpawnRoutine()
    {
        Debug.Log("Starting spawn routine...");

        while (_currentSpawned < _maxSpawnAmount)
        {
            if (_spawnGroup)
                SpawnGroup();
            else
                Spawn();

            yield return new WaitForSeconds(_spawnCooldown);
        }

        Debug.Log("Spawn routine completed.");
    }

    private void SpawnGroup()
    {
        for (int i = 0; i < _groupSize; i++)
            Spawn();

        _groupSize++;
    }


    public IEnumerator BreachIncrease()
    {
        _shouldUpgrade = false;
        yield return new WaitForSeconds(_timeToIncreaseBreachSpawn);
        if (_spawnCooldown > 0)
        {
            _spawnCooldown -= 0.5f;
        }
        _shouldUpgrade = true;
    }

    private IEnumerator DisableGO()
    {
        yield return new WaitForSeconds(2.0f);
        gameObject.SetActive(false);
    }

    private void EnemyDied()
    {
        _currentSpawned = Mathf.Max(0, _currentSpawned - 1);
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnRoutine());
    }

    public void ResetSpawners()
    {
        _maxSpawnAmount = _originalMaxSpawnAmount;
        _spawnRadius = _originalSpawnRadius;
        _timeToIncreaseBreachSpawn = _originalTimeToIncreaseSpawn;
        _currentSpawned = _originalCurrentSpawned;
        _totalSpawned = _originalTotalSpawned;
        _groupSize = _originalGroupSize;
        _spawnCooldown = _originalSpawnCooldown;
    }

    private void OnEnemyDeath(EnemyBase enemy)
    {
        EnemyDied();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw a wire sphere to visualize the spawn radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}
