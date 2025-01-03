using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineExample : MonoBehaviour
{
    private bool _isIdle;
    private bool _isAimingAt;
    private bool _isSearchingPlayer;
    private bool _isShootingPlayer;
    private bool _hasLostPlayer;

    private delegate void JackalStateMachine();
    private JackalStateMachine _state;

    private void Start()
    {
        _state = IdleLogic;
    }
    private void Update()
    {
        _state.Invoke();

        /*if (_isIdle)
        {
            IdleLogic();
        }
        else if (_isWatchingPlayer)
        {
            WatchingPlayerLogic();
        }
        else if (_isSearchingPlayer)
        {
            SearchingPlayerLogic();
        }
        else if (_isShootingPlayer)
        {
            ShootingPlayerLogic();
        }
        else if (_hasLostPlayer)
        {
            LostPlayerLogic();
        }*/
    }

    #region Logic
    private void IdleLogic()
    {

    }
    private void WatchingPlayerLogic()
    {

    }
    private void SearchingPlayerLogic()
    {

    }
    private void ShootingPlayerLogic()
    {

    }
    private void LostPlayerLogic()
    {

    }
    #endregion

    private void SwitchIdleToAimingAtPlayer()
    {
        if (_state.Method.Name == nameof(IdleLogic))
        {
            _state = WatchingPlayerLogic;
        }
    }
}
