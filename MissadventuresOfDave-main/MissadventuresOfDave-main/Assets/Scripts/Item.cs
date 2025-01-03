using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Weapon , Tool }
public abstract class Item : MonoBehaviour
{
    public ItemType itemType;
    public abstract void Use();
}
