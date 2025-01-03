using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterObstacleBehaviour : MonoBehaviour
{
    public float spinSpeed = 10f;
    public bool rotateOnXAxis = false; // Boolean to determine rotation axis
    public bool limitRotation = false; // Boolean to determine if rotation is limited
    public float maxRotationAngle = 180f; // Maximum rotation angle if limitRotation is true
    public bool useDelay = false; // Boolean to determine if delay is used
    public float loopDelay = 1f; // Delay duration between each loop rotation
    public float currentRotationAngle = 0f; // Current rotation angle
    public float targetRotationAngle = 0f; // Target rotation angle
    public bool rotateBack = false; // Boolean to determine if rotating back to original rotation
    public Quaternion originalRotation; // Original rotation
    public bool isDelaying = false; // Boolean to track delay status
    public float delayTimer = 0f; // Timer for delay

    private void Start()
    {
        originalRotation = transform.rotation; // Store the original rotation
    }

    void FixedUpdate()
    {
        if (isDelaying)
        {
            delayTimer -= Time.fixedDeltaTime;

            if (delayTimer <= 0f)
            {
                isDelaying = false;
                rotateBack = !rotateBack;
                if (rotateBack)
                {
                    targetRotationAngle = 0f;
                }
                else
                {
                    targetRotationAngle = limitRotation ? maxRotationAngle : 360f;
                }
            }
        }
        else
        {
            if (rotateOnXAxis)
            {
                if (rotateBack)
                {
                    if (currentRotationAngle > targetRotationAngle)
                    {
                        transform.Rotate(Vector3.right * -spinSpeed * Time.fixedDeltaTime);
                        currentRotationAngle -= spinSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        if (useDelay)
                        {
                            StartDelayTimer();
                        }
                        else
                        {
                            rotateBack = false;
                            targetRotationAngle = limitRotation ? maxRotationAngle : 360f;
                        }
                    }
                }
                else
                {
                    if (currentRotationAngle < targetRotationAngle)
                    {
                        transform.Rotate(Vector3.right * spinSpeed * Time.fixedDeltaTime);
                        currentRotationAngle += spinSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        if (useDelay)
                        {
                            StartDelayTimer();
                        }
                        else
                        {
/*                            rotateBack = true;
*/                            targetRotationAngle = 0f;
                        }
                    }
                }
            }
            else
            {
                if (rotateBack)
                {
                    if (currentRotationAngle > targetRotationAngle)
                    {
                        transform.Rotate(Vector3.up * -spinSpeed * Time.fixedDeltaTime);
                        currentRotationAngle -= spinSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        if (useDelay)
                        {
                            StartDelayTimer();
                        }
                        else
                        {
                            rotateBack = false;
                            targetRotationAngle = limitRotation ? maxRotationAngle : 360f;
                        }
                    }
                }
                else
                {
                    if (currentRotationAngle < targetRotationAngle)
                    {
                        transform.Rotate(Vector3.up * spinSpeed * Time.fixedDeltaTime);
                        currentRotationAngle += spinSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        if (useDelay)
                        {
                            StartDelayTimer();
                        }
                        else
                        {
                            rotateBack = true;
                            targetRotationAngle = 0f;
                        }
                    }
                }
            }
        }
    }

    private void StartDelayTimer()
    {
        isDelaying = true;
        delayTimer = loopDelay;
    }
}