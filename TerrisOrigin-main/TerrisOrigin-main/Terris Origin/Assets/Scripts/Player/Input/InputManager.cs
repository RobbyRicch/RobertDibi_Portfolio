using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.PlayerActions player;

    private PlayerMotor motor;
    private PlayerLook look;

    [SerializeField] private StartUIManager startUIManager;

    public bool AttackPressed = false;
    public bool AttackHold = false;
    public bool SecondaryBasicHold = false;
    public bool SecondaryBasicHoldCanceled = false;

    //Lightning
    public bool DashPress = false;
    public bool DashHold = false;
    public bool DashHoldCanceled = false;
    public bool WrathPress = false;
    public bool WrathHold = false;
    public bool WrathHoldCanceled = false;

    //Fire
    public bool FireAura = false;
    public bool FireCanonBallPress = false;
    public bool FireCanonBallHold = false;
    public bool FireCanonBallHoldCanceled = false;

    public bool Paused = false;
    public bool Tab = false;

    private void Awake()
    {
        playerInput = new PlayerInput();
        player = playerInput.Player;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        player.Jump.performed += ctx => motor.Jump();
        player.Crouch.performed += ctx => motor.Crouch();
        player.Sprint.performed += ctx => motor.Sprint();

        /*if (!SecondaryBasicHold)
        {
            player.BasicShootPress.started += ctx =>
            {
                if (ctx.interaction is PressInteraction)
                {
                    AttackPressed = true;
                }
            };
            player.BasicShootPress.canceled += ctx => AttackPressed = false;

            player.BasicShootHold.started += ctx =>
            {
                if (ctx.interaction is HoldInteraction)
                {
                    AttackHold = true;
                }
            };
            player.BasicShootHold.canceled += ctx => AttackHold = false;
        }

        if (!AttackPressed || !AttackHold)
        {
            player.SecondaryBasicHold.started += ctx =>
            {
                if (ctx.interaction is HoldInteraction)
                {
                    SecondaryBasicHold = true;
                }
            };
            player.SecondaryBasicHold.canceled += ctx =>
            {
                if (SecondaryBasicHold && ctx.canceled)
                {
                    SecondaryBasicHold = false;
                    SecondaryBasicHoldCanceled = true;
                }
            };
        }*/



        player.Pause.started += ctx =>
        {
            if (Paused)
            {
                Paused = false;
            }
            else
            {
                Paused = true;
            }
            startUIManager.Pause(Paused);
        };

        player.Tab.started += ctx =>
        {
            if (ctx.interaction is HoldInteraction)
            {
                Tab = true;
            }
            //startUIManager.ToggleIntroScreen(Tab);
        };
        player.Tab.canceled += ctx =>
        {
            Tab = false;
            //startUIManager.ToggleIntroScreen(Tab);
        };
    }
    private void Update()
    {

        player.BasicShootPress.started += ctx =>
        {
            if (ctx.interaction is PressInteraction)
            {
                if (!SecondaryBasicHold)
                {
                    AttackPressed = true;
                }
            }
        };
        player.BasicShootPress.canceled += ctx => AttackPressed = false;

        player.BasicShootHold.started += ctx =>
        {
            if (ctx.interaction is HoldInteraction)
            {
                if (!SecondaryBasicHold)
                {
                    AttackHold = true;
                }
            }
        };
        player.BasicShootHold.canceled += ctx => AttackHold = false;


        player.SecondaryBasicHold.started += ctx =>
        {
            if (ctx.interaction is HoldInteraction)
            {
                if (!AttackPressed || !AttackHold)
                {
                    SecondaryBasicHold = true;
                }
            }
        };
        player.SecondaryBasicHold.canceled += ctx =>
        {
            if (SecondaryBasicHold && ctx.canceled)
            {
                if (!AttackPressed || !AttackHold)
                {
                    SecondaryBasicHold = false;
                    SecondaryBasicHoldCanceled = true;
                }
            }
        };


        player.Dash.started += ctx =>
        {
            if (ctx.interaction is HoldInteraction)
            {
                if (!WrathPress && !WrathHold)
                {
                    DashHold = true;
                    FireCanonBallHold = true;
                }
            }
            else if (ctx.interaction is PressInteraction)
            {
                if (!WrathPress && !WrathHold)
                {
                    DashPress = true;
                    FireCanonBallPress = true;
                }
            }
        };
        player.Dash.canceled += ctx =>
        {
            if (!WrathPress && !WrathHold)
            {
                DashPress = false;
                FireCanonBallPress = false;
                DashHold = false;
                FireCanonBallHold = false;
                DashHoldCanceled = true;
                FireCanonBallHoldCanceled = true;
            }
        };

        player.WrathPress.started += ctx =>
        {
            if (ctx.interaction is TapInteraction)
            {
                if (!DashPress && !DashHold)
                {
                    WrathPress = true;
                    FireAura = true;
                }
            }
        };
        player.WrathPress.canceled += ctx => 
        {
            if (!DashPress && !DashHold)
            {
                WrathPress = false;
                FireAura = false;
            }
        };

        player.WrathHold.performed += ctx =>
        {
            if (ctx.interaction is HoldInteraction)
            {
                if (!DashPress && !DashHold)
                {
                    WrathHold = true;
                }
            }
        };
        player.WrathHold.canceled += ctx =>
        {
            if (!DashPress && !DashHold)
            {
                WrathHold = false;
                WrathHoldCanceled = true;
            }
        };

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        //tell the playermotor to move using the value from our movement action
        motor.ProcessMove(player.Movement.ReadValue<Vector2>());
        motor.checkMoveForSound();
    }
    private void LateUpdate()
    {
        look.ProcessLook(player.Look.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        player.Enable();
    }
    private void OnDisable()
    {
        player.Disable();
    }
    public void ResetValues()
    {
        AttackPressed = false;
        AttackHold = false;
        SecondaryBasicHold = false;
        SecondaryBasicHoldCanceled = false;
        DashPress = false;
        DashHold = false;
        DashHoldCanceled = false;
        WrathPress = false;
        WrathHold = false;
        WrathHoldCanceled = false;
        FireAura = false;
        FireCanonBallPress = false;
        FireCanonBallHold = false;
        FireCanonBallHoldCanceled = false;
    }
}
