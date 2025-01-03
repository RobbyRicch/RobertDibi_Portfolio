using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigurationManager : MonoBehaviour
{
    private static PlayerConfigurationManager _instance;
    public static PlayerConfigurationManager Instance => _instance;

    [SerializeField] int _maxPlayers;

    [SerializeField] private GameObject _playerPrefab;
    public GameObject PlayerPrefab => _playerPrefab;

    private List<PlayerConfiguration> _allPlayersConfig;
    public List<PlayerConfiguration> AllPlayersConfig => _allPlayersConfig;

    private List<ColorData> _allColorHolders;
    public List<ColorData> AllColorHolders => _allColorHolders;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        _allPlayersConfig = new List<PlayerConfiguration>();
        //_allColorHolders = new List<ColorHolder>();
        DontDestroyOnLoad(_instance);
    }
    public void SetPlayerColorAndEmmision(int index, Color mainColor, Color secondaryColor)
    {
        _allPlayersConfig[index].MainColor = mainColor;
        _allPlayersConfig[index].MainEmmisionColor = mainColor;
        _allPlayersConfig[index].SecondaryColor = secondaryColor;
        _allPlayersConfig[index].SecondaryEmmisionColor = secondaryColor * 5f;
        //_playerConfigs[index].BodyEmmisionColor.SetColor("_EmissionColor", color * 5f);
    }
    public void SetPlayerModelNum(int index, int num)
    {
        _allPlayersConfig[index].ModelNum = num;
    }
    public void ReadyUp(int index)
    {
        _allPlayersConfig[index].IsCharacterSetupDone = true;

        /*_allPlayersConfig.Count == _maxPlayers &&*/
        if (_allPlayersConfig.All(playerConfig => playerConfig.IsCharacterSetupDone)) 
        {
            for (int i = 0; i < _allPlayersConfig.Count; i++)
                _allPlayersConfig[i].IsCharacterSetupDone = false;

            if (CustomSceneManager.IsFirstTimeInMenu)
                CustomSceneManager.IsFirstTimeInMenu = false;

            /*UIManager.Instance.CharacterCustomizationLayout.transform.parent.gameObject.SetActive(false);
            UIManager.Instance.GameModsPanel.SetActive(true);
            UIManager.Instance.CustomizationCam.SetActive(true);
            UIManager.Instance.HiddenButton.onClick.Invoke();*/
        }
    }
    public void UnReady(int index)
    {
        _allPlayersConfig[index].IsCharacterSetupDone = false;
    }

    #region Unity Events
    public void PlayerJoin(PlayerInput playerInput)
    {
        Debug.Log($"Player {playerInput.playerIndex} has joined!");

        if (!_allPlayersConfig.Any(playerConfig => playerConfig.ID == playerInput.playerIndex))
        {
            playerInput.transform.SetParent(transform);
            _allPlayersConfig.Add(new PlayerConfiguration(playerInput));

            ColorData colorHolder = transform.GetChild(playerInput.playerIndex).GetComponent<ColorData>();
            _allColorHolders.Add(colorHolder);

            // need fixes
            //GameObject playerIcon = Instantiate(UIManager.Instance.PlayerLogInIcon, UIManager.Instance.PlayerLogInLayOut);

            //Transform playerIconText = playerIcon.transform.GetChild(0);
            //playerIconText.GetComponent<TextMeshProUGUI>().text += (playerInput.playerIndex + 1).ToString();
        }
    }
    #endregion
}
