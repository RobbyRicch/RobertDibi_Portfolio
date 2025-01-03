using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using Cinemachine;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager _instance;
    public static LobbyManager Instance => _instance;

    #region Login
    [Header("Login")]
    [SerializeField] private Image[] _playerJoinPanels;
    [SerializeField] private Image[] _playerIcons;

    [SerializeField] private TextMeshProUGUI[] _joinTexts, _nickTexts, _readyTexts;
    public TextMeshProUGUI[] ReadyTexts => _readyTexts;
    #endregion

    #region Camera
    [Header("Camera")]
    [SerializeField] private Camera _mainCam;
    [SerializeField] private CinemachineVirtualCamera _virtualCam;
    #endregion

    #region Spawn
    [Header("Spawn")]
    [SerializeField] private Transform[] _allPlayersSpawns;
    public Transform[] AllPlayersSpawns => _allPlayersSpawns;
    #endregion

    #region Dressing Room Related
    private List<PlayerInputHandler> _allChangingPlayers;
    public List<PlayerInputHandler> AllChangingPlayers { get => _allChangingPlayers; set => _allChangingPlayers = value; }
    #endregion

    [Space][SerializeField] private bool _isFirstTimeInLobby = true;
    public bool IsFirstTimeInLobby => _isFirstTimeInLobby;


    [Space][SerializeField] private MoveToGameMode _moveToGameModeScript;

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _allChangingPlayers = new List<PlayerInputHandler>();
    }
    private void Start()
    {
        int setupDataCount = PlayerManager.Instance.AllPlayers.Count;
        if (setupDataCount > 0)
        {
            for (int i = 0; i < setupDataCount; i++)
            {
                OnPlayerJoined(PlayerManager.Instance.AllPlayers[i]);
            }
        }
    }
    private void OnEnable()
    {
        EventManager.OnGameLaunched += OnGameLaunched;
        EventManager.OnPlayerJoined += OnPlayerJoined;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        EventManager.OnGameLaunched -= OnGameLaunched;
        EventManager.OnPlayerJoined -= OnPlayerJoined;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    #endregion

    #region Player Initialization
    private void ApplyColorsToUIElements(PlayerSetupData playerSetupData, ColorData colorData)
    {
        Color color = colorData.EmissionEmissionColor;
        color.a = 0.5f;
        _playerJoinPanels[playerSetupData.ID].color = color;
    }
    private void CycleColorsOnNewPlayer(PlayerSetupData playerSetupData)
    {
        /* apply to player setup data */
        PlayerSetupManager.Instance.CycleNextColor(playerSetupData);
        ColorData colorData = playerSetupData.ColorData;

        ApplyColorsToUIElements(playerSetupData, colorData);
    }
    private void CycleModelsOnNewPlayer(PlayerSetupData playerSetupData, bool isInitialized)
    {
        /* apply to player setup data */
        PlayerSetupManager.Instance.CycleNextModel(playerSetupData, isInitialized);
    }
    private void SetModelAndColorOnPlayer(PlayerSetupData playerSetupData)
    {
        PlayerInputHandler player = PlayerManager.Instance.AllPlayersAlive[playerSetupData.ID];
        player.Controller.SetModelAndColor();
        _playerIcons[playerSetupData.ID].sprite = player.Data.ModelData.IconImage.sprite;
        //_playerIcons[playerSetupData.ID].color = playerSetupData.ColorData.IconColor;
    }
    private void InitializePlayerSetupData(PlayerSetupData playerSetupData, bool isInitialized)
    {
        CycleColorsOnNewPlayer(playerSetupData);
        CycleModelsOnNewPlayer(playerSetupData, isInitialized);
    }
    public void InitializeNewPlayerAfterSetup(PlayerSetupData playerSetupData)
    {
        playerSetupData.ColorData ??= PlayerSetupManager.Instance.AllColors[0]; // "??=" equals to "is null" equals to "ReferenceEquals(x, null)"
        InitializePlayerSetupData(playerSetupData, false);
    }
    #endregion

    #region Lobby Behaviours
    public void CycleColorsOnPlayer(PlayerSetupData playerSetupData)
    {
        /* apply to player setup data */
        PlayerSetupManager.Instance.CycleNextColor(playerSetupData);
        ColorData colorData = playerSetupData.ColorData;

        ApplyColorsToUIElements(playerSetupData, colorData);
        SetModelAndColorOnPlayer(playerSetupData);
    }
    public void CycleModelsOnPlayer(PlayerSetupData playerSetupData, bool isInitialized)
    {
        /* apply to player setup data */
        PlayerSetupManager.Instance.CycleNextModel(playerSetupData, isInitialized);
        SetModelAndColorOnPlayer(playerSetupData);
    }
    #endregion

    #region Unity Events
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (SceneManager.GetActiveScene().buildIndex == arg0.buildIndex)
        {
            CinemachineManager.Instance.SetupNewScene(_mainCam, _virtualCam);
            CinemachineManager.Instance.TargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];

            List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
            if (allPlayers.Count > 0)
            {
                PlayerManager.Instance.AllPlayersAlive.Sort(PlayerManager.Instance.CompareByID);
                for (int i = 0; i < allPlayers.Count; i++)
                {
                    PlayerInputHandler player = allPlayers[i];

                    CinemachineManager.Instance.TargetGroup.AddMember(player.transform, CinemachineManager.Instance.TargetWeight, CinemachineManager.Instance.TargetRadius);
                }
            }

            _virtualCam.Follow = CinemachineManager.Instance.TargetGroup.transform;
            EventManager.InvokeGameLaunched();
        }
    }

    private void OnGameLaunched()
    {
        if (SceneManager.GetActiveScene().name == "LobbyToReturn")
            UIManager.Instance.FadeOut(0.5f);

        _isFirstTimeInLobby = false;
    }
    private void OnPlayerJoined(PlayerInputHandler player)
    {
        int maxPlayers = _playerIcons.Length;
        int playerID = player.SetupData.ID;

        _playerIcons[playerID].sprite = player.Data.ModelData.IconImage.sprite;
        UIManager.Instance.ShowUIObject(_playerIcons[playerID].transform, new(0.8f, 0.8f, 0.8f)); // right icon size

        int correctPlayerIndex = playerID + 1;
        _joinTexts[playerID].gameObject.SetActive(false);
        _nickTexts[playerID].text = "Player " + correctPlayerIndex;
        UIManager.Instance.ShowUIObject(_nickTexts[playerID].transform, Vector3.one);

        Color color = player.SetupData.ColorData.EmissionEmissionColor;
        color.a = 0.5f;
        _playerJoinPanels[playerID].color = color;
        UIManager.Instance.ShowUIObject(_readyTexts[playerID].transform, Vector3.one);

        IdleUIDance idleDancer = _playerJoinPanels[playerID].gameObject.GetComponent<IdleUIDance>();
        idleDancer.StopAllCoroutines();
        Destroy(idleDancer);

        _moveToGameModeScript.StopTimer();

        if (playerID < maxPlayers - 1)
        {
            _playerJoinPanels[playerID +1].gameObject.SetActive(true);
            _playerJoinPanels[playerID +1].gameObject.AddComponent<IdleUIDance>();
        }
    }
    #endregion
}
