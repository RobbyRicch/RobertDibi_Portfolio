using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewOfficerData", menuName = "Scriptable Objects/Enemy Types/AI Henchman/Officer", order = 3)]
public class AI_SO_Officer : AI_SO_Henchman
{
    [SerializeField] private float _attackDelay = 0.45f;
    public float AttackDelay => _attackDelay;

    [SerializeField] private float _attackTime = 0.15f;
    public float AttackTime => _attackTime;
}
