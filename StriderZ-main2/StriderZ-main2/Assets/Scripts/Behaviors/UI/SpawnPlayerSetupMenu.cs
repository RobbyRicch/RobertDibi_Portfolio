using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class SpawnPlayerSetupMenu : MonoBehaviour
{
    //private GameObject _playerSetupLayoutGroup;
    [SerializeField] private GameObject _playerSetupMenuPrefab;
    public GameObject PlayerSetupMenuPrefab => _playerSetupMenuPrefab;

    [SerializeField] private PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;

    private void Start()
    {
        SceneManager.sceneLoaded += CreateCustomizationMenuOnLoad;
        CreateCustomizationMenu();
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= CreateCustomizationMenuOnLoad;
    }

    private void CreateCustomizationMenu()
    {
        /*GameObject playerSetupLayoutGroup = LaserRu.Instance.CharacterCustomizationLayout;
        if (playerSetupLayoutGroup)
        {
            GameObject setupMenu = Instantiate(_playerSetupMenuPrefab, playerSetupLayoutGroup.transform);
            PlayerSetupMenuController playerSetupMenuController = setupMenu.GetComponent<PlayerSetupMenuController>();
            _playerInput.uiInputModule = playerSetupMenuController.PlayerInputModule;
            playerSetupMenuController.PlayerIndex = _playerInput.playerIndex;

            if (CustomSceneManager.IsFirstTimeInMenu)
                playerSetupMenuController.InitializeNewPlayer();
            else
                playerSetupMenuController.InitializeExistingPlayer();

            PlayerSetupManager.Instance.AllPlayersSetupData[_playerInput.playerIndex].IsSetupDone = false;
            //Instantiate(UIManager.Instance.PlayerLogInIcon, UIManager.Instance.PlayerLogInLayOut); // indicating which players connected
        }*/
    }
    private void CreateCustomizationMenuOnLoad(Scene scene, LoadSceneMode lsm)
    {
        if (scene.name != "Main Menu") // need to be better
            return;

        CreateCustomizationMenu();
    }
}
