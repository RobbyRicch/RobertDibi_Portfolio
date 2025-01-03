using UnityEngine;

public class RobbyHammers : MonoBehaviour
{
    public float swingSpeed = 45f; // Speed of swinging in degrees per second
    public float swingAngle = 45f; // Angle of swing in degrees
    public bool useSwingDelay = false; // Toggle delay
    public float swingDelay = 1f; // Delay before swinging to the opposite direction (in seconds)

    private Vector3 initialRotation;
    private Vector3 targetRotation;
    private float swingStartTime;
    private bool isSwingingForward = true; // Start swinging forward

    private void Start()
    {
        initialRotation = transform.eulerAngles;
        targetRotation = initialRotation + new Vector3(swingAngle, 0f, 0f);
    }

    private void Update()
    {
        if (isSwingingForward)
        {
            SwingTowardsTarget();
        }
        else
        {
            SwingBackToInitial();
        }
    }

    private void SwingTowardsTarget()
    {
        float t = useSwingDelay ? (Time.time - swingStartTime) / swingDelay : 1f;
        Vector3 newRotation = Vector3.Lerp(initialRotation, targetRotation, t);
        transform.eulerAngles = newRotation;

        if (t >= 1f)
        {
            isSwingingForward = false;
            swingStartTime = Time.time;
        }
    }

    private void SwingBackToInitial()
    {
        float t = (Time.time - swingStartTime) / swingDelay;
        Vector3 newRotation = Vector3.Lerp(targetRotation, initialRotation, t);
        transform.eulerAngles = newRotation;

        if (t >= 1f)
        {
            isSwingingForward = true;
            swingStartTime = Time.time;
        }
    }
}
