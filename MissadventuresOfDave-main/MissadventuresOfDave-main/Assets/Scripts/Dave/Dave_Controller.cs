using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dave_Controller : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpingPower;
    [SerializeField] private bool _isFacingRight;
    [SerializeField] private bool _isMoving;
    private float _horizontal;

    [Header("Inventory")]
    [SerializeField] public HatBase _currentHat;
    [SerializeField] public EyeWear_Base _currentEyewear;

    [Header("Currency")]
    [SerializeField] public int _coinsInPurse;

    [Header("Refrences")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator _daveAnimatorRef;
    [SerializeField] public Transform _body;
    [SerializeField] public Transform _HatPivot;
    [SerializeField] public Transform _eyewearPivot;
    [SerializeField] public BoxCollider2D _physicsCollider;
    

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");

        if (_isMoving)
        {
            _daveAnimatorRef.SetBool("IsMoving", true);
        }
        else
        {
            _daveAnimatorRef.SetBool("IsMoving", false);
        }

        if (Input.GetKeyDown(KeyCode.W) && IsGrounded())
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpingPower);
        }

        if (Input.GetKeyUp(KeyCode.W) && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
        }

        Flip();
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_horizontal * _speed, _rb.velocity.y);
        if (_rb.velocity.x > 0 || _rb.velocity.x < 0)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }
    }

    private void Flip()
    {
        if (_horizontal > 0f && !_isFacingRight)
        {
            _isFacingRight = true;
            _daveAnimatorRef.SetBool("IsFacingRight", true);
            FlipBody();
        }
        else if (_horizontal < 0f && _isFacingRight)
        {
            _isFacingRight = false;
            _daveAnimatorRef.SetBool("IsFacingRight", false);

            FlipBody();
        }
    }

    private void FlipBody()
    {
        Vector3 bodyScale = _body.localScale;
        bodyScale.x = Mathf.Sign(_body.localScale.x) * -7f;  
        _body.localScale = bodyScale;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }


}

