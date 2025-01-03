using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    public PuckStatusEffects StatusEffects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInChildren<DeflectorAbility>().CurrentStatusEffect = StatusEffects;
            Destroy(gameObject);
        }
    }
}
