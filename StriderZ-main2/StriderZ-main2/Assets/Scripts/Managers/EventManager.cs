using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static Action OnGameLaunched;
    public static Action OnGameModeLaunched, OnGameModeBegin, OnGameModeLate, OnGameModeResults, OnGameModeConcluded, OnReturnToLobby;

    public static Action<PlayerInputHandler> OnPlayerJoined, OnPlayerPause, OnPlayerDeath, OnPlayerVictory, OnPlayerWin;
    public static Action<PlayerInputHandler[]> OnPlayersReady, OnSpawnPlayers, OnRespawnPlayers;

    public static void InvokeGameLaunched()
    {
        OnGameLaunched?.Invoke();
        Debug.Log("Event: GameLaunched");
    }
    public static void InvokeGameModeLaunched()
    {
        if (PlayerSetupManager.Instance == null || GameManager.Instance == null || PlayerManager.Instance == null)
            return;

        PlayerSetupManager.Instance.PlayerInputManager.DisableJoining();
        GameManager.Instance.ChangeState(GameStates.PreGame);

        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        foreach (PlayerInputHandler player in allPlayers)
        {
            player.IsPlayerInputsDisable = true;
            player.Controller.ResetInputs();

            player.Controller.Rb.velocity = Vector3.zero;
            player.Controller.Rb.angularVelocity = Vector3.zero;

            player.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        OnGameModeLaunched?.Invoke();
        
        Debug.Log("Event: GameModeLaunched");
    }
    public static void InvokeGameModeBegin()
    {
        if (GameManager.Instance == null || PlayerManager.Instance == null)
            return;

        GameManager.Instance.ChangeState(GameStates.MidGame);

        foreach (PlayerInputHandler player in PlayerManager.Instance.AllPlayers)
        {
            player.IsPlayerInputsDisable = false;
            player.Attractor.DisableAttractor(false);
        }

        OnGameModeBegin?.Invoke();
        Debug.Log("Event: GameModeBegin");
    }
    public static void InvokeGameModeLate()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.ChangeState(GameStates.LateGame);
        OnGameModeLate?.Invoke();
        Debug.Log("Event: GameModeLate");
    }
    public static void InvokeGameModeResults()
    {
        if (GameManager.Instance == null)
            return;

        foreach (PlayerInputHandler player in PlayerManager.Instance.AllPlayers)
        {
            player.Attractor.ReturnLeftAttractor();
            player.Attractor.ReturnRightAttractor();
            player.Attractor.CancelAttractorLeft(true);
            player.Attractor.CancelAttractorLeft(true);
            player.Attractor.DisableAttractor(true);
            player.Controller.Rb.angularVelocity = Vector3.zero;
            player.Controller.Rb.velocity = Vector3.zero;
            player.transform.position = player.transform.position;
        }

        GameManager.Instance.ChangeState(GameStates.Results);
        OnGameModeResults?.Invoke();
        Debug.Log("Event: GameModeResults");
    }
    public static void InvokeGameModeConcluded()
    {
        int playersCount = PlayerManager.Instance.AllPlayers.Count;
        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerInputHandler player = allPlayers[i];
            player.Attractor.ReturnLeftAttractor();
            player.Attractor.ReturnRightAttractor();
            player.Attractor.CancelAttractorLeft(true);
            player.Attractor.CancelAttractorLeft(true);
            player.Attractor.DisableAttractor(true);
            player.Controller.Rb.angularVelocity = Vector3.zero;
            player.Controller.Rb.velocity = Vector3.zero;
            player.transform.position = player.transform.position;
        }

        OnGameModeConcluded?.Invoke();
        Debug.Log("Event: GameModeConcluded");
    }
    public static void InvokeReturnToLobby()
    {
        if (PlayerSetupManager.Instance == null || GameManager.Instance == null || PlayerManager.Instance == null)
            return;

        //PlayerSetupManager.Instance.PlayerInputManager.enabled = true;
        GameManager.Instance.CurrentGameMode = GameModeType.Lobby;
        GameManager.Instance.ChangeState(GameStates.Preperations);
        PlayerSetupManager.Instance.PlayerInputManager.EnableJoining();

        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerInputHandler player = allPlayers[i];
            player.transform.position = PlayerSetupManager.Instance.AllPlayersSpawnPositions[i];
            player.Controller.Rb.angularVelocity = Vector3.zero;
            player.Controller.Rb.velocity = Vector3.zero;
            player.Attractor.CancelAttractorLeft(true);
            player.Attractor.CancelAttractorLeft(true);
            player.Attractor.ReturnLeftAttractor();
            player.Attractor.ReturnRightAttractor();

            if (!player.Controller.IsAlive || player.Controller.IsDead)
                player.Controller.Revive();

            player.transform.position = player.transform.position;
            player.Controller.Rb.useGravity = true;
            player.Controller.Rb.isKinematic = false;
            player.IsPlayerInputsDisable = false;
            player.Data.Score = 0;
        }

        PlayerManager.Instance.AllPlayersAlive.Sort(PlayerManager.Instance.CompareByID);
        OnReturnToLobby?.Invoke();
        Debug.Log("Event: GameModeLaunched");
    }

    public static void InvokePlayerJoined(PlayerInputHandler player)
    {
        if (player != null && player.gameObject.activeInHierarchy && player.Controller.IsAlive)
        {
            OnPlayerJoined?.Invoke(player);
            Debug.Log($"Event: PlayerPause, Source: player {player.SetupData.ID}");
        }
    }
    public static void InvokePlayerPause(PlayerInputHandler player)
    {
        if (player != null && player.gameObject.activeInHierarchy && player.Controller.IsAlive)
        {
            OnPlayerPause?.Invoke(player);
            Debug.Log($"Event: PlayerPause, Source: player {player.SetupData.ID}");
        }
    }
    public static void InvokePlayerDeath(PlayerInputHandler player)
    {
        if (player != null && player.gameObject.activeInHierarchy)
        {
            OnPlayerDeath?.Invoke(player);
            Debug.Log($"Invoked PlayerDeath");
        }
    }
    public static void InvokePlayerVictory(PlayerInputHandler player)
    {
        if (player != null && player.gameObject.activeInHierarchy)
        {
            OnPlayerVictory?.Invoke(player);
        }
    }
    public static void InvokePlayerWin(PlayerInputHandler player)
    {
        if (player != null && player.gameObject.activeInHierarchy)
        {
            OnPlayerWin?.Invoke(player);
            Debug.Log($"Event: PlayerWin, Source: player {player.SetupData.ID}");
        }
    }

    public static void InvokePlayersReady(PlayerInputHandler[] players)
    {
        if (players != null)
        {
            OnPlayersReady?.Invoke(players);
            Debug.Log($"Event: PlayersReady");
        }
    }
    public static void InvokeSpawnPlayers(PlayerInputHandler[] players)
    {
        if (players != null)
        {
            OnSpawnPlayers?.Invoke(players);
            Debug.Log($"Event: SpawnPlayers");
        }
    }
    public static void InvokeRespawnPlayers(PlayerInputHandler[] players)
    {
        if (players != null)
        {
            OnRespawnPlayers?.Invoke(players);
            Debug.Log($"Event: RespawnPlayers");
        }
    }

    public static void InvokeRoundEnd(PlayerInputHandler player)
    {
        if (player != null && player.gameObject.activeInHierarchy)
        {
            //OnRoundEnd?.Invoke(player);
            Debug.Log($"RoundEnd Invoked by {player.Data.Nickname}");
        }
        else if (player == null)
        {
            //OnRoundEnd?.Invoke(PlayerManager.Instance.LastPlayerWhoDied);
            Debug.Log($"RoundEnd Invoked by {player.Data.Nickname}");
        }
    } // example for handling draw case
}
