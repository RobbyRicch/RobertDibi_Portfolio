using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Creature_Inventory : MonoBehaviour
{
    [Header("Player Inventory Items")]
    [SerializeField] public int slimesInInventory;
    [SerializeField] public CreatureBase _currentCreature;

    public void FeedSlime()
    {
        if (slimesInInventory > 0)
        {
            _currentCreature._slimesFed++;
            slimesInInventory--;
        }

    }
}
