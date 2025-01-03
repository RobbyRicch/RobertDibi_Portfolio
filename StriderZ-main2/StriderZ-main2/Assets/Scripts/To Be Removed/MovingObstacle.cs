using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    Vector3 _originalPosition;
    [SerializeField] Vector3 _newPosition;
    [Header("")]
    public float _x;
    public float _y;
    public float _z;
    public bool _isMoving;
    public float _moveSpeed;

    private void Start()
    {
        _originalPosition = transform.position;
        _newPosition = new Vector3(transform.position.x + _x, transform.position.y + _y, transform.position.z + _z);
    }

    private void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _newPosition, Time.deltaTime * _moveSpeed);
            if (transform.position == _newPosition)
            {
                _isMoving = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _originalPosition, Time.deltaTime * _moveSpeed);
            if (transform.position == _originalPosition)
            {
                _isMoving = true;
            }
        }
    }
}
