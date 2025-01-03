using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [Header("Hand Transforms")]
    public Transform leftHand;  // Reference to left hand transform
    public Transform rightHand; // Reference to right hand transform
    public Transform crosshair;
    public Transform weaponPos; // Reference to the crosshair

    private Transform _ogLeftHandTransform;
    private Transform _ogRightHandTransform;

    [Header("Item References")]
    public Item leftHandItem;  
    public Item rightHandItem; 
    public bool _holdingRightItem;
    public bool _holdingLeftItem;

    [Header("Switching Hands")]
    [SerializeField] private KeyCode switchKey = KeyCode.Space; 
    [SerializeField] private float maxDistance = 1.0f; 
    [SerializeField] private bool isLeftHandActive = false;
    public bool _rightHandIsFlipped;
    public bool _leftHandIsFlipped;

    [Header("Behavior Switches")]
    [SerializeField] public bool _followCrosshair;

    private void Start()
    {
        _ogLeftHandTransform = leftHand.transform;
        _ogRightHandTransform = rightHand.transform;        
    }

    private void Update()
    {
        // Switch active hand
        if (Input.GetKeyDown(switchKey))
        {
            isLeftHandActive = !isLeftHandActive;
        }

        // Move the active hand towards the cursor
        if (isLeftHandActive && _followCrosshair)
        {
            MoveHandTowards(leftHand, crosshair, maxDistance);
        }
        else if (!isLeftHandActive && _followCrosshair)
        {
            MoveHandTowards(rightHand, crosshair, maxDistance);
        }

        if (!_followCrosshair)
        {
            leftHand.localPosition = new Vector3(-0.73f , -0.39f , 0);
            rightHand.localPosition = new Vector3(0.73f, -0.39f, 0);
        }

        // Handle using items
        if (Input.GetMouseButtonDown(0) && leftHandItem != null)
        {
            UseLeftHandItem();
        }
        else if (Input.GetMouseButtonDown(1) && rightHandItem != null)
        {
            UseRightHandItem();
        }
    }

    // Equip an item to the correct hand based on item type
    public void EquipItem(Item item)
    {
        if (item.itemType == ItemType.Weapon)
        {
            // Equip to the right hand
            rightHandItem = item;
            item.transform.SetParent(rightHand);
            item.transform.localPosition = Vector3.zero; // Set the position to be at the hand
        }
        else if (item.itemType == ItemType.Tool)
        {
            // Equip to the left hand
            leftHandItem = item;
            item.transform.SetParent(leftHand);
            item.transform.localPosition = Vector3.zero; // Set the position to be at the hand
        }
    }

    // Move a hand towards the crosshair but limit its range
    private void MoveHandTowards(Transform hand, Transform target, float maxDistance)
    {
        Vector3 direction = target.position - transform.position; // Get direction from body
        float distance = direction.magnitude;

        if (distance > maxDistance)
        {
            direction = direction.normalized * maxDistance; // Clamp distance
        }

        hand.position = transform.position + direction;

        // Optional: Rotate hand to face crosshair
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        hand.rotation = Quaternion.Euler(0, 0, angle);

        // Call flip method to adjust the hand's rotation if needed
        FlipHand(hand, angle);
    }

    // Flip the hand if it crosses a threshold
    private void FlipHand(Transform hand, float angle)
    {
        // Check if it's the right hand
        if (hand == rightHand)
        {
            if (angle > 90 || angle < -90)
            {
                // Flip the right hand horizontally
                hand.localScale = new Vector3(-Mathf.Abs(hand.localScale.x), hand.localScale.y, hand.localScale.z);

                // Adjust the rotation for flipped state (inverted angle)
                hand.localRotation = Quaternion.Euler(0, 0, -(180 - Mathf.Abs(angle)) * Mathf.Sign(angle));
                _rightHandIsFlipped = true;
            }
            else
            {
                // Reset the right hand scale
                hand.localScale = new Vector3(Mathf.Abs(hand.localScale.x), hand.localScale.y, hand.localScale.z);

                // Adjust the rotation for normal state
                hand.localRotation = Quaternion.Euler(0, 0, angle);
                _rightHandIsFlipped = false;
            }
        }
        else if (hand == leftHand) // Check if it's the left hand
        {
            if (angle > 90 || angle < -90)
            {
                // Flip the left hand horizontally
                hand.localScale = new Vector3(-Mathf.Abs(hand.localScale.x), hand.localScale.y, hand.localScale.z);

                // Adjust the rotation for flipped state (inverted angle)
                hand.localRotation = Quaternion.Euler(0, 0, -(180 - Mathf.Abs(angle)) * Mathf.Sign(angle));
                _leftHandIsFlipped = true;
            }
            else
            {
                // Reset the left hand scale
                hand.localScale = new Vector3(Mathf.Abs(hand.localScale.x), hand.localScale.y, hand.localScale.z);

                // Adjust the rotation for normal state
                hand.localRotation = Quaternion.Euler(0, 0, angle);
                _leftHandIsFlipped = false;
            }
        }
    }
    public void UseLeftHandItem()
    {
        if (leftHandItem != null)
        {
            leftHandItem.Use(); // Call the Use method on the left-hand item
        }
    }

    // Use the right-hand item (e.g., a weapon)
    public void UseRightHandItem()
    {
        if (rightHandItem != null)
        {
            rightHandItem.Use(); // Call the Use method on the right-hand item
        }
    }
}