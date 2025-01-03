using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //PlayerManager.Instance.MoveCurrentPlacementP();
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
