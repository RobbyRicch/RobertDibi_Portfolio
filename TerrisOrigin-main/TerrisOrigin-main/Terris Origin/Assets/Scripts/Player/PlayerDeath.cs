using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    private HealthHandler _playerHealthHandler;
    // Start is called before the first frame update
    void Start()
    {
        _playerHealthHandler = GetComponent<HealthHandler>();
        _playerHealthHandler.OnDeathOccured += _playerHealthHandler_OnDeathOccured;
    }

    private void _playerHealthHandler_OnDeathOccured(object sender, System.EventArgs e)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
