using System.Collections.Generic;
using UnityEngine;

public class BodyTilt : MonoBehaviour
{
    public float tiltAngle = 10f; // Adjust this to control the tilt angle
    public float rotationSpeed = 10f; // Adjust this to control the rotation speed
    public float returnSpeed = 5f; // Adjust this to control the return speed

    //public Transform leftHandTransform; // Reference to the left hand transform
    //public Transform rightHandTransform; // Reference to the right hand transform
    public List<Transform> IgnoreTransforms; // Reference to the child transform to ignore
    public List<Transform> IgnoreTransformsNoY; // Reference to the child transform to ignore

    //private Rigidbody rb; // Reference to the Rigidbody component
    private Quaternion targetBodyRotation; // Target rotation for the body
    private Quaternion targetLeftHandRotation; // Target rotation for the left hand
    private Quaternion targetRightHandRotation; // Target rotation for the right hand
    private Quaternion defaultBodyRotation; // Default rotation for the body

    private bool isTilted = false; // Flag to track if the character is currently tilted

    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        defaultBodyRotation = transform.localRotation;
    }

    public void TiltBody(PlayerInputHandler player, float inpuX, float inputY)
    {
        // Calculate the target rotation based on the movement input
        Vector3 forward = player.transform.forward;
        Vector3 right = player.transform.right;
        Vector3 bodyTiltEulerAngles = Quaternion.LookRotation(forward, Vector3.up) * new Vector3(inputY * tiltAngle, 0f, -inpuX * tiltAngle);
        targetBodyRotation = Quaternion.Euler(bodyTiltEulerAngles);

        Vector3 handTiltEulerAngles = Quaternion.LookRotation(forward, Vector3.up) * new Vector3(-inputY * tiltAngle, 0f, inpuX * tiltAngle);
        targetLeftHandRotation = Quaternion.Euler(handTiltEulerAngles);
        targetRightHandRotation = Quaternion.Euler(handTiltEulerAngles);

        // Apply the target rotations using Rigidbody interpolation
        if (inpuX != 0f || inputY != 0f)
        {
            // Smoothly tilt the body when there is movement input
            isTilted = true; // Set the flag to indicate the character is tilted
            player.Controller.Rb.MoveRotation(Quaternion.RotateTowards(player.Controller.Rb.rotation, targetBodyRotation, rotationSpeed * Time.deltaTime));
            //player.Data.HandsRig[0].rotation = Quaternion.RotateTowards(player.Data.HandsRig[0].rotation, targetLeftHandRotation, rotationSpeed * Time.deltaTime);
            //player.Data.HandsRig[1].rotation = Quaternion.RotateTowards(player.Data.HandsRig[1].rotation, targetRightHandRotation, rotationSpeed * Time.deltaTime);
        }
        else if (isTilted)
        {
            // Smoothly return the body to the default rotation when there is no movement input and the character is tilted
            player.Controller.Rb.MoveRotation(Quaternion.RotateTowards(player.Controller.Rb.rotation, defaultBodyRotation, returnSpeed * Time.deltaTime));
            //player.Data.HandsRig[0].rotation = Quaternion.RotateTowards(player.Data.HandsRig[0].rotation, Quaternion.identity, returnSpeed * Time.deltaTime);
            //player.Data.HandsRig[1].rotation = Quaternion.RotateTowards(player.Data.HandsRig[1].rotation, Quaternion.identity, returnSpeed * Time.deltaTime);

            if (Quaternion.Angle(player.Controller.Rb.rotation, defaultBodyRotation) < 0.01f)
            {
                // Reset the flag when the body rotation is close to the default rotation
                isTilted = false;
            }
        }

        // Ignore rotation on the specific child element
        foreach (Transform ignoreTransform in IgnoreTransforms)
        {
            ignoreTransform.rotation = Quaternion.identity;
        }
        foreach (Transform ignoreTransform in IgnoreTransformsNoY)
        {
            ignoreTransform.rotation = new Quaternion(0,ignoreTransform.rotation.y,0,0);
        }
    }

    /*public void TiltBody(float inputY, float inpuX)
    {
        // Calculate the target rotation based on the movement input
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 bodyTiltEulerAngles = Quaternion.LookRotation(forward, Vector3.up) * new Vector3(inputY * tiltAngle, 0f, -inpuX * tiltAngle);
        targetBodyRotation = Quaternion.Euler(bodyTiltEulerAngles);

        Vector3 handTiltEulerAngles = Quaternion.LookRotation(forward, Vector3.up) * new Vector3(-inputY * tiltAngle, 0f, inpuX * tiltAngle);
        targetLeftHandRotation = Quaternion.Euler(handTiltEulerAngles);
        targetRightHandRotation = Quaternion.Euler(handTiltEulerAngles);

        // Apply the target rotations using Rigidbody interpolation
        if (inpuX != 0f || inputY != 0f)
        {
            // Smoothly tilt the body when there is movement input
            isTilted = true; // Set the flag to indicate the character is tilted
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetBodyRotation, rotationSpeed * Time.deltaTime));
            leftHandTransform.rotation = Quaternion.RotateTowards(leftHandTransform.rotation, targetLeftHandRotation, rotationSpeed * Time.deltaTime);
            rightHandTransform.rotation = Quaternion.RotateTowards(rightHandTransform.rotation, targetRightHandRotation, rotationSpeed * Time.deltaTime);
        }
        else if (isTilted)
        {
            // Smoothly return the body to the default rotation when there is no movement input and the character is tilted
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, defaultBodyRotation, returnSpeed * Time.deltaTime));
            leftHandTransform.rotation = Quaternion.RotateTowards(leftHandTransform.rotation, Quaternion.identity, returnSpeed * Time.deltaTime);
            rightHandTransform.rotation = Quaternion.RotateTowards(rightHandTransform.rotation, Quaternion.identity, returnSpeed * Time.deltaTime);

            if (Quaternion.Angle(rb.rotation, defaultBodyRotation) < 0.01f)
            {
                // Reset the flag when the body rotation is close to the default rotation
                isTilted = false;
            }
        }

        // Ignore rotation on the specific child element
        if (ignoreTransform != null)
        {
            ignoreTransform.rotation = Quaternion.identity;
        }
    }*/
}
