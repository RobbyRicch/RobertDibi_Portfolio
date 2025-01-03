using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBase : MonoBehaviour
{
    [Header("Player Reference")]
    [SerializeField] protected Player_Controller _playerController;
    public Player_Controller PlayerController {get => _playerController; set => _playerController = value; }

    [SerializeField] protected CinemachineVirtualCamera _vCam;
    public CinemachineVirtualCamera VCam { get => _vCam; set => _vCam = value; }

    [Header("Basic Spawner Data")]
    [SerializeField] protected int _maxSpawnAmount = 5;
    [SerializeField] protected int _currentSpawned = 0;
    [SerializeField] protected int _totalSpawned;
    [SerializeField] protected int _groupSize = 3;
    [SerializeField] protected float _spawnCooldown = 1.0f;
    [SerializeField] protected bool _spawnGroup = true;
    [SerializeField] protected bool _shouldUpgrade = false;
    [SerializeField] protected bool _isfakeSpawner = false;
}
