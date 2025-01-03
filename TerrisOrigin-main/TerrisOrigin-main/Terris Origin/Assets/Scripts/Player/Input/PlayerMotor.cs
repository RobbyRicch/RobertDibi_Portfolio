using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private BasicAttack basicAttack;
    public Vector3 playerVelocity;
    public SwayAndBob SnB;
    public bool isGrounded;
    public bool isMoving;
    public bool Jumped = false;
    private bool lerpCrouch;
    private bool crouching;
    private bool sprinting;
    private float crouchTimer;
    private float speed = 5f;
    public float movementSpeed = 5f;
    public float SprintSpeed = 8f;
    private float gravity;
    public float Gravity = -9.8f;
    public float jumpHeight = 1f;
    public Vector3 moveDirection;
    public bool curve = false;
    public bool Landed = true;
    [SerializeField] private LayerMask GroundLayer;
    public float rocketJumpForce = 10.0f;
    private float jumpVelocity;
    public float timeToJumpApex = 0.5f;
    public float RocketJumpSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        basicAttack = GetComponentInChildren<BasicAttack>();
        gravity = Gravity;
        //gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        //jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        CheckLanding();
        /*if (moveDirection.x == 0 && moveDirection.z == 0 && !basicAttack.IsShooting)
            SnB.Weight = 0.5f;
        else if ((moveDirection.x > 0 || moveDirection.x < 0 || moveDirection.z > 0 || moveDirection.z < 0) && !basicAttack.IsShooting)
            SnB.Weight = 0.7f;
        else if (basicAttack.IsShooting)
            SnB.Weight = 0.8f;*/

    }
    //Receive the inputs for our InputManager.cs and apply them to our character controller
    public void ProcessMove(Vector2 input)
    {
        if (!curve)
            moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += Gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime /** RocketJumpSpeed*/);
    }

    public void checkMoveForSound()
    {
        // For Walking
        if ((moveDirection.z > 0 || moveDirection.z < 0 || moveDirection.x > 0 || moveDirection.x < 0) && isGrounded )
        {
            
            SoundManager.Instance.PlaySound(SoundManager.SoundType.Walk);

        }

        // For The Jump
        if (!isGrounded && Jumped && playerVelocity.y > -2) 
        {
            
            SoundManager.Instance.PlaySound(SoundManager.SoundType.Jump);

        }


    }
    
    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3f * Gravity);
            Jumped = true;
            Landed = false;
        }
    }
    private void CheckLanding()
    {
        if (!isGrounded && Jumped && playerVelocity.y < -1f)
        {
            if (Physics.CheckSphere(transform.position , 0.25f, GroundLayer))
            {
                Debug.Log(Landed);
                Landed = true;
                Jumped = false;
            }
        }
    }
    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }
    public void Sprint()
    {
        sprinting = !sprinting;
        if (sprinting)
            speed = SprintSpeed;
        else
            speed = movementSpeed;
    }
    public void EnableGravity(bool state)
    {
        if (state)
            Gravity = gravity;
        else
            Gravity = 0;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }
}
