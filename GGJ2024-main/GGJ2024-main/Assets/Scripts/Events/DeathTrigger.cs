using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathTrigger : MonoBehaviour
{
    [Header("OverlayManager Reference")]
    [SerializeField] private OverlayEffectsManager overlayEffectsManager;

    [Header("Booleans")]

    public bool _shouldPlayDeathOverlay;

    private void OnTriggerEnter(Collider other)
    {
        if (_shouldPlayDeathOverlay)
        {
            overlayEffectsManager._levelStartEvent.gameObject.SetActive(true);
            overlayEffectsManager._loseEvent.Play();
            StartCoroutine(overlayEffectsManager.LoseStateOverlayTimer());
            SceneManager.LoadScene("Main_Menu");

        }
    }
}
