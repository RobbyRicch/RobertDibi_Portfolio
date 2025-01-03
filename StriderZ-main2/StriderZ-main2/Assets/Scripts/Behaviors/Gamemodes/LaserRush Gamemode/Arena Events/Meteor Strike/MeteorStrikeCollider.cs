using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorStrikeCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerInputHandler>().Controller.IsAlive = false;
        }
    }
}
