using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Item
{
    private void Awake()
    {
        itemType = ItemType.Tool; // Set this as a tool
    }

    public override void Use()
    {
        // Handle tool-specific usage, like digging or repairing
        Debug.Log("Using tool!");
    }
}
