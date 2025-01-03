using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MVC Model
public class Player_Data : MonoBehaviour, IProfileSaveable
{
    #region Progression
    [Header("Progression")]
    [SerializeField] private int _currentCheckpointID = 0;
    public int CurrentCheckpointID { get => _currentCheckpointID; set => _currentCheckpointID = value; }

    [SerializeField] private int _currency = 0;
    public int Currency { get => _currency; set => _currency = value; }
    #endregion

    #region Health & Stamina
    [Header("Health")]
    [SerializeField] private int _maxHealth = 28;
    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    [SerializeField] private int _originalMaxHealth = 28;
    public int OriginalMaxHealth => _originalMaxHealth;

    [Header("Stamina")]
    [SerializeField] private int _maxStamina = 150;
    public int MaxStamina { get => _maxStamina; set => _maxStamina = value; }

    [SerializeField] private float _staminaRegenRate = 40.0f;
    public float StaminaRegenRate { get => _staminaRegenRate; set => _staminaRegenRate = value; }

    [SerializeField] private float _staminaRegenDelay = 0.01f;
    public float StaminaRegenDelay { get => _staminaRegenDelay; set => _staminaRegenDelay = value; }

    [SerializeField] private float _staminaRegenTime = 100.0f;
    public float StaminaRegenTime { get => _staminaRegenTime; set => _staminaRegenTime = value; }
    #endregion

    #region Attacks
    [Header("Melee")]
    [SerializeField] private float _meleeDamage = 2.0f;
    public float MeleeDamage { get => _meleeDamage; set => _meleeDamage = value; }

    [SerializeField] private float _originalMeleeDamage = 2.0f;
    public float OriginalMeleeDamage => _originalMeleeDamage;

    [SerializeField] private float _meleeCooldown = 0.0f;
    public float MeleeCooldown => _meleeCooldown;

    [SerializeField] private float _meleeStaminaCost = 20.0f;
    public float MeleeStaminaCost => _meleeStaminaCost;
    #endregion

    #region VFXs
    [Header("VFXs")]
    [SerializeField] private Color _damageColor = Color.red;
    public Color DamageColor { get => _damageColor; set => _damageColor = value; }
    #endregion

    [SerializeField] private float _maxHealthFactor = 1.0f;
    public float MaxHealthFactor { get => _maxHealthFactor; set => _maxHealthFactor = value; }

    [SerializeField] private float _meleeDamageFactor = 1.0f;
    public float MeleeDamageFactor { get => _meleeDamageFactor; set => _meleeDamageFactor = value; }

    [SerializeField] private float _focusCostFactor = 1.0f;
    public float FocusCostFactor { get => _focusCostFactor; set => _focusCostFactor = value; }

    private string _equippedPrimaryId;
    public string EquippedPrimaryId { get => _equippedPrimaryId; set => _equippedPrimaryId = value; }

    private string _equippedSideArmId;
    public string EquippedSideArmId { get => _equippedSideArmId; set => _equippedSideArmId = value; }

    private SerializableDictionary<string, int> _unlockableGunIds = new();
    public SerializableDictionary<string, int> UnlockableGunIds { get => _unlockableGunIds; set => _unlockableGunIds = value; }

    public void SaveData(ref Profile data)
    {
        data.Currency = _currency;
        data.UnlockableGunIds = _unlockableGunIds;

        data.EquippedPrimaryId = _equippedPrimaryId;
        data.EquippedSideArmId = _equippedSideArmId;
        data.MaxHealth = _maxHealth;
        data.MeleeDamage = _meleeDamage;

        data.MaxHealthFactor = _maxHealthFactor;
        data.MeleeDamageFactor = _meleeDamageFactor;
        data.FocusCostFactor = _focusCostFactor;
    }
    public void LoadData(Profile data)
    {
        _currency = data.Currency;
        _maxHealth = data.MaxHealth;
        _meleeDamage = data.MeleeDamage;
        _equippedPrimaryId = data.EquippedPrimaryId;
        _equippedSideArmId = data.EquippedSideArmId;
        _unlockableGunIds = data.UnlockableGunIds;
        _maxHealthFactor = data.MaxHealthFactor;
        _meleeDamageFactor = data.MeleeDamageFactor;
        _focusCostFactor = data.FocusCostFactor;
    }
}
