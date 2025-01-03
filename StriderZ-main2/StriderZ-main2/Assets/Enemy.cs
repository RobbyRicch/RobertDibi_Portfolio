using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected float Speed;
    [SerializeField] protected GameObject DeathVFX;
    [SerializeField] protected GameObject ResourcePrefab;
    [SerializeField] protected Transform Target;
}
