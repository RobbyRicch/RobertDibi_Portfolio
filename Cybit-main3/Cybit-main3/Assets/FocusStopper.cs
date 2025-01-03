using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusStopper : MonoBehaviour
{
    private Player_Focus _focus;
    private void Start()
    {
        _focus = FindFirstObjectByType<Player_Focus>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _focus.ResumeTime();
        }
    }
}
