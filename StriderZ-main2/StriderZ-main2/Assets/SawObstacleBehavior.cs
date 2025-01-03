using UnityEngine;

public class SawObstacleBehavior : MonoBehaviour
{
    public Vector3 moveDistance = new Vector3(2f, 0f, 2f); // How much the object should move in each axis
    public float moveSpeed = 2f;                           // Speed at which it moves
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool movingRight = true;

    private void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition + moveDistance;
    }

    private void Update()
    {
        if (movingRight)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, originalPosition) < 0.01f)
            {
                movingRight = true;
            }
        }
    }
}
