using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEntry : MonoBehaviour
{
    [SerializeField] private Portal _portalParent;

    private void OnTriggerEnter(Collider target)
    {
        if (target.tag == "Player")
        {
            _portalParent.Teleport(target.gameObject);
        }
    }
}
