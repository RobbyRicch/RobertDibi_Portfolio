using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBusterData", menuName = "Scriptable Objects/Enemy Types/AI Henchman/Buster", order = 5)]
public class AI_SO_Buster : AI_SO_Henchman
{
    [SerializeField] private EnemyProjectileBullet _projectile;
    public EnemyProjectileBullet Projectile => _projectile;

    [SerializeField] private EnemyProjectileBullet _chargedProjectile;
    public EnemyProjectileBullet ChargedProjectile => _chargedProjectile;

    [SerializeField] private Color _chargeAttackColor;
    public Color ChargeAttackColor => _chargeAttackColor;

    [SerializeField] private float _projectileSpeed = 2000.0f;
    public float ProjectileSpeed => _projectileSpeed;

    [SerializeField] private float _chargeAttackFlashDuration = 1.0f;
    public float ChargeAttackFlashDuration => _chargeAttackFlashDuration;

    [SerializeField] private float _chargeAttackCooldown = 1.0f;
    public float ChargeAttackCooldown => _chargeAttackCooldown;
}
