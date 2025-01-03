using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaserRushLateGameType
{
    GroundFall,
    MeteorShower,
    FloorIsLava
}

public class ForkManager : MonoBehaviour
{
    //private static ForkManager _instance;
    //public static ForkManager Instance => _instance;

    [SerializeField] private LaserRushLateGameType _activeArenaEventType;
    [SerializeField] private int _winPointsOnPass = 250;
    [SerializeField] private ArenaEventTrigger[] _arenaGates;

    private PlayerInputHandler _firstPlayerToPass = null;
    public PlayerInputHandler FirstPlayerToPass => _firstPlayerToPass;
    public LaserRushLateGameType ActiveArenaEventType => _activeArenaEventType;

    /*private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }*/

    private void DisableNoneActiveGates()
    {
        for (int i = 0; i < _arenaGates.Length; i++)
        {
            if (_arenaGates[i].IsPlayerPassed)
                continue;

            _arenaGates[i].gameObject.SetActive(false);
        }
    }

    public void TriggerGameModeLate(PlayerInputHandler player, LaserRushLateGameType arenaEventType, ForkManager forkManager)
    {
        if (player == null)
            return;

        if (ArenaScrumbler.Instance == null)
            return;

        ArenaScrumbler.Instance.ForkManager = forkManager;
        _firstPlayerToPass = player;
        _activeArenaEventType = arenaEventType;

        player.Data.Score += _winPointsOnPass;
        DisableNoneActiveGates();
        EventManager.InvokeGameModeLate();
    }
}
