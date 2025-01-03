using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickMenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] bool MenuIsOpen = false;
    [SerializeField] SceneType _nextScene;
    [SerializeField] CustomSceneManager customSceneManager;
    private float _previousTimeScale = 0;

    private void OnEnable()
    {
        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        Time.timeScale = _previousTimeScale;
    }

    public void RestartScene()
    {
        customSceneManager.LoadScene(_nextScene);
        Time.timeScale = 1f;
        Cursor.visible = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
