using JetBrains.Annotations;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Scriptable Objects/Enemy Types/AI Base", order = 1)]
public class AI_SO : ScriptableObject
{

    [Foldout("Audio Clips")]
    [SerializeField] private AudioClip _spawnAC = null;
    public AudioClip SpawnAC => _spawnAC;

    [Foldout("Audio Clips")]
    [SerializeField] private AudioClip _idleAC = null;
    public AudioClip IdleAC => _idleAC;

    [Foldout("Audio Clips")]
    [SerializeField] private AudioClip _hurtAC = null;
    public AudioClip HurtAC => _hurtAC;

    [Foldout("Audio Clips")]
    [SerializeField] private AudioClip _deathAC = null;
    public AudioClip DeathAC => _deathAC;

    [Foldout("Audio Clips")]
    [SerializeField] private AudioClip _endDeathAC = null;
    public AudioClip EndDeathAC => _endDeathAC;

    [Foldout("Audio Clips")]
    [SerializeField] private AudioClip _itemDropAC = null;
    public AudioClip ItemDropAC => _itemDropAC;

    [Foldout("Stats")]
    [SerializeField] private Color _damageColor = new();
    public Color DamageColor => _damageColor;

    [Foldout("Stats")]
    [SerializeField] private int _maxHealth = 1;
    public int MaxHealth => _maxHealth;

    [Foldout("Stats")]
    [SerializeField] private float _moveSpeed = 3.0f;
    public float MoveSpeed => _moveSpeed;

    [Foldout("Stats")]
    [SerializeField] private int _attackDamage = 1;
    public int AttackDamage => _attackDamage;

    [Foldout("Stats")]
    [SerializeField] private float _attackRange = 2.0f;
    public float AttackRange => _attackRange;

    [Foldout("Stats")]
    [SerializeField] private float _timeBetweenAttacks = 0.4f;
    public float TimeBetweenAttacks => _timeBetweenAttacks;

    [Foldout("Stats")]
    [SerializeField] private float _knockBackTime = 0.15f;
    public float KnockBackTime => _knockBackTime;

    [Foldout("Rewards")]
    [SerializeField] private float _focusToGivePlayer = 10.0f;
    public float FocusToGivePlayer => _focusToGivePlayer;

    [Foldout("Rewards")]
    [SerializeField] private int _currencyToGivePlayer = 5;
    public int CurrencyToGivePlayer => _currencyToGivePlayer;
}

