using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class GetPlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera _CameraRef;
    [SerializeField] private VideoPlayer _videoplayerRef;

    void Start()
    {
        // If _CameraRef is not set in the Inspector, find the main camera in the scene
        if (_CameraRef == null)
        {
            _CameraRef = Camera.main; // Finds the main camera tagged as "MainCamera"
        }

        // If the _videoplayerRef is set, assign the target camera to it
        if (_videoplayerRef != null && _CameraRef != null)
        {
            _videoplayerRef.targetCamera = _CameraRef;
        }
    }
}