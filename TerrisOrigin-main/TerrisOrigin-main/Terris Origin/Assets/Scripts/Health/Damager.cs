using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Who can get Damaged
enum CanAffect
{
    Player,
    Enemy,
    Both
}

// What kind of Damage
enum DamagerType
{
    OneHit,
    OverTime,
    InstaDeath
}


public class Damager : MonoBehaviour
{
    // Variables
    // --------------------
    [Header("Damage")]
    [SerializeField] private GameObject _usedBy;
    [SerializeField] private CanAffect _canAffect;
    [SerializeField] private DamagerType _damagerType;
    [SerializeField] private int _damageAmount;
    [SerializeField] private float _damageTimeInterval;
    [SerializeField] private AbilitiesSelection.StackedElement Selement;
    [SerializeField] private AbilitiesSelection.ElementType element;

    [Header("Knockback")]
    [SerializeField] private float _knockbackPower = 5;
    [SerializeField] private float _knockbackStunTime = 0.05f;
    [SerializeField] private bool _canKnockback = true;


    // Properties
    // --------------------
    public GameObject UsedBy { get { return _usedBy; } }
    internal CanAffect CanAffect { get { return _canAffect; } }
    internal DamagerType DamagerType { get { return _damagerType; } }
    public int DamageAmount { get => _damageAmount; set => _damageAmount = value; }
    public float DamageTimeInterval { get { return _damageTimeInterval; } }
    public AbilitiesSelection.StackedElement StackingElement { get => Selement; }
    public AbilitiesSelection.ElementType Element { get => element; }

    public float KnockbackPower { get { return _knockbackPower; } }
    public float KnockbackStunTime { get { return _knockbackStunTime; } }
    public bool CanKnockback { get { return _canKnockback; } }
}
