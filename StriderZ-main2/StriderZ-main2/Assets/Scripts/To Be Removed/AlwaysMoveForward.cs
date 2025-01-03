using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysMoveForward : MonoBehaviour
{
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _maxVelocity;
    [SerializeField] private bool _isCamera;

    private void FixedUpdate()
    {
        if (!_isCamera)
        {
            if (_playerRb.velocity.z > _maxVelocity)
            {
                _playerRb.velocity = new Vector3(0f, 0f, _maxVelocity);
            }
            else if (_playerRb.velocity.z < _maxVelocity)
            {
                _playerRb.AddForce(new Vector3(0f, 0f, _moveSpeed * Time.fixedDeltaTime), ForceMode.Acceleration);
            }
        }
        else
        {
            transform.position += new Vector3(0f, 0f, _moveSpeed * Time.fixedDeltaTime);
        }

        Debug.Log(_playerRb.velocity.z);
    }
}
