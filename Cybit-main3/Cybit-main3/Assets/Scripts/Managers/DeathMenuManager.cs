using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathMenuManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _retryTMP, _quitTMP;
    [SerializeField] private float _timeToFade = 1.0f;
    private CustomSceneManager _sceneManager;

    private IEnumerator InitializeLevel()
    {
        SaveManager saveManager = SaveManager.Instance;
        LinkIntegritySystem lis = saveManager.Player.LIS;
        yield return null;

        _sceneManager = saveManager.SceneCustomManager;
        yield return null;

        lis.LinkHudAnimator.SetBool("Dead", true);
        yield return null;

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        yield return null;
    }
    private IEnumerator HandleDeathRoutine(bool isReturningToMenu)
    {
        SaveManager saveManager = SaveManager.Instance;
        Player_Controller playerController = saveManager.Player;

        playerController.ReverseDeathVFX();
        yield return null;

        _sceneManager.Retry(isReturningToMenu);
    }

    public void ActivateDeathCanvas()
    {
        gameObject.SetActive(true);
        StartCoroutine(InitializeLevel());
    }
    public void HandleDeath(bool isReturningToMenu) // - for returning to training room not checkpoint
    {
        StartCoroutine(HandleDeathRoutine(isReturningToMenu));
    }

    public void OnQuitButtonClick()
    {
        _sceneManager.QuitGame();
    }
}
