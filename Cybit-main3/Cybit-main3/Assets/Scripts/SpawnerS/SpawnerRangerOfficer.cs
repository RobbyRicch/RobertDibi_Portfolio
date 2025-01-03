using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerRangerOfficer : SpawnerBase
{
    [Header("Officer Spawner Data")]
    [SerializeField] private EnemyBase[] _prefabs;
    [SerializeField] private List<EnemyBase> _spawnedEnemies = new();
    [SerializeField] private TaskBase _task;
    private IEnumerator _spawnRoutine = null;

    private void OnEnable()
    {
        EventManager.OnEnemyDeath += OnEnemyDeath;
    }
    void Start()
    {
        StartSpawning();
    }
    private void Update()
    {
        if (_totalSpawned >= _maxSpawnAmount)
        {
            StopSpawning();
        }
    }
    private void OnDisable()
    {
        EventManager.OnEnemyDeath -= OnEnemyDeath;
    }

    private void SpawnOfficer()
    {
        if (_totalSpawned >= _maxSpawnAmount)
            return;

        Vector3 spawnPosition;
        spawnPosition = transform.position;

        EnemyBase newEnemy = Instantiate(_prefabs[Random.Range(0, _prefabs.Length)], spawnPosition, Quaternion.identity);
        
        if (newEnemy is EnemyHenchman newHenchman)
        {
            newHenchman.PlayerTarget = _playerController.transform;

            switch (newHenchman)
            {
                case Officer_AI newOfficer:
                    newOfficer.Task = _task;
                    break;
                case Ranger_AI newRanger:
                    newRanger.Task = _task;
                    break;
                default:
                    break;
            }
        }

        _spawnedEnemies.Add(newEnemy);
        _totalSpawned++;
    }
    private void SpawnGroup()
    {
        for (int i = 0; i < _groupSize; i++)
            SpawnOfficer();
    }

    private IEnumerator SpawnRoutine()
    {
        if (_task != null && !_task.IsTaskComplete || _isfakeSpawner)
        {
            while (_totalSpawned < _maxSpawnAmount)
            {
                if (_spawnGroup)
                    SpawnGroup();
                else
                    SpawnOfficer();

                yield return new WaitForSeconds(_spawnCooldown);
            }
        }
    }
    public void StartSpawning()
    {
        _spawnRoutine = SpawnRoutine();
        StartCoroutine(_spawnRoutine);
    }
    public void StopSpawning()
    {
        StopCoroutine(_spawnRoutine);
    }

    private void OnEnemyDeath(EnemyBase enemy)
    {
        if (_spawnedEnemies.Contains(enemy))
            _spawnedEnemies.Remove(enemy);
    }
}
