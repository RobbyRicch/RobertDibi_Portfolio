using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool canMove;

    [SerializeField] CharacterController _controller;
    [SerializeField] Transform _cam;

    [SerializeField] float _speed;
    [SerializeField] float _maxspeed;
    float _startSpeed;

    [Header("Body")]
    [SerializeField] Transform body;
    [SerializeField] float turnSpeed;
    [SerializeField] float turnSmooth;

    [Header("Ground Check")]
    [SerializeField] LayerMask _groundMask;
    [SerializeField] float _radiusCheck;
    [SerializeField] Transform _baseCheck;

    [Header("Gravity")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _maxJumpForce;
    [SerializeField] float _jumpAccelerate;

    [SerializeField] float _gravity;
    [SerializeField] float _coyoteTimer;
    [SerializeField] bool _isGrounded;

    Vector3 _moveDirection;
    float _startJumpForce;
    float _horizontal;

    private void Start()
    {
        canMove = true;
        _controller = GetComponent<CharacterController>();
        _startJumpForce = _jumpForce;
        _startSpeed = _speed;
    }
    private void Update()
    {
        PlayerInput();
    }

    void PlayerInput()
    {
        if (canMove && GameManager.Instance.health>0)
        {
            Jump();
            Move();
            _controller.Move(_moveDirection * Time.deltaTime);
        }
    }

    void Jump()
    {
        if (CheckGround())
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (_jumpForce < _maxJumpForce)
                {
                    _jumpForce += _jumpAccelerate * Time.deltaTime;
                    AnimationManager.Instance.PlayJump(_jumpForce);
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                AnimationManager.Instance.PlayTypeJump();
                AnimationManager.Instance.OnGrounded(false);
                _moveDirection.y = _jumpForce;
                _jumpForce = _startJumpForce;
            }

            if (_moveDirection.y < 0)
            {
                _moveDirection.y = -2;
            }
        }
        else
        {
            _moveDirection.y += _gravity * Time.deltaTime;
        }
    }



    private void Move()
    {
        _horizontal= Input.GetAxis("Horizontal");
        if(_horizontal!=0)
        {
            if (_speed < _maxspeed) { _speed += Time.deltaTime; }
            else { _speed = _maxspeed; }
            AnimationManager.Instance.PlayRun(_speed);
        }
        else
        {
            _speed = _startSpeed;
            AnimationManager.Instance.PlayRun(0);
        }
        RotateBody();
        _moveDirection.z = _horizontal * _speed;
    }

    bool CheckGround()
    {
        bool _tempGroundCheck = _isGrounded;
        _isGrounded = Physics.CheckSphere(_baseCheck.position, _radiusCheck, _groundMask);
        if(_isGrounded && _tempGroundCheck==false)
        {
            AnimationManager.Instance.PlayJump(0);
            AnimationManager.Instance.OnGrounded(true);
        }
        return _isGrounded;
    }

    IEnumerator CoyoteEffect()
    {
        yield return new WaitForSeconds(_coyoteTimer);
    }

    void RotateBody()
    {
        Vector3 bodyDirrect = new Vector3(_horizontal, 0, 0);
        if (bodyDirrect.magnitude > 0.1f)
        {
            float _angle = Mathf.Atan2(bodyDirrect.x, bodyDirrect.z) * Mathf.Rad2Deg + _cam.eulerAngles.y;
            float _targetAngle = Mathf.SmoothDampAngle(body.eulerAngles.y, _angle, ref turnSpeed, turnSmooth);
            body.rotation = Quaternion.Euler(0, _targetAngle, 0);
        }
    }

}
