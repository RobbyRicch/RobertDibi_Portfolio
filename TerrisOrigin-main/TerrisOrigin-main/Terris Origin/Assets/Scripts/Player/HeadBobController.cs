using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool _enable = true;

    [SerializeField, Range(0, 0.1f)] private float _amplitude = 0.015f;
    [SerializeField, Range(0, 30)] private float _frequency = 10f;

    [SerializeField] private Transform _arms = null;
    [SerializeField] private Transform _camera = null;
    [SerializeField] private Transform _cameraHolder = null;

    private PlayerMotor _motor;
    private float _toggleSpeed = 1f;
    private Vector3 _startPos;
    private Vector3 _armStartPos;
    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _motor = GetComponent<PlayerMotor>();
        _startPos = _camera.localPosition;
        _armStartPos = _arms.localPosition;
    }
    private void Update()
    {
        if (!_enable) return;

        CheckMotion();
        //ResetPosition();
        //_camera.LookAt(FocusTarget());
    }
    private void PlayMotion(Vector3 motion)
    {
        //_camera.localPosition += motion;
        _arms.localPosition += motion;
    }
    private void CheckMotion()
    {
        float speed =  new Vector3(_motor.moveDirection.x,0, _motor.moveDirection.z).magnitude;
        ResetPosition();
        if (speed < _toggleSpeed) return;
        if (!_controller.isGrounded) return;

        PlayMotion(FootStepMotion());
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * _frequency) * _amplitude;
        pos.x += Mathf.Cos(Time.time * _frequency/2) * _amplitude * 2;
        return pos;
    }
    private void ResetPosition()
    {
        /*if (_camera.localPosition == _startPos) return;
        _camera.localPosition = Vector3.Lerp(_camera.localPosition,_startPos,4 * Time.deltaTime);*/
        if (_arms.localPosition == _armStartPos) return;
        _arms.localPosition = Vector3.Lerp(_arms.localPosition, _armStartPos, 4 * Time.deltaTime);
    }
    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x,transform.position.y + _cameraHolder.localPosition.y,transform.position.z);
        pos += _cameraHolder.forward * 15f;
        return pos;
    }
}
