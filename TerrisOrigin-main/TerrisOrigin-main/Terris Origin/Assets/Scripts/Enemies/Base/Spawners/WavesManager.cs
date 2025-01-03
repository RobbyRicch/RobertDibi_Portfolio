using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [SerializeField] private bool _canSpawn;
    [SerializeField] private int _waveIndex;
    private int _enemiesSpawned;
    private int _enemiesLeft;
    private bool _waveOver;
    private bool _stageCompleted;
    [SerializeField] private float _roundStartDelay = 5;
    private float _roundStartDelayTimer;
    private WaveHolder[] _wavesHolder;

    public bool CanSpawn { get => _canSpawn; set => _canSpawn = value; }
    public bool StageCompleted { get => _stageCompleted;}

    private void Awake()
    {
        _wavesHolder = GetComponentsInChildren<WaveHolder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var wave in _wavesHolder)
        {
            wave.PrepareWave();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (_canSpawn)
        {
            foreach (var wave in _wavesHolder)
            {
                if (wave.WaveActive)
                {
                    _enemiesSpawned = wave.TotalEnemyCount;
                    _enemiesLeft = wave.ActiveEnemyCount;
                }
            }

            WaveEndChecker();
        }

    }


    private void WaveEndChecker()
    {
        if (_enemiesLeft <= 0)
        {
            if (!_waveOver)
            {
                _roundStartDelayTimer = 0;
            }
            _waveOver = true;
            _enemiesLeft = 0;
            _enemiesSpawned = 0;
        }
        else
        {
            _waveOver = false;
            _roundStartDelayTimer = 0;
        }

        if (_waveOver)
        {
            if (_roundStartDelayTimer >= _roundStartDelay)
            {
                _roundStartDelayTimer = 0;
                _waveOver = false;
                if (_waveIndex < _wavesHolder.Length)
                {
                    NextWave();
                }
                else
                {
                    _stageCompleted = true;
                    _canSpawn = false;
                }
            }
            else
            {
                _roundStartDelayTimer += Time.deltaTime;
            }
        }
    }

    [ContextMenu("Initiate Wave")]
    private void InitiateWave()
    {
        if (_canSpawn)
        {
            for (int i = 0; i < _wavesHolder.Length; i++)
            {
                if (i == _waveIndex - 1)
                {
                    _wavesHolder[i].gameObject.SetActive(true);
                    _wavesHolder[i].WaveActive = true;
                }
                else
                {
                    _wavesHolder[i].gameObject.SetActive(false);
                    _wavesHolder[i].WaveActive = false;
                }
            }
        }
    }

    [ContextMenu("Next Wave")]
    private void NextWave()
    {
        _waveIndex++;
        InitiateWave();
    }
}
