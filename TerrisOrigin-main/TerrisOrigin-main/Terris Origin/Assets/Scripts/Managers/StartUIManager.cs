using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    [SerializeField] public GameObject MainPanel;
    [SerializeField] public GameObject SettingsPanel;
    [SerializeField] public GameObject ElementChangePanel;
    // Start is called before the first frame update
    void Awake()
    {
        Pause(true);
        //SettingsPanel.SetActive(false);
    }

    public void Pause(bool state)
    {
        if (state)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            MainPanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            MainPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void Settings()
    {
        MainPanel.SetActive(false);
        //SettingsPanel.SetActive(true);
    }
    public void MainMenu()
    {
        //SettingsPanel.SetActive(false);
        MainPanel.SetActive(true);
    }
    public void RestartLevel()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
    public void ChangeElementPanel()
    {
        ElementChangePanel.SetActive(true);
        MainPanel.SetActive(false);
    }
    public void BackToMenu()
    {
        ElementChangePanel.SetActive(false);
        MainPanel.SetActive(true);
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ChangeLeftElement(int value)
    {
        switch (value)
        {
            case 0:
                AbilitiesSelection.Instance.LeftHandElement = AbilitiesSelection.ElementType.Lightning;
                AbilitiesSelection.Instance.LeftHandChange();
                break;
            case 1:
                AbilitiesSelection.Instance.LeftHandElement = AbilitiesSelection.ElementType.Fire;
                AbilitiesSelection.Instance.LeftHandChange();
                break;
        }
    }
    public void ChangeRightElement(int value)
    {
        switch (value)
        {
            case 0:
                AbilitiesSelection.Instance.RightHandElement = AbilitiesSelection.ElementType.Lightning;
                AbilitiesSelection.Instance.RightHandChange();
                break;
            case 1:
                AbilitiesSelection.Instance.RightHandElement = AbilitiesSelection.ElementType.Fire;
                AbilitiesSelection.Instance.RightHandChange();
                break;
        }
    }
}
