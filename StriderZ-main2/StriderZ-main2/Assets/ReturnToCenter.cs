using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToCenter : MonoBehaviour
{
    [SerializeField] private Transform _returnPoint;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = _returnPoint.position;
    }
}
