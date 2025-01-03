using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    MainMenu,
    Training,
    Tutorial,
    Level,
}

public class CustomSceneManager : MonoBehaviour, IProfileSaveable
{
    [SerializeField] private SceneType _currentScene;
    [SerializeField] private UIFader _fader;
    [SerializeField] private float _timeToFade;
    private bool _isTutorialComplete = false;

    private void Start()
    {
        _currentScene = (SceneType)SceneManager.GetActiveScene().buildIndex;
    }

    private IEnumerator MenuToScene()
    {
        yield return StartCoroutine(_fader.FadeInRoutine(_timeToFade)); // should see if everything okay before continuing
    }

    private IEnumerator ToTutorialRoutine()
    {
        yield return StartCoroutine(MenuToScene());
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene((int)SceneType.Tutorial);
    }
    public void ToTutorial()
    {
        StartCoroutine(ToTutorialRoutine());
    }

    private IEnumerator GoToTrainingRoutine()
    {
        yield return StartCoroutine(MenuToScene());
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene((int)SceneType.Training);
    }
    public void GoToTraining()
    {
        StartCoroutine(GoToTrainingRoutine());
    }
    
    private IEnumerator GoToLevelRoutine()
    {
        yield return StartCoroutine(MenuToScene());
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene((int)SceneType.Level);
    }
    public void GoToLevel()
    {
        StartCoroutine(GoToLevelRoutine());
    }
    public void GoToLevelRoutineNoTransition()
    {
        SceneManager.LoadScene((int)SceneType.Level);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator DeathToMenuRoutine()
    {
        yield return StartCoroutine(_fader.FadeInRoutine(_timeToFade));
        SceneManager.LoadScene((int)SceneType.MainMenu);
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void DeathToMenu()
    {
        StartCoroutine(DeathToMenuRoutine());
    }

    private IEnumerator RetryRoutine()
    {
        yield return StartCoroutine(MenuToScene());
    }
    public void Retry(bool isReturningToMenu)
    {
        GoToLevel(); // restart without entering the training room - no starting weapon change
    }

    public void HandlePlayTransition()
    {
        SaveManager saveManager = SaveManager.Instance;
        saveManager.LoadGame();

        if (!_isTutorialComplete)
            ToTutorial();
        else if (SceneManager.GetActiveScene().buildIndex == (int)SceneType.Training) // if player in training room the next scene should be the level
            GoToLevel();
        else
            GoToTraining();
    }
    public void LoadScene(SceneType nextScene)
    {
        int scenBuildIndex = (int)nextScene;
        SceneManager.LoadScene(scenBuildIndex);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadData(Profile data)
    {
        _isTutorialComplete = data.HasFinishedTutorial;
    }
    public void SaveData(ref Profile data)
    {
        data.HasFinishedTutorial = _isTutorialComplete;
    }
}
