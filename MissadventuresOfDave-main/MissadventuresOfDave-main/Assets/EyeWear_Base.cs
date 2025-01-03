using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeWear_Base : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _currentTrigger;
    [SerializeField] private BoxCollider2D _collider;
    [SerializeField] private Rigidbody2D _currentRb;

    private Transform originalParent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dave"))
        {
            // Get the player's reference
            Dave_Controller daveRef = collision.GetComponent<Dave_Controller>();

            if (daveRef._currentEyewear == null)
            {
                // Parent the hat to the pivot
                originalParent = transform.parent; // Store original parent (if needed)
                transform.SetParent(daveRef._eyewearPivot);

                // Align the hat
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;

                // Disable colliders and assign the current hat
                _currentTrigger.enabled = false;
                _collider.enabled = false;
                _currentRb.bodyType = RigidbodyType2D.Static;
                // Assign this hat to the player's current hat reference
                daveRef._currentEyewear = this;

            }
        }
    }

    private void LateUpdate()
    {
        // Explicitly follow the parent in case of animation or hierarchy issues
        if (transform.parent != null)
        {
            transform.position = transform.parent.position;
            transform.rotation = transform.parent.rotation;
        }
    }
}
