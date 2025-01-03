using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactVFX : MonoBehaviour
{
    [SerializeField] protected float _timeToDestroyImpact;

    private void Start()
    {
        Destroy(gameObject, _timeToDestroyImpact);
    }
}
