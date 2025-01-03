using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSetupManager : MonoBehaviour
{
    private static PlayerSetupManager _instance;
    public static PlayerSetupManager Instance => _instance;

    [SerializeField] private int _maxPlayers = 4;
    public int MaxPlayers => _maxPlayers;

    [SerializeField] private PlayerInputManager _playerInputManager;
    public PlayerInputManager PlayerInputManager => _playerInputManager;

    [SerializeField] private GameObject _playerPrefab;
    public GameObject PlayerPrefab => _playerPrefab;

    [SerializeField] private List<ColorData> _allColors;
    public List<ColorData> AllColors => _allColors;

    [SerializeField] private Sprite[] _allHelmetSprites;
    public Sprite[] AllHelmetSprites => _allHelmetSprites;

    private Vector3[] _allPlayersSpawnPositions;
    public Vector3[] AllPlayersSpawnPositions => _allPlayersSpawnPositions;

    private List<PlayerSetupData> _allPlayersSetupData;
    public List<PlayerSetupData> AllPlayersSetupData => _allPlayersSetupData;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        _allPlayersSetupData = new List<PlayerSetupData>();
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        Transform[] allPlayerSpawns = LobbyManager.Instance.AllPlayersSpawns;
        _allPlayersSpawnPositions = new Vector3[allPlayerSpawns.Length];
        for (int i = 0; i < _allPlayersSpawnPositions.Length; i++)
        {
            _allPlayersSpawnPositions[i] = allPlayerSpawns[i].position;
        }
    }

    /*  when player is instantiated they choose a model by their id and take one color from the list */

    public void CycleNextModel(PlayerSetupData playerSetupData, bool isInitialized) // in playerController's Start - switch by enum to activate the right model
    {
        if (isInitialized)
        {
            int modelIndex = (int)playerSetupData.ChosenModelType;
            modelIndex++;

            if (modelIndex == Enum.GetValues(typeof(ModelType)).Length)
                modelIndex = 0;

            playerSetupData.ChosenModelType = (ModelType)modelIndex;
            //playerSetupData.HelmetSprite = _allHelmetSprites[modelIndex]; // return after the is new place for helmet icons
        }
        else
        {
            playerSetupData.ChosenModelType = (ModelType)playerSetupData.ID;
            //playerSetupData.HelmetSprite = _allHelmetSprites[playerSetupData.ID]; // return after the is new place for helmet icons
        }
    }
    public void CycleNextColor(PlayerSetupData playerSetupData) // in playerController's Start - initialize colors with modelData
    {
        if (playerSetupData.IsColored)
            _allColors.Add(playerSetupData.ColorData);

        playerSetupData.ColorData = _allColors[0];
        _allColors.RemoveAt(0);
        playerSetupData.IsColored = true;
    }
    public void ReadyUp(int playerIndex)
    {
        _allPlayersSetupData[playerIndex].IsSetupDone = true;

        /*_allPlayersConfig.Count == _maxPlayers &&*/
        if (_allPlayersSetupData.All(playerSetup => playerSetup.IsSetupDone))
        {
            for (int i = 0; i < _allPlayersSetupData.Count; i++)
                _allPlayersSetupData[i].IsSetupDone = false;

            if (CustomSceneManager.IsFirstTimeInMenu)
                CustomSceneManager.IsFirstTimeInMenu = false;

            /*UIManager.Instance.CharacterCustomizationLayout.transform.parent.gameObject.SetActive(false);
            UIManager.Instance.GameModsPanel.SetActive(true);
            UIManager.Instance.CustomizationCam.SetActive(true); // ask robby
            UIManager.Instance.HiddenButton.onClick.Invoke();*/
        }
    }
    public void UnReady(int playerIndex)
    {
        _allPlayersSetupData[playerIndex].IsSetupDone = false;
    }

    #region Unity Events
    public void PlayerJoin(PlayerInput playerInput)
    {
        Debug.Log($"Player {playerInput.playerIndex} has joined!");

        if (!_allPlayersSetupData.Any(playerSetupData => playerSetupData.ID == playerInput.playerIndex))
        {
            playerInput.transform.SetParent(transform);

            PlayerSetupData playerSetupData = new(playerInput);
            _allPlayersSetupData.Add(playerSetupData);

            Transform playerSpawn = LobbyManager.Instance.AllPlayersSpawns[playerInput.playerIndex];
            GameObject playerGameObject = Instantiate(_playerPrefab, playerSpawn.position, playerSpawn.rotation);

            PlayerInputHandler player = playerGameObject.GetComponent<PlayerInputHandler>();
            player.Controller.IsInLobby = true;
            player.Initialize(_allPlayersSetupData[playerInput.playerIndex]);
            LobbyManager.Instance.InitializeNewPlayerAfterSetup(player.SetupData);
            CinemachineManager.Instance.TargetGroup.AddMember(player.transform, CinemachineManager.Instance.TargetWeight, CinemachineManager.Instance.TargetRadius);

            List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
            List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;

            if (_allPlayersSetupData.Count == 1)
                player.Controller.SetSoloPlayer(true);
            else if (_allPlayersSetupData.Count == 2)
                allPlayers[0].Controller.SetSoloPlayer(false);

            allPlayers.Add(player);
            allPlayersAlive.Add(player);
            allPlayers.Sort(PlayerManager.Instance.CompareByID);
            allPlayersAlive.Sort(PlayerManager.Instance.CompareByID);
            //EventManager.InvokePlayerJoined(player);
        }
    }
    public void PlayerLeave(PlayerInput playerInput)
    {
        Debug.Log($"Player {playerInput.playerIndex} has left!");

        if (PlayerManager.Instance.AllPlayers is not null) // might not work well
        {
            List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
            List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
            PlayerInputHandler player = PlayerManager.Instance.AllPlayers[playerInput.playerIndex];
            allPlayers.Remove(player);
            allPlayersAlive.Remove(player);
            allPlayers.Sort(PlayerManager.Instance.CompareByID);
            allPlayersAlive.Sort(PlayerManager.Instance.CompareByID);
            Destroy(player.gameObject);
            Destroy(playerInput.gameObject);
        }

        _allPlayersSetupData.RemoveAt(playerInput.playerIndex);
    }
    #endregion
}
