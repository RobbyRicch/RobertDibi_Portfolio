using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform _startPoint;
    [SerializeField] Transform _endPoint;
    [SerializeField] float _speed;

    Vector3 _position;

    private void Start()
    {
        _position = transform.position;
    }

    private void Update()
    {
        if(transform.position.y >= _endPoint.position.y)
        {
            transform.position = new Vector3(_position.x, _startPoint.position.y, _position.z);
        }

        transform.Translate(Vector3.up*_speed*Time.deltaTime);
    }




}
