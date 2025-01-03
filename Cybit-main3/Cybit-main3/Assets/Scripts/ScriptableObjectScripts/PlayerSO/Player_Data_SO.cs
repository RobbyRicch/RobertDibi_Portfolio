using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Scriptable Objects/Player/Data", order = 0)]
public class Player_Data_SO : ScriptableObject
{
    #region Health & Stamina
    [Header("Health")]
    [SerializeField] private int _maxHealth = 400;
    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }

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
    [SerializeField] private float _meleeDamage = 35.0f;
    public float MeleeDamage => _meleeDamage;

    [SerializeField] private float _meleeCooldown = 0.0f;
    public float MeleeCooldown => _meleeCooldown;

    [SerializeField] private float _meleeStaminaCost = 20.0f;
    public float MeleeStaminaCost => _meleeStaminaCost;

    [Header("Ultimate")]
    [SerializeField] private ProjectileUlt _ultProjectile;
    public ProjectileUlt UltProjectile { get => _ultProjectile; set => _ultProjectile = value; }

    [SerializeField] private int _ultDamage = 200;
    public int UltDamage { get => _ultDamage; set => _ultDamage = value; }

    [SerializeField] private float _ultSpeed = 20.0f;
    public float UltSpeed { get => _ultSpeed; set => _ultSpeed = value; }

    [SerializeField] private int _maxUltCharge = 180;
    public int MaxUltCharge { get => _maxUltCharge; set => _maxUltCharge = value; }

    [SerializeField] private float _ultChargeCost = 30.0f;
    public float UltChargeCost { get => _ultChargeCost; set => _ultChargeCost = value; }
    #endregion

    #region Focus
    [Header("Focus")]
    [SerializeField] private AudioClip _focusSFX;
    public AudioClip FocusSFX => _focusSFX;

    [SerializeField] private int _maxFocus = 100;
    public int MaxFocus { get => _maxFocus; set => _maxFocus = value; }

    [SerializeField] private float _focusCost = 20.0f;
    public float FocusCost => _focusCost;

    [SerializeField] private float _focusCd = 0.35f;
    public float FocusCd => _focusCd;

    [SerializeField] private float _focusTimeScaleTarget = 0.2f;
    public float FocusTimeScaleTarget => _focusTimeScaleTarget;
    #endregion

    #region VFXs
    [Header("VFXs")]
    [SerializeField] private Color _damageColor = Color.red;
    public Color DamageColor { get => _damageColor; set => _damageColor = value; }
    #endregion
}
