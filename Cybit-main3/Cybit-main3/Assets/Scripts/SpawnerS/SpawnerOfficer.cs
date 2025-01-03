using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnerOfficer : SpawnerBase
{
    [Header("Officer Spawner Data")]
    [SerializeField] private Officer_AI _officerPrefab;
    [SerializeField] private Ranger_AI _rangerPrefab;
    [SerializeField] public List<Officer_AI> _spawnedOfficer = new();
    [SerializeField] public List<Ranger_AI> _spawnedRanger = new();
    [SerializeField] private TaskBase _task;
    private IEnumerator _spawnRoutine = null;

    [Header("Spawn Options")]
    [SerializeField] private bool _spawnBothAtSameTime = false;

    [Header("Spawn Counts")]
    [SerializeField] private int _officerCountPerSpawn = 1;
    [SerializeField] private int _rangerCountPerSpawn = 1;

    private void OnEnable()
    {
        /*        EventManager.OnEnemyDeath += OnEnemyDeath;
        */
    }

    void Start()
    {
        StartSpawning();
        _playerController = SaveManager.Instance.Player;
        _vCam = _playerController.CurrentVirtualCamera;
    }

    private void Update()
    {
        if (_totalSpawned >= _maxSpawnAmount )
        {
            StopSpawning();


        }
    }

    private void OnDisable()
    {
/*        EventManager.OnEnemyDeath -= OnEnemyDeath;
*/    }

    private void SpawnOfficer()
    {
        if (_totalSpawned >= _maxSpawnAmount)
            return;

        Vector3 spawnPosition = transform.position;

        Officer_AI newOfficer = Instantiate(_officerPrefab, spawnPosition, Quaternion.identity);
        newOfficer.PlayerTarget = _playerController.transform;

        _spawnedOfficer.Add(newOfficer);
        _totalSpawned++;
    }

    private void SpawnRanger()
    {
        if (_totalSpawned >= _maxSpawnAmount)
            return;

        Vector3 spawnPosition = transform.position;

        Ranger_AI newRanger = Instantiate(_rangerPrefab, spawnPosition, Quaternion.identity);
        newRanger.PlayerTarget = _playerController.transform;
/*        newRanger.Task = _task;
*/
        _spawnedRanger.Add(newRanger);
        _totalSpawned++;
    }

    private void SpawnBoth()
    {
        SpawnOfficer();
        SpawnRanger();
    }

    private void SpawnGroup()
    {
        for (int i = 0; i < _officerCountPerSpawn; i++)
        {
            if (_totalSpawned < _maxSpawnAmount)
                SpawnOfficer();
        }

        for (int i = 0; i < _rangerCountPerSpawn; i++)
        {
            if (_totalSpawned < _maxSpawnAmount)
                SpawnRanger();
        }
    }

    private IEnumerator SpawnRoutine()
    {
        Player_Controller tempPlayerController = _playerController;

        if (_task != null && !_task.IsTaskComplete || _isfakeSpawner)
        {
            while (_totalSpawned < _maxSpawnAmount)
            {
                if (_spawnGroup || _spawnBothAtSameTime)
                {
                    SpawnGroup();
                }
                else
                {
                    for (int i = 0; i < _officerCountPerSpawn; i++)
                    {
                        if (_totalSpawned < _maxSpawnAmount)
                            SpawnOfficer();
                    }

                    for (int i = 0; i < _rangerCountPerSpawn; i++)
                    {
                        if (_totalSpawned < _maxSpawnAmount)
                            SpawnRanger();
                    }
                }

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
        /*        if (enemy is Officer_AI officer)
                {
                    if (_spawnedOfficer.Contains(officer))
                        _spawnedOfficer.Remove(officer);
                }
                else if (enemy is Ranger_AI ranger)
                {
                    if (_spawnedRanger.Contains(ranger))
                        _spawnedRanger.Remove(ranger);
                }
            }*/

    }
}
