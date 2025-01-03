using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private bool _rotateTheOtherWay;

    private void FixedUpdate()
    {
        // Get the current angular velocity of the Rigidbody
        Vector3 currentAngularVelocity = _rb.angularVelocity;

        // Set the new angular velocity to rotate the object at the desired speed
        if (_rotateTheOtherWay)
        {
            // We'll rotate the object around the Y-axis, so we only modify the Y component of the angular velocity
            currentAngularVelocity.y = _rotationSpeed * Mathf.Deg2Rad; // Convert the speed to radians per second
        }
        else
            currentAngularVelocity.y = -_rotationSpeed * Mathf.Deg2Rad; // Convert the speed to radians per second


        // Apply the new angular velocity to the Rigidbody
        _rb.angularVelocity = currentAngularVelocity;
    }
}
