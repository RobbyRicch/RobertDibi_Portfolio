using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunPickup : MonoBehaviour
{
    public StunBank StunBankRef;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (StunBankRef.availablePickups < StunBankRef.maxPickups)
            {
                StunBankRef.availablePickups++;
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("Player has Full capacity of : Stuns ");
        }
    }
}
