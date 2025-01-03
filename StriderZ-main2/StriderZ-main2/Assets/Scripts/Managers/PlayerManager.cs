using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance => _instance;

    private PlayerInputHandler _lastPlayerAlive;
    public PlayerInputHandler LastPlayerAlive { get => _lastPlayerAlive; set => _lastPlayerAlive = value; }

    private PlayerInputHandler _lastPlayerWhoDied;
    public PlayerInputHandler LastPlayerWhoDied { get => _lastPlayerWhoDied; set => _lastPlayerWhoDied = value; }

    [SerializeField] private List<PlayerInputHandler> _allPlayers;
    public List<PlayerInputHandler> AllPlayers => _allPlayers;

    [SerializeField] private List<PlayerInputHandler> _allPlayersAlive;
    public List<PlayerInputHandler> AllPlayersAlive => _allPlayersAlive;

    [SerializeField] private string _playerTag = "Player";
    public string PlayerTag => _playerTag;

    [SerializeField] private bool _isDebugMessagesOn;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _allPlayers = new List<PlayerInputHandler>();
        _allPlayersAlive = new List<PlayerInputHandler>();
    }



    public bool CheckIfAllPlayersAreReady()
    {
        if (PlayerSetupManager.Instance.AllPlayersSetupData.Count == 1) // one player fix
        {
            if (_allPlayers[0] && _allPlayers[0].Controller.IsReady)
                return true;
        }
            
        int numOfReadyPlayers = 0;
        for (int i = 0; i < _allPlayers.Count; i++)
        {
            if (_allPlayers[i] && _allPlayers[i].Controller.IsReady)
            {
                numOfReadyPlayers++;
                continue;
            }
            else
                break;
        }

        if (numOfReadyPlayers != 0 && numOfReadyPlayers == _allPlayers.Count)
            return true;
        else
            return false;
    }
    public int CompareByID(PlayerInputHandler firstPlayer, PlayerInputHandler secondPlayer)
    {
        return firstPlayer.Data.ID.CompareTo(secondPlayer.Data.ID);
    }

    public PlayerInputHandler IsOnePlayerLeft()
    {
        int playersAliveCounter = 0;
        PlayerInputHandler lastPlayerAlive = null;

        if (_allPlayers.Count == 1)
        {
            PlayerInputHandler playerInputHandler = _allPlayers[0];
            PlayerController playerController = playerInputHandler.Controller;

            if (playerController.IsAlive)
                lastPlayerAlive = playerInputHandler;

            return lastPlayerAlive;
        }

        foreach (PlayerInputHandler playerInputHandler in _allPlayers)
        {
            PlayerController playerController = playerInputHandler.Controller;
            playersAliveCounter = playerController.IsAlive == true ? playersAliveCounter + 1 : playersAliveCounter;

            if (playerController.IsAlive)
                lastPlayerAlive = playerInputHandler;
        }

        bool isOnlyOnePlayerAlive = playersAliveCounter == 1 ? true : false;

        if (_isDebugMessagesOn)
        {
            Debug.Log(playersAliveCounter);
            Debug.Log(isOnlyOnePlayerAlive);
        }

        if (isOnlyOnePlayerAlive)
        {
            return lastPlayerAlive;
        }
        else
        {
            return null;
        }
    }
    public PlayerInputHandler PlayersDiedByDraw()
    {
        if (_lastPlayerAlive)
            return _lastPlayerAlive;
        else
            return null;
    }

    #region Events
    #endregion
}
