using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GalsMainMenu : MonoBehaviour
{
    [SerializeField] private SceneManagerMono _sceneManager;
    [SerializeField] private GameObject _mainPanel, _localBtn, _infoPanel, _characterCustomizationPanel, _optionsPanel, _creditsPanel,_firstButton;
    [SerializeField] private AudioSource _startSource;

    private bool _isMoreThanOnePlayer = false;

    private void Start()
    {
        _startSource.Play();
        _mainPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(_firstButton);
    }
    private void Update()
    {
        if (!_isMoreThanOnePlayer && PlayerSetupManager.Instance.AllPlayersSetupData.Count > 1)
        {
            ReleasePlayButton();
            _isMoreThanOnePlayer = true;
        }
    }

    public void SetNextSelected(GameObject panel)
    {
        if (panel is not null)
            EventSystem.current.SetSelectedGameObject(panel);
        else
            Debug.Log("Couldn't set next selected, target gameObject is null");
    }
    public void EnableEventSystemOnCustomizationFinish(/*GameObject nextSelected*/)
    {
        //StartCoroutine(UIManager.Instance.GetCharacterCustomizationWindowStateWhenClosed(/*nextSelected*/));
    }
    public void SelectGameMode(int num)
    {
        _sceneManager.NextSceneBuildIndex = num;
    }
    public void ReleasePlayButton()
    {
        _localBtn.GetComponent<UnityEngine.UI.Button>().interactable = true;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
