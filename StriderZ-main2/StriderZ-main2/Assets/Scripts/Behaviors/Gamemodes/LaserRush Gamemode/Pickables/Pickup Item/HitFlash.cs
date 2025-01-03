using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float _flashDuration = 2f;
    private bool _isFlashing;
    private Color _flashColor;
    private Color _originalColor;
    private float startFlashTime;

    //private void OnEnable()
    //{
    //    EventManager.OnStun += OnPlayerStunned;
    //}
    //private void OnDisable()
    //{
    //    EventManager.OnStun -= OnPlayerStunned;
    //}

    private void OnPlayerStunned(Rigidbody playerRigidbody)
    {
        if (playerRigidbody.gameObject != _playerController.gameObject)
            return;

        _isFlashing = true;
        startFlashTime = Time.time;
        StartCoroutine( Flash(_playerController.InputHandler.Data.ModelData.BodyMesh.material, _flashDuration));
    }

    IEnumerator Flash(Material mat, float sec)
    {
        _originalColor = mat.color;
        while (_isFlashing)
        {
            float currentTime = Time.time;
            if (startFlashTime + sec > currentTime)
            {
                if (mat.color == _originalColor)
                {
                    mat.color = _flashColor;
                    // add delay with waitForSeconds (0.05f)
                }
                else
                {
                    mat.color = _originalColor;
                    // add delay with waitForSeconds (0.05f)
                }
                yield return null;
            }

            _isFlashing = false;
            mat.color = _originalColor;
        }
    }
}
