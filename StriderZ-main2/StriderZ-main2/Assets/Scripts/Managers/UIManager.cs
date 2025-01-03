using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance => _instance;

    [Header("Menus")]
    [SerializeField] private GameObject _pauseMenu;
    public GameObject PauseMenu => _pauseMenu;

    [Header("Transitions")]
    [SerializeField] private RectTransform _lobbyToGameTransitionRTr;
    public RectTransform LobbyToGameTransitionRTr => _lobbyToGameTransitionRTr;

    [SerializeField] private RectTransform _leftLobbyToGameTransitionRTr, _midLobbyToGameTransitionRTr, _rightLobbyToGameTransitionRTr;
    public RectTransform LeftLobbyToGameTransitionRTr => _leftLobbyToGameTransitionRTr;
    public RectTransform MidLobbyToGameTransitionRTr => _midLobbyToGameTransitionRTr;
    public RectTransform RightLobbyToGameTransitionRTr => _rightLobbyToGameTransitionRTr;

    [Header("Countdown")]
    [SerializeField] private GameObject _countdownPanel;
    public GameObject CountdownPanel => _countdownPanel;

    [SerializeField] private Image _countdownImage;
    [SerializeField] private Sprite[] _countdownSprites;

    #region UI Effects
    [Header("UI Effects")]
    [SerializeField] private float _popTime = 0.5f;
    [SerializeField] private float _popHeight = 5.0f;
    [SerializeField] private Vector3 _popTargetSize = new(1.25f, 1.25f, 1), _popPeakSize = new(1.5f, 1.5f, 1);
    public Vector3 PopTargetSize => _popTargetSize;
    public Vector3 PopPeakSize => _popPeakSize;
    #endregion

    [Header("Scoreboard")]
    [SerializeField] private GameObject _scoreboardPanel;
    public GameObject ScoreboardPanel => _scoreboardPanel;

    [Header("Alerts")]
    [SerializeField] private GameObject _winnerAlert;
    public GameObject WinAlert => _winnerAlert;

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
    }
    #endregion

    #region Countdown UI Methods
    public void SetCountdown(bool shouldBeActive)
    {
        if (shouldBeActive)
            _countdownPanel.SetActive(true);
        else
            _countdownPanel.SetActive(false);
    }
    public void ChangeCountdownSprite(int count)
    {
        _countdownImage.sprite = _countdownSprites[count];
    }
    #endregion

    #region UI Effects
    public IEnumerator ShowUIObjectBehaviour(Transform gameObjectTr, Vector3 targetSize)
    {
        float time = 0;
        Vector3 startScale = gameObjectTr.localScale;

        while (time < _popTime / 2)
        {
            gameObjectTr.localScale = Vector3.Lerp(startScale, _popTargetSize, time / (_popTime / 2));
            time += Time.deltaTime;
            yield return null;
        }
        gameObjectTr.localScale = _popTargetSize;

        Vector3 newScale = gameObjectTr.localScale;
        while (time < _popTime / 3)
        {
            gameObjectTr.localScale = Vector3.Lerp(newScale, targetSize, time / (_popTime / 3));
            time += Time.deltaTime;
            yield return null;
        }
        gameObjectTr.localScale = targetSize;
    }
    public IEnumerator PopUIObjectBehaviour(Transform gameObjectTr, float popTime, Vector3 targetPopSize, Vector3 peakPopSize)
    {
        float time = 0;
        Vector3 startScale = gameObjectTr.localScale;
        Vector3 targetPos = gameObjectTr.position;
        targetPos.y += _popHeight;

        while (time < popTime / 2)
        {
            gameObjectTr.localScale = Vector3.Lerp(startScale, peakPopSize, time / (popTime / 2));
            time += Time.deltaTime;
            yield return null;
        }
        gameObjectTr.localScale = peakPopSize;

        Vector3 newScale = gameObjectTr.localScale;
        while (time < popTime / 3)
        {
            gameObjectTr.localScale = Vector3.Lerp(newScale, targetPopSize, time / (popTime / 3));
            time += Time.deltaTime;
            yield return null;
        }
        gameObjectTr.localScale = targetPopSize;
    }
    public IEnumerator UnPopUIObjectBehaviour(Transform gameObjectTr, float popTime)
    {
        if (!gameObjectTr)
            yield break;

        float time = 0;
        Vector3 originalScale = new(1.0f, 1.0f, 1.0f);
        Vector3 startScale = gameObjectTr.localScale;

        while (time < popTime / 4)
        {
            gameObjectTr.localScale = Vector3.Lerp(startScale, originalScale, time / (popTime / 4));
            time += Time.deltaTime;
            yield return null;
        }

        if (!gameObjectTr)
            yield break;
        gameObjectTr.localScale = originalScale;
    }
    public IEnumerator QuickPopBehaviour(Transform gameObjectTr, float quickPopTime, Vector3 targetPopSize, Vector3 peakPopSize)
    {
        yield return StartCoroutine(PopUIObjectBehaviour(gameObjectTr, quickPopTime, targetPopSize, peakPopSize));
        yield return StartCoroutine(UnPopUIObjectBehaviour(gameObjectTr, quickPopTime));
    }
    public void ShowUIObject(Transform gameObjectTr, Vector3 targetSize)
    {
        StartCoroutine(ShowUIObjectBehaviour(gameObjectTr, targetSize));
    }
    public void PopUIObject(Transform gameObjectTr, Vector3 targetPopSize, Vector3 peakPopSize)
    {
        StartCoroutine(PopUIObjectBehaviour(gameObjectTr, _popTime, targetPopSize, peakPopSize));
    }
    public void UnPopUIObject(Transform gameObjectTr)
    {
        StartCoroutine(UnPopUIObjectBehaviour(gameObjectTr, _popTime));
    }
    public void QuickPopObject(Transform gameObjectTr, float quickPopTime, Vector3 targetPopSize, Vector3 peakPopSize)
    {
        StartCoroutine(QuickPopBehaviour(gameObjectTr, quickPopTime, targetPopSize, peakPopSize));
    }
    #endregion

    #region Transitions
    public void FadeIn(float timeToMove)
    {
        _lobbyToGameTransitionRTr.gameObject.SetActive(true);
        StartCoroutine(MoveFadeIn(timeToMove));
    }
    public void FadeOut(float timeToMove)
    {
        StartCoroutine(MoveFadeOut(timeToMove));
    }
    private void FadeReset()
    {
        _lobbyToGameTransitionRTr.position = _leftLobbyToGameTransitionRTr.position;
    }

    public IEnumerator MoveFadeIn(float timeToMove)
    {
        float time = 0;
        Vector3 startPos = _lobbyToGameTransitionRTr.position;
        Vector3 targetPos = _midLobbyToGameTransitionRTr.position;

        while (time < timeToMove)
        {
            _lobbyToGameTransitionRTr.position = Vector3.Lerp(startPos, targetPos, time / timeToMove);
            time += Time.deltaTime;
            yield return null;
        }

        _lobbyToGameTransitionRTr.position = targetPos;
    }
    public IEnumerator MoveFadeOut(float timeToMove)
    {
        float time = 0;
        Vector3 startPos = _lobbyToGameTransitionRTr.position;
        Vector3 targetPos = _rightLobbyToGameTransitionRTr.position;

        while (time < timeToMove)
        {
            _lobbyToGameTransitionRTr.position = Vector3.Lerp(startPos, targetPos, time / timeToMove);
            time += Time.deltaTime;
            yield return null;
        }

        _lobbyToGameTransitionRTr.position = targetPos;
        yield return null;

        _lobbyToGameTransitionRTr.gameObject.SetActive(false);
        FadeReset();
    }
    #endregion

    #region Events
    public void ResumeGame()
    {
        EventManager.InvokePlayerPause(PlayerManager.Instance.AllPlayers[0]); // need to change later cause player is always first player (not that it really matters)
    }
    public void ReturnToLobby()
    {
        EventManager.InvokeReturnToLobby();
        //SceneManager.LoadSceneAsync((int)Scenes.Lobby);
        SceneManager.LoadSceneAsync("LobbyToReturn");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
