using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GameStates { Paused, Preperations, PreGame, MidGame, LateGame, Results }
public enum GameModeType { Lobby, LaserRush, ArenaOfPeritia, IndiciumGarden }

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    private delegate void GameState();
    private GameState _gameState;

    private GameStates _currentGameState = GameStates.Preperations;
    public GameStates CurrentGameState => _currentGameState;

    private GameStates _tempGameState = GameStates.Preperations;
    public GameStates TempGameState => _tempGameState;

    [SerializeField] private GameModeType _currentGameMode = GameModeType.Lobby;
    public GameModeType CurrentGameMode { get => _currentGameMode; set => _currentGameMode = value; }

    [SerializeField] protected int _roundNumber = 1;
    public int RoundNumber { get => _roundNumber; set => _roundNumber = value; }


    [SerializeField] protected Transform[] _podiumTransforms;
    public Transform[] PodiumTransforms { get => _podiumTransforms; set => _podiumTransforms = value; }

    [SerializeField] private bool _isGamePaused;
    public bool IsGamePaused => _isGamePaused;

    [SerializeField] private bool _isDebugMessagesOn;

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _gameState = PreperationsState;
    }
    private void OnEnable()
    {
        EventManager.OnPlayerPause += OnPlayerPause;
        EventManager.OnPlayersReady += OnPlayersReady;
    }
    private void Update()
    {
        _gameState.Invoke();
    }
    private void OnDisable()
    {
        EventManager.OnPlayerPause -= OnPlayerPause;
        EventManager.OnPlayersReady -= OnPlayersReady;
    }
    #endregion

    #region GameStates
    private void PausedState()
    {
        if (_isDebugMessagesOn) Debug.Log("Game State: Paused.");
    }
    private void PreperationsState()
    {
        if (_isDebugMessagesOn) Debug.Log("Game State: Preperations.");
    }
    private void PreGameState()
    {
        if (_isDebugMessagesOn) Debug.Log("Game State: Pre-Game.");
    }
    private void MidGameState()
    {
        if (_isDebugMessagesOn) Debug.Log("Game State: Mid-Game.");
    }
    private void LateGameState()
    {
        if (_isDebugMessagesOn) Debug.Log("Game State: End-Game.");
    }
    private void ResultsState()
    {
        if (_isDebugMessagesOn) Debug.Log("Game State: Resultts.");
    }
    #endregion

    #region State Alteration
    public void ChangeState(GameStates newGameState)
    {
        _currentGameState = newGameState;

        switch (_currentGameState)
        {
            case GameStates.Preperations:
                _gameState = PreperationsState;
                break;
            case GameStates.PreGame:
                _gameState = PreGameState;
                break;
            case GameStates.MidGame:
                _gameState = MidGameState;
                break;
            case GameStates.LateGame:
                _gameState = LateGameState;
                break;
            case GameStates.Results:
                _gameState = ResultsState;
                break;
            default:
                _gameState = PreperationsState;
                break;
        }
    }
    public void ChangeStateUI(int newStateIndex)
    {
        GameStates newGameState = (GameStates)newStateIndex;
        ChangeState(newGameState);
    }
    #endregion

    #region Slowmotion Behavior
    private IEnumerator FadeTimeScale(float targetTimeScale, float duration)
    {
        float initialTimeScale = Time.timeScale;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            float t = currentTime / duration;
            Time.timeScale = Mathf.Lerp(initialTimeScale, targetTimeScale, t);
            yield return null;
        }
    }
    private IEnumerator DisableSlowMotion(float duration, float fadeOutDuration)
    {
        yield return new WaitForSecondsRealtime(duration);

        // Fade out slow-motion effect
        StartCoroutine(FadeTimeScale(1f, fadeOutDuration));
    }
    public void FateTimeInAndOut(float slowestTime, float timeToWaitToDisableSlowMo, float fadeInTime, float fadeOutTime)
    {
        // Enable slow-motion effect (0.5, 2)
        StartCoroutine(FadeTimeScale(slowestTime, fadeInTime));

        // Set a timer to disable the slow-motion effect after 2 seconds (2f, 0.1f)
        StartCoroutine(DisableSlowMotion(timeToWaitToDisableSlowMo, fadeOutTime));
    }
    #endregion

    protected Transform[] AssignPlayersToPodiums()
    {
        List<PlayerInputHandler> players = PlayerManager.Instance.AllPlayers.OrderByDescending(p => p.Data.Score).ToList();
        Transform[] sortedTransforms = players.Select(player => player.transform).ToArray();
        Quaternion newRotation = Quaternion.identity * Quaternion.Euler(1.0f, 180.0f, 1.0f);

        for (int i = 0; i < sortedTransforms.Length; i++)
        {
            sortedTransforms[i].SetParent(_podiumTransforms[i].transform);
            sortedTransforms[i].localPosition = Vector3.zero;
            sortedTransforms[i].localRotation = Quaternion.identity;
            players[i].Controller.Rb.useGravity = false;
            players[i].Controller.Rb.isKinematic = true;
            sortedTransforms[i].SetParent(null);
            sortedTransforms[i].localRotation = newRotation;
        }

        return sortedTransforms;
    }

    #region Events
    private void OnPlayerPause(PlayerInputHandler player)
    {
        if (!_isGamePaused)
        {
            _tempGameState = _currentGameState;
            ChangeState(GameStates.Paused);
            UIManager.Instance.PauseMenu.SetActive(true);
            _isGamePaused = true;
            Time.timeScale = 0;
        }
        else
        {
            _isGamePaused = false;
            UIManager.Instance.PauseMenu.SetActive(false);
            ChangeState(_tempGameState);
            Time.timeScale = 1;
        }
    }
    private void OnPlayersReady(PlayerInputHandler[] players)
    {
        StartCoroutine(SceneManagerMono.Instance.ChangeSceneOnTimerCoroutine);
    }
    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }
}
