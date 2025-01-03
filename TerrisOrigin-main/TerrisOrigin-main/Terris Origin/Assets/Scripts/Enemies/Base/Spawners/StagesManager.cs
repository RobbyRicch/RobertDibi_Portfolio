using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagesManager : MonoBehaviour
{
    [SerializeField] private int _stageIndex;
    private bool _stageOver;
    private bool _finishedAllStages;
    [SerializeField] private float _stageStartDelay = 5;
    private float _stageStartDelayTimer;
    private WavesManager[] _wavesManagers;
    private Portal[] _portals;

    private void Awake()
    {
        _wavesManagers = GetComponentsInChildren<WavesManager>();
        _portals = GetComponentsInChildren<Portal>();
    }

    // Update is called once per frame
    void Update()
    {
        StageEndChecker();
    }


    private void StageEndChecker()
    {
        if (!_finishedAllStages)
        {
            if (_stageIndex <= 0)
            {
                if (!_stageOver)
                {
                    _stageStartDelayTimer = 0;
                }
                _stageOver = true;
            }
            else if (_stageIndex > 0)
            {
                if (_wavesManagers[_stageIndex - 1].StageCompleted)
                {
                    if (!_stageOver)
                    {
                        _stageStartDelayTimer = 0;
                        if (_portals.Length > 0 && _stageIndex <= _portals.Length) _portals[_stageIndex - 1].Activate();
                    }
                    _stageOver = true;
                }
            }
            else
            {
                _stageOver = false;
                _stageStartDelayTimer = 0;
            }

            if (_stageOver)
            {
                if (_stageStartDelayTimer >= _stageStartDelay)
                {
                    if (_stageIndex <= 0)
                    {
                        _stageStartDelayTimer = 0;
                        _stageOver = false;
                        if (_stageIndex < _wavesManagers.Length)
                        {
                            NextStage();
                        }
                        else
                        {
                            _finishedAllStages = true;
                        }
                    }
                    else if (_portals.Length > 0 && _stageIndex <= _portals.Length)
                    {
                        if (_portals[_stageIndex - 1].Teleported)
                        {
                            _stageStartDelayTimer = 0;
                            _stageOver = false;
                            if (_stageIndex < _wavesManagers.Length)
                            {
                                NextStage();
                            }
                            else
                            {
                                _finishedAllStages = true;
                            }
                        }
                    }
                    else
                    {
                        _stageStartDelayTimer = 0;
                        _stageOver = false;
                        if (_stageIndex < _wavesManagers.Length)
                        {
                            NextStage();
                        }
                        else
                        {
                            _finishedAllStages = true;
                        }
                    }
                }
                else
                {
                    _stageStartDelayTimer += Time.deltaTime;
                }
            }
        }
    }

    [ContextMenu("Initiate Stage")]
    private void InitiateStage()
    {
        for (int i = 0; i < _wavesManagers.Length; i++)
        {
            if (i == _stageIndex - 1 && !_wavesManagers[i].StageCompleted)
            {
                _wavesManagers[i].CanSpawn = true;
            }
            else
            {
                _wavesManagers[i].CanSpawn = false;
            }
        }
    }

    [ContextMenu("Next Stage")]
    private void NextStage()
    {
        _stageIndex++;
        InitiateStage();
    }
}
