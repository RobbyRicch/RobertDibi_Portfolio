using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 _offeset;
    [SerializeField] Transform target;
    [SerializeField] float smoothTime;
    private Vector3 _currentVelocity = Vector3.zero;
    // Start is called before the first frame update
    private void Awake()
    {
        _offeset = transform.position - target.position;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        //this.transform.LookAt(target);
        Vector3 targetPos = target.position + _offeset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _currentVelocity, smoothTime);
    }
}
