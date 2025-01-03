using UnityEngine;

public class RobbyMenuCamera : MonoBehaviour
{
    public Transform targetTransform;

    public void MoveToTarget()
    {
        if (targetTransform != null)
        {
            transform.position = targetTransform.position;
            transform.rotation = targetTransform.rotation;
        }
    }
}