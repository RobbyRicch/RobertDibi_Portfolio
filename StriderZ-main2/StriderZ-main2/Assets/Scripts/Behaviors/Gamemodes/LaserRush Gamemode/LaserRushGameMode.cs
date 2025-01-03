using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRushGameMode : AbstractGameMode
{
    private static LaserRushGameMode _instance;
    public static LaserRushGameMode Instance => _instance;

    #region LaserRush Components
    [SerializeField] private LaserRushUIHandler _laserRushUIHandler;
    public LaserRushUIHandler LaserRushUIHandler => _laserRushUIHandler;
    #endregion

    #region Dynamic Components
    private List<PlayerInputHandler> _playerPlacements;
    public List<PlayerInputHandler> PlayerPlacement => _playerPlacements;

    private List<PlayerInputHandler> _allPlayerToRespawnInArena = new();

    private Transform[] _allPlayersArenaSpawns;
    public Transform[] AllPlayersArenaSpawns { get => _allPlayersArenaSpawns; set => _allPlayersArenaSpawns = value; }

    private Transform _endPoint = null;
    public Transform EndPoint { get => _endPoint; set => _endPoint = value; }
    
    private IEnumerator _invokeVictoryWihDelay = null;
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool _isDebugMessagesOn = false;
    #endregion

    #region Deathline Data
    [Header("Deathline")]
    [SerializeField] private Transform _deathlineColliderTr;
    public Transform DeathlineColliderTr => _deathlineColliderTr;

    [SerializeField] private DeathlineCollider _deathlineCollider;
    public DeathlineCollider DeathlineCollider=> _deathlineCollider;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _playerPlacements = new List<PlayerInputHandler>();
    }
    private void OnEnable()
    {
        EventManager.OnGameModeLaunched += OnGameModeLaunched;
        EventManager.OnGameModeLaunched += OnGameModeLaunchedLaserRush;
        EventManager.OnSpawnPlayers += OnSpawnPlayers;
        EventManager.OnGameModeBegin += OnGameModeBegin;
        EventManager.OnGameModeBegin += OnGameModeBeginLaserRush;
        EventManager.OnPlayerDeath += OnPlayerDeath;
        EventManager.OnPlayerVictory += OnPlayerVictory;
        EventManager.OnGameModeLate += OnGameModeLate;
        EventManager.OnGameModeConcluded += OnGameModeConcluded;
    }
    private void Start()
    {
        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        foreach (PlayerInputHandler player in allPlayers)
        {
            player.Data.ModelData.BodyMesh.transform.parent.rotation = Quaternion.identity;
            player.Controller.CrosshairParent.rotation = Quaternion.identity;
            player.transform.position = _playerSpawns[player.SetupData.ID].position;
        }

        EventManager.InvokeGameModeLaunched();
    }
    private void Update()
    {
        SetPlayerPlacements();
    }
    private void OnDisable()
    {
        EventManager.OnGameModeLaunched -= OnGameModeLaunched;
        EventManager.OnSpawnPlayers -= OnSpawnPlayers;
        EventManager.OnGameModeBegin -= OnGameModeBegin;
        EventManager.OnGameModeBegin -= OnGameModeBeginLaserRush;
        EventManager.OnPlayerDeath -= OnPlayerDeath;
        EventManager.OnPlayerVictory -= OnPlayerVictory;
        EventManager.OnGameModeLate -= OnGameModeLate;
        EventManager.OnGameModeConcluded -= OnGameModeConcluded;
    }
    #endregion

    #region Race Related
    private void SetPlayerPlacements()
    {
        if (PlayerSetupManager.Instance == null || PlayerManager.Instance == null)
            return;

        if (PlayerSetupManager.Instance.AllPlayersSetupData.Count == 1) // one player fix
        {
            _playerPlacements = PlayerManager.Instance.AllPlayersAlive;
            return;
        }

        _playerPlacements = PlayerManager.Instance.AllPlayersAlive;
        _playerPlacements.Sort((x, y) => x.Controller.CalculatePath().CompareTo(y.Controller.CalculatePath()));
        string placements = "Placements: ";
        for (int i = 0; i < _playerPlacements.Count; i++)
        {
            placements += $", {i}. Player {_playerPlacements[i].GetComponent<PlayerData>().ID}";
        }
        Debug.Log(placements);
    }
    #endregion

    #region Victory & Conclusions
    private IEnumerator InvokeVictoryWithDelay(PlayerInputHandler player)
    {
        yield return new WaitForSeconds(_victoryDelay);

        player.Data.Score += 500;
        CinemachineManager.Instance.ActivateWinRoundCamera(player, player.Data.WinCamTr);
        yield return new WaitForSeconds(_resultsDelay);

        EventManager.InvokeGameModeResults();
        _invokeVictoryWihDelay = null;

        yield return new WaitUntil(() => ScoreManager.Instance.IsScoreboardDone);

        // new victory sequence
        // reload scene
        yield return new WaitForSeconds(_winDelay);

        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (allPlayers[i].Data.Score < _maxScore)
                continue;

            EventManager.InvokePlayerWin(allPlayers[i]);
            UIManager.Instance.ScoreboardPanel.SetActive(false);
            yield break;
        }

        UIManager.Instance.ScoreboardPanel.SetActive(false);
        UIManager.Instance.FadeIn(0.25f);
        yield return new WaitForSeconds(0.25f);

        CustomSceneManager.ReloadScene();
        Debug.Log($"Event: PlayerVictory, Source: player {player.SetupData.ID}");
    }
    #endregion

    #region Events
    private void OnGameModeLaunchedLaserRush()
    {
        _deathlineCollider.IsMoving = true;
    }
    private void OnGameModeBeginLaserRush()
    {
        _deathlineCollider.IsMoving = true;
        CinemachineManager.Instance.TargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            CinemachineManager.Instance.TargetGroup.AddMember(allPlayers[i].Data.TrackCamTr, CinemachineManager.Instance.TargetWeight, CinemachineManager.Instance.TargetRadius);
        }
    }
    private void OnGameModeLate()
    {
        CinemachineManager.Instance.ActivateArenaVirtualCamera();

        DeathlineCollider deathline = _deathlineColliderTr.GetComponent<DeathlineCollider>();
        deathline.IsMoving = false;
        deathline.gameObject.SetActive(false);
    }
    private void OnPlayerDeath(PlayerInputHandler player)
    {
        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;

        // handle stand alone death
        if (allPlayersAlive.Count == 1 && allPlayers.Count == allPlayersAlive.Count)
        {
            Vector3 newPos = player.Controller.PlayerLastObstaclePos;
            newPos.y = 8.0f;
            player.transform.position = newPos;
            player.Controller.IsDead = false;
            return;
        }

        CinemachineManager.Instance.TargetGroup.RemoveMember(player.Data.TrackCamTr);
        allPlayersAlive.Remove(player);
        allPlayersAlive.Sort(PlayerManager.Instance.CompareByID);
        player.transform.position = _deathArea.position;

        if (allPlayersAlive.Count == 1)
        {
            PlayerInputHandler lastPlayerAlive = PlayerManager.Instance.IsOnePlayerLeft();
            PlayerManager.Instance.LastPlayerAlive = lastPlayerAlive;
            EventManager.InvokePlayerVictory(lastPlayerAlive);
        }
    }
    private void OnPlayerVictory(PlayerInputHandler player)
    {
        DeathlineCollider deathline = _deathlineColliderTr.GetComponent<DeathlineCollider>();
        deathline.IsMoving = false;
        deathline.gameObject.SetActive(false);

        _invokeVictoryWihDelay = InvokeVictoryWithDelay(player);
        StartCoroutine(_invokeVictoryWihDelay);
    }
    #endregion
}
