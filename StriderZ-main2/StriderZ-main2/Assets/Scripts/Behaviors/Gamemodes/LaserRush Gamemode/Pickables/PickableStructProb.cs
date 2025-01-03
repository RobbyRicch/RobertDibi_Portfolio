using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct PickableStructProb
{
    [Range(0f, 0.7f)]
    public float Probability;
    public GameObject PickupObj;
}
