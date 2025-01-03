using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningObstacle : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _rotationSpeed = 100.0f;

    Vector3 m_EulerAngleVelocity;

    private void Start()
    {
        m_EulerAngleVelocity = new Vector3(0, _rotationSpeed, 0);
    }
    void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
        _rb.MoveRotation(_rb.rotation * deltaRotation);
    }
}
