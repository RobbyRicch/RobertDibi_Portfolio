using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralCanvas : MonoBehaviour
{
    private static GeneralCanvas _instance;
    public static GeneralCanvas Instance => _instance;

    [SerializeField] private GameObject _pauseMenu;
    public GameObject PauseMenu => _pauseMenu;

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

    public void ResumeGame()
    {
        EventManager.InvokePlayerPause(PlayerManager.Instance.AllPlayers[0]); // need to change later cause player is always first player (not that it really matters)
    }
}
