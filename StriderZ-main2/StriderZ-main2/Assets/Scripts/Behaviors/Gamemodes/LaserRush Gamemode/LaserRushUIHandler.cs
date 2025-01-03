using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LaserRushUIHandler : MonoBehaviour
{
    [Header("Players UI")]
    [SerializeField] private GameObject _playersUI;
    [SerializeField] private GameObject[] _playerPanels;
    [SerializeField] private RectTransform[] _playerPanelsRTr;
    [SerializeField] private Image[] _playerBackgrounds;
    [SerializeField] private Image[] _playerIcons;
    [SerializeField] private TextMeshProUGUI[] _playerScores;

    [Header("Placement Values")]
    [SerializeField]
    private Vector2[] _uiPositions = new Vector2[4]
        { new(-5.0f, -53.33333f), new(-20.0f, -140.0f), new(-35.0f, -226.6666f), new(-50.0f, -313.3333f) };

    private List<PlayerInputHandler> _playerPlacements;
    private List<PlayerInputHandler> _tempPlayerPlacements;

    private bool _isUiOnline = false;
    private void OnEnable()
    {
        EventManager.OnGameModeBegin += OnGameModeBegin;
        EventManager.OnPlayerDeath += OnPlayerDeath;
        EventManager.OnGameModeLate += OnGameModeLate;
        EventManager.OnGameModeResults += OnGameModeResults;
    }
    private void OnDisable()
    {
        EventManager.OnGameModeBegin -= OnGameModeBegin;
        EventManager.OnPlayerDeath -= OnPlayerDeath;
        EventManager.OnGameModeLate -= OnGameModeLate;
        EventManager.OnGameModeResults -= OnGameModeResults;
    }
    private void Update()
    {
        if (_isUiOnline)
        {
            for (int i = 0; i < _playerPlacements.Count; i++)
            {
                PlayerInputHandler player = _playerPlacements[i];
                int playerID = player.Data.ID;

                if (_playerPanelsRTr[playerID].anchoredPosition != _uiPositions[i])
                {
                    _playerPanelsRTr[playerID].anchoredPosition = _uiPositions[i];
                    Vector3 targetPopSize = Vector3.one;
                    targetPopSize.z = _playerPanelsRTr[playerID].transform.localScale.z;

                    Vector3 peakPopSize = new Vector3(1.1f, 1.1f, _playerPanelsRTr[playerID].transform.localScale.z);
                    UIManager.Instance.QuickPopObject(_playerPanelsRTr[playerID].transform, 0.25f, targetPopSize, peakPopSize);
                }
            }
        }
    }

    public void ApplyColorToPlayerUI(PlayerInputHandler player)
    {
        if (_playerBackgrounds.Length < 1)
            return;

        int playerID = player.Data.ID;

        Color color = player.SetupData.ColorData.EmissionEmissionColor;
        color.a = 0.5f;
        _playerBackgrounds[playerID].color = color;
    }

    private void OnGameModeBegin()
    {
        if (_playerBackgrounds.Length < 1)
            return;

        _playerPlacements = LaserRushGameMode.Instance.PlayerPlacement;
        _tempPlayerPlacements = new List<PlayerInputHandler>(_playerPlacements);

        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        for (int i = 0; i < allPlayersAlive.Count; i++)
        {
            PlayerInputHandler player = allPlayersAlive[i];
            int playerID = player.Data.ID;
            int playerScore = player.Data.Score;

            Color color = player.SetupData.ColorData.EmissionEmissionColor;
            color.a = 0.5f;
            _playerBackgrounds[playerID].color = color;

            _playerIcons[playerID].sprite = player.Data.ModelData.IconImage.sprite;
            _playerScores[playerID].text = playerScore.ToString();
            _playerPanels[playerID].SetActive(true);
        }
        _playersUI.SetActive(true);
        _isUiOnline = true;
    }
    private void OnPlayerDeath(PlayerInputHandler player)
    {
        if (_playerBackgrounds.Length < 1)
            return;

        int playerID = player.Data.ID;
        Color color = Color.grey;
        color.a = 0.25f;
        _playerBackgrounds[playerID].color = color;
        _playerPanelsRTr[playerID].anchoredPosition = _uiPositions[_playerPlacements.Count];
    }
    private void OnGameModeLate()
    {
        _isUiOnline = false;
    }
    private void OnGameModeResults()
    {
        _playersUI.SetActive(false);
    }
}
