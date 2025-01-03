using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCompleteTrigger : MonoBehaviour
{
    private const string _playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_playerTag))
            EventManager.InvokeTutorialComplete();
    }
}
