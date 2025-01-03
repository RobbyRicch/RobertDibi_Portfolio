using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBossData", menuName = "Scriptable Objects/Enemy Types/Bosses/Boss", order = 6)]
public class AI_SO_Boss : AI_SO
{
    [SerializeField] private AudioClip _basicAttackAC;
    public AudioClip BasicAttackAC => _basicAttackAC;

    [SerializeField] private AudioClip _specialAttackAC;
    public AudioClip SpecialAttackAC => _specialAttackAC;

    [SerializeField] private EnemyProjectileBase _projectile;
    public EnemyProjectileBase Projectile => _projectile;

    [SerializeField] private float _projectileSpeed = 2000.0f;
    public float ProjectileSpeed => _projectileSpeed;
}
