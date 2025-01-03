using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerMono : MonoBehaviour
{
    private static SceneManagerMono _instance;
    public static SceneManagerMono Instance => _instance;

    [SerializeField] private Scenes _currentSceneType;

    [SerializeField] private int _currentSceneBuildIndex = 0;
    public int CurrentSceneBuildIndex { get => _currentSceneBuildIndex; set => _currentSceneBuildIndex = value; }

    [SerializeField] private int _nextSceneBuildIndex = 1;
    public int NextSceneBuildIndex { get => _nextSceneBuildIndex; set => _nextSceneBuildIndex = value; }

    [SerializeField] private float _timerFinishTime = 3.0f, _currentTimerTime;
    public float TimerFinishTime { get => _timerFinishTime; set => _timerFinishTime = value; }
    public float CurrentTimerTime => _currentTimerTime;

    [SerializeField] private float _changeSceneDelay = 2f;
    public float ChangeSceneDelay => _changeSceneDelay;

    [SerializeField] private bool _isChangingScene = false;

    private IEnumerator _changeSceneOnTimerCoroutine;
    public IEnumerator ChangeSceneOnTimerCoroutine => _changeSceneOnTimerCoroutine;

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
    private void Start()
    {
        _currentTimerTime = _timerFinishTime;
        _changeSceneOnTimerCoroutine = StartChangeSceneTimer();
        _currentSceneBuildIndex = CustomSceneManager.GetCurrentSceneID();
    }

    public void ChangeToNextTargetScene()
    {
        CustomSceneManager.ChangeScene(_nextSceneBuildIndex);
        _currentSceneBuildIndex = _nextSceneBuildIndex;
    }
    public void ContinueToGameWithDelay()
    {
        StartCoroutine(ContinueToGameAfterSeconds(_changeSceneDelay));
    }
    public void ChangeScene(int num)
    {
        CustomSceneManager.ChangeScene(num);
    }
    public void ChangeSceneWithDelay(int num)
    {
        StartCoroutine(ChangeSceneAfterSeconds(num, _changeSceneDelay));
    }

    private IEnumerator ChangeSceneAfterSeconds(int num, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CustomSceneManager.ChangeScene(num);
    }
    private IEnumerator ContinueToGameAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        CustomSceneManager.ChangeScene(_nextSceneBuildIndex);
        _currentSceneBuildIndex = _nextSceneBuildIndex;
    }

    private IEnumerator StartChangeSceneTimer()
    {
        _isChangingScene = true;
        while (_currentTimerTime > 0)
        {
            if (!_isChangingScene)
            {
                _currentTimerTime = _timerFinishTime;
                yield break;
            }

            yield return new WaitForSeconds(1.0f);
            _currentTimerTime--;
        }

        foreach (PlayerInputHandler player in PlayerManager.Instance.AllPlayers)
        {
            player.transform.rotation = Quaternion.identity;
            player.Controller.IsInLobby = false;
            StartCoroutine(player.Controller.Disintegrate);
            yield return new WaitForSeconds(player.Controller.TimeToDisintegrate/2.5f); // small delay between player disintegration.
        }

        UIManager.Instance.LobbyToGameTransitionRTr.gameObject.SetActive(true);
        yield return StartCoroutine(UIManager.Instance.MoveFadeIn(0.5f));

        _changeSceneOnTimerCoroutine = null;
        _changeSceneOnTimerCoroutine = StartChangeSceneTimer();
        _currentTimerTime = _timerFinishTime;

        ChangeToNextTargetScene();
    }
    public void StopTimer()
    {
        _isChangingScene = false;
        StopCoroutine(_changeSceneOnTimerCoroutine);
        _changeSceneOnTimerCoroutine = null;
        _currentTimerTime = _timerFinishTime;
        _changeSceneOnTimerCoroutine = StartChangeSceneTimer();
    }

    public void QuitGame()
    {
        QuitGame();
    }

    #region Events
    #endregion
}
