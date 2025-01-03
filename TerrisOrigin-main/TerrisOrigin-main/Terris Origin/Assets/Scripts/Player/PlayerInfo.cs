using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private Transform _targetingPoint;
    [SerializeField] private bool _isAttacking;

    public Transform TargetingPoint { get => _targetingPoint; }
    public bool IsAttacking { get => _isAttacking; }

    public void SetAttacking(bool state)
    {
        _isAttacking = state;
    }
}
