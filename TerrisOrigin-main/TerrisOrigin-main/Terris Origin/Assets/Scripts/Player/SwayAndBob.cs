using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwayAndBob : MonoBehaviour
{
    [Range(0f, 1f)]
    public float Weight = 1;

    [Header("External References")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerMotor playerMotor;
    [SerializeField] private PlayerLook playerLook;
    public Vector3 playerVelocity;
    private Vector2 walkInput;
    private Vector2 lookInput;

    [Header("Settings")]
    public bool sway = true;
    public bool swayRotation = true;
    public bool bobOffset = true;
    public bool bobSway = true;

    [Header("Sway")]
    public float Step = 0.01f; // multiplied by the value from the mouse for 1 frame
    public float MaxStepDistance = 0.06f; // Max distance from the local origin
    Vector3 swayPos; // Store our value for later

    [Header("Sway Rotation")]
    public float rotationStep = 4f; // Mujltiplied by the value from the mouse for 1 frame
    public float maxRotationStep = 5f; // Max rotation from the local identity rotaion
    Vector3 swayEulerRot; // Store our value

    [Header("Bobbing")]
    public float speedCurve; //used by both bobbing methods
    float curveSin { get => Mathf.Sin(speedCurve); } //easy getter for the sin of our curve
    float curveCos { get => Mathf.Cos(speedCurve); } //easy getter for the cos of our curve
    public Vector3 travelLimit = Vector3.one * 0.025f; //the maximum limits of travel from move input
    public Vector3 bobLimit = Vector3.one * 0.01f; //limits of travel from bobbing over time
    Vector3 bobPosition;

    [Header("Bob Rotation")]
    public Vector3 multiplier;
    Vector3 bobEularRotation;

    [Header("Composite")]
    float smooth = 10f; // used by both BobOffset and sway
    float smoothRot = 12f; // used by both BobSway and TiltSway

    [Header("Landing")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayCastOriginOffset = 0.5f;
    [SerializeField] private Animator animator;
    Vector3 rayCastOrigin;
    bool Landed = false;

    private float _weight;
    private float StepOrg;
    private float MaxStepDistanceOrg;
    private float rotationStepOrg;
    private float maxRotationStepOrg;
    private Vector3 travelLimitOrg;
    private Vector3 bobLimitOrg;
    private Vector3 multiplierOrg;
    private void Awake()
    {
        _weight = Weight;
        SaveInitialeVariables();
        ChangeWeight();
    }
    // Update is called once per frame
    void Update()
    {
        if (Weight != _weight)
        {
            _weight = Weight;
            ReturnInitialeVariables();
            ChangeWeight();
        }

        GetInput();
        //Get each movement and rotation component
        Sway();
        SwayRotation();
        BobOffset();
        BobRotation();
        //LandingOffset();

        //Apply all movement and rotation components
        CompositePositionRotation();
    }
    private void SaveInitialeVariables()
    {
        StepOrg = Step;
        MaxStepDistanceOrg = MaxStepDistance;
        rotationStepOrg = rotationStep;
        maxRotationStepOrg = maxRotationStep;
        travelLimitOrg = travelLimit;
        bobLimitOrg = bobLimit;
        multiplierOrg = multiplier;
    }
    private void ReturnInitialeVariables()
    {
        Step = StepOrg;
        MaxStepDistance = MaxStepDistanceOrg;
        rotationStep = rotationStepOrg;
        maxRotationStep = maxRotationStepOrg;
        travelLimit = travelLimitOrg;
        bobLimit = bobLimitOrg;
        multiplier = multiplierOrg;
    }
    private void ChangeWeight()
    {
        Step *= Weight;
        MaxStepDistance *= Weight;
        rotationStep *= Weight;
        maxRotationStep *= Weight;
        travelLimit *= Weight;
        bobLimit *= Weight;
        multiplier *= Weight;
    }
    void GetInput()
    {
        playerVelocity = new Vector3(playerMotor.moveDirection.x, playerMotor.playerVelocity.y + 2, playerMotor.moveDirection.z);
        walkInput = playerMotor.moveDirection;
        lookInput = playerLook.LookInput;
    }

    void Sway() //x,y,z position change as a result of moving the mouse
    {
        if (!sway) { swayPos = Vector3.zero; return; }

        Vector3 invertLook = lookInput * -Step;
        invertLook.x = Mathf.Clamp(invertLook.x, -MaxStepDistance, MaxStepDistance);
        invertLook.y = Mathf.Clamp(invertLook.y, -MaxStepDistance, MaxStepDistance);

        swayPos = invertLook;
    }
    void SwayRotation() //roll, pitch, yaw change as a result of moving the mouse
    {
        if (!swayRotation) { swayEulerRot = Vector3.zero; return; }

        Vector2 invertLook = lookInput * -rotationStep;
        invertLook.x = Mathf.Clamp(invertLook.x, -maxRotationStep, maxRotationStep);
        invertLook.y = Mathf.Clamp(invertLook.y, -maxRotationStep, maxRotationStep);

        swayEulerRot = new Vector3(invertLook.y, invertLook.x, invertLook.x);
    }
    void BobOffset() //x,y,z position change as a result of walking
    {
        //used to generate our sin and cos waves
        speedCurve += Time.deltaTime * (characterController.isGrounded ? playerVelocity.magnitude : 1f) + 0.01f;

        if (!bobOffset) { bobPosition = Vector3.zero; return; }

        /*if (playerMotor.Jumped && !Landed)
        {
            travelLimit *= 10;
            playerMotor.Jumped = false;
        }
        else if (Landed)
        {
            travelLimit /= 10;
            Landed = false;
        }*/


        bobPosition.x =
            (curveCos * bobLimit.x * (characterController.isGrounded ? 1 : 0)) - (walkInput.x * travelLimit.x); //(bob) - (input offset)
        bobPosition.y =
            (curveSin * bobLimit.y) - (playerVelocity.y * travelLimit.y); //(bob) - (y velocity offset)
        bobPosition.z =
            -(walkInput.y * travelLimit.z); // -(input offset)
    }
    void BobRotation() //roll, pitch, yaw change as a result of walking
    {
        if (!bobSway) { bobEularRotation = Vector3.zero; return; }

        bobEularRotation.x = (walkInput != Vector2.zero ? multiplier.x * (Mathf.Sin(2 * speedCurve)) :
                                                          multiplier.x * (Mathf.Sin(2 * speedCurve) / 2)); //pitch
        bobEularRotation.y = (walkInput != Vector2.zero ? multiplier.y * curveCos : 0);                    //yaw
        bobEularRotation.z = (walkInput != Vector2.zero ? multiplier.z * curveCos * walkInput.x : 0);      //roll
    }
    void CompositePositionRotation()
    {
        //position
        transform.localPosition =
            Vector3.Lerp(transform.localPosition, swayPos + bobPosition, Time.deltaTime * smooth);

        //rotation
        transform.localRotation =
            Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayEulerRot) * Quaternion.Euler(bobEularRotation),
            Time.deltaTime * smoothRot);
    }

    void LandingOffset()
    {
        RaycastHit hit;
        rayCastOrigin = transform.position;
        rayCastOrigin.y -= rayCastOriginOffset;

        if (Physics.Raycast(rayCastOrigin, -Vector3.up, out hit, 0.1f, groundLayer))
        {
            if (!playerMotor.isGrounded && playerVelocity.y < 0)
            {
                Debug.Log(hit.collider.name);
                Landed = true;
                //animator.SetTrigger("Land");
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(rayCastOrigin, 0.2f);
        //Gizmos.DrawRay(rayCastOrigin, -Vector3.up);
        Debug.DrawRay(rayCastOrigin, -Vector3.up, Color.green, 0.1f);
    }
}
