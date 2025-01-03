using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleEscelator : MonoBehaviour
{
    [SerializeField] private Animation _moveAnim;

    private const string _handTag = "Hand";
    private void OnTriggerEnter(Collider  other)
    {
        if (other.CompareTag(_handTag))
            _moveAnim.Play();
    }
}
