using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyNotes : MonoBehaviour
{
    bool _isVFXDone;
    bool _canPress = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _canPress = true;
            Debug.Log("TRIGGER");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _canPress == true && _isVFXDone == false)
        {
            GameManager.Instance.RemoveNotes();
        }
    }
}
