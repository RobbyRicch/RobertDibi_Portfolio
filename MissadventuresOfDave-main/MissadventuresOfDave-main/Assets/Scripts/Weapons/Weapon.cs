using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item
{
    public int _dmg;

    private void Awake()
    {
        itemType = ItemType.Weapon; // Set this as a weapon
    }

    public override void Use()
    {
        // Handle weapon-specific usage, like attacking
        Debug.Log("Using weapon!");
    }
}
