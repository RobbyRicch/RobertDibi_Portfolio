using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    private const string _playerTag = "Player";
    private Player_Controller _playerController;

    private void OnEnable()
    {
        EventManager.OnInteract += OnInteract;
    }
    private void OnDisable()
    {
        EventManager.OnInteract -= OnInteract;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_playerTag))
        {
            if (collision.TryGetComponent(out Player_Controller player))
                _playerController = player;
            else if (collision.transform.parent.TryGetComponent(out Player_Controller parentPlayer))
                _playerController = parentPlayer;

            if (_playerController)
            {
                _playerController.InteractionKey.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(_playerTag))
        {
            if (_playerController)
            {
                _playerController.InteractionKey.SetActive(false);
                _playerController = null;
            }
        }
    }

    private void OnInteract()
    {
        if (_playerController)
            EventManager.InvokeVendingMachineInteract(_playerController);

        EventManager.InvokeCloseObjective(true);
    }
}
