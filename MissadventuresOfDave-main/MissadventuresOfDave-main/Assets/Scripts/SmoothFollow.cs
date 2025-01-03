using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform _target;   // The target (usually the player)
    [SerializeField] private float _smoothSpeed = 0.125f;  // Smooth speed (higher = slower follow)
    [SerializeField] private Vector3 _offset;  // Offset from the target (can adjust to keep the camera where you want)

    void LateUpdate()
    {
        // Calculate the desired position of the camera based on the target's position and the offset
        Vector3 desiredPosition = _target.position + _offset;

        // Smoothly move the camera towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);

        // Update the camera's position
        transform.position = smoothedPosition;
    }
}