using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDragBehind : MonoBehaviour
{
    public Transform playerTransform;
    public float followDelayPosition; // Adjust this to control the delay for position
    public float followDelayRotation; // Adjust this to control the delay for rotation
    public SpriteRenderer spriteRenderer;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private float ogPosDelay;
    private float ogRotDelay;
    private void Start()
    {
        ogPosDelay = followDelayPosition;
        ogRotDelay = followDelayRotation;


    }
    void Update()
    {

        DragFollow();

    }

    private void DragFollow()
    {
        // Calculate target position slightly behind the player
        targetPosition = playerTransform.position - playerTransform.forward * 0.5f;

        // Apply a delay to the follower's movement
        transform.position = Vector3.Lerp(transform.position, targetPosition, followDelayPosition * Time.deltaTime);

        // Calculate target rotation slightly behind the player
        targetRotation = Quaternion.LookRotation(playerTransform.forward, playerTransform.up);

        // Apply a delay to the follower's rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followDelayRotation * Time.deltaTime);

        // Flip the sprite based on horizontal movement
        Vector3 movementDirection = (targetPosition - transform.position).normalized;
        if (movementDirection.x < 0)
        {
            // Sprite should face left
            spriteRenderer.flipX = true;
        }
        else if (movementDirection.x > 0)
        {
            // Sprite should face right
            spriteRenderer.flipX = false;
        }
    }
}


