using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoCanvasTrigger : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] private InfoCanvasManager _infoCanvasManager;
    [SerializeField] private BoxCollider2D _currentTriggerBox;

    [Header("State")]
    [SerializeField] private bool _shouldActivate;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && _shouldActivate)
        {
            
            _infoCanvasManager.ApplyCanvas();
            _shouldActivate = false;
        }
    }
}
