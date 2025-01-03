using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Profile
{
    #region Meta
    public float PlayTime;
    public float DeathCount;
    public int MaxLevelReachedID;
    #endregion

    #region Player
    public int Id;
    public string Name;
    public int Currency;

    public bool HasFinishedTutorial;
    public bool IsFirstTimeInTraining;
    
    public int MaxIntegrity;
    public float CurrentIntegrity;

    public Vector3 PlayerPos;

    public int MaxHealth;
    public int MoveSpeed;
    public float MeleeDamage;
    public float MeleeTime; // was melee cooldwon
    public float FocusCost;
    public float DodgeChance;
    public float DashDamage;

    public SerializableDictionary<string, int> UnlockableGunIds;

    public string EquippedPrimaryId;
    public string EquippedSideArmId;
    #endregion

    #region PassiveSkill Tree
    #region Passive Skill Tree Speedster
    public float MeleeSpeedFactor;
    public float FocusCostFactor;
    public float DodgeFactor;
    public float DashDistanceFactor;
    public float DashDamageFactor;
    #endregion

    #region Passive Skill Tree Bruiser
    public float MaxHealthFactor;
    public float MeleeDamageFactor;
    public float OverHealFactor;
    public bool IsUndying;
    #endregion

    #region Passive Skill Tree Gunslinger
    public float ReloadTimeFactor;
    public float FireRateFactor;
    public bool IsRicochet;
    public bool IsThirdWeaponSlot;
    #endregion

    #region Passive Skill Tree Combo
    // speedster combo factors
    public float AmmoFactor; // bruiser
    public float ProjectileSlowFactor; //gunslinger 

    // bruiser combo factors
    public float ProjectileChainFactor; // gunslinger
    public float MovementSpeedFactor; // speedster

    // gunslinger combo factors
    public bool IsDashReloading; // speedster
    public bool IsLifeSteal; // bruiser
    #endregion
    #endregion

    public Profile(string name, int id)
    {
        #region Meta
        PlayTime = 0.0f;
        DeathCount = 0;
        MaxLevelReachedID = 0;
        #endregion

        #region Player
        Id = id;
        Name = name;
        Currency = 0;

        HasFinishedTutorial = false;
        IsFirstTimeInTraining = false;

        MaxIntegrity = 250;
        CurrentIntegrity = MaxIntegrity;

        PlayerPos = Vector3.zero;

        MaxHealth = 28;
        MoveSpeed = 1;
        MeleeDamage = 2.0f;
        MeleeTime = 0.0f;
        FocusCost = 20.0f;
        DodgeChance = 0.0f;
        DashDamage = 0.0f;

        UnlockableGunIds = new SerializableDictionary<string, int>();
        EquippedPrimaryId = string.Empty;
        EquippedSideArmId = string.Empty;
        #endregion

        #region Skills
        /* enhancements */
        // speedster
        MeleeSpeedFactor = 1.0f;
        FocusCostFactor = 1.0f;
        DodgeFactor = 1.0f;
        DashDistanceFactor = 1.0f;
        DashDamageFactor = 1.0f;
        AmmoFactor = 1.0f; // combo bruiser
        ProjectileSlowFactor = 1.0f; // combo gunslinger

        // bruiser
        MaxHealthFactor = 1.0f;
        MeleeDamageFactor = 1.0f;
        OverHealFactor = 1.0f;
        IsUndying = false;
        ProjectileChainFactor = 1.0f; // combo gunslinger
        MovementSpeedFactor = 1.0f; // combo speedster

        // gunslinger
        ReloadTimeFactor = 1.0f;
        FireRateFactor = 1.0f;
        IsRicochet = false;
        IsThirdWeaponSlot = false;
        IsDashReloading = false; // combo speedster
        IsLifeSteal = false; // combo bruiser
        #endregion
    }

    public void ResetDataEndRun()
    {
        // progression
        Currency = 0;

        // link integrity system
        MaxIntegrity = 250;
        CurrentIntegrity = MaxIntegrity;

        // player
        PlayerPos = Vector3.zero;
        EquippedPrimaryId = string.Empty;
        EquippedSideArmId = string.Empty;
        MaxHealth = 28;
        MeleeDamage = 2.0f;
        MeleeTime = 0.0f;
        FocusCost = 10.0f;
        DodgeChance = 0.0f;
        DashDamage = 0.0f;

        /* enhancements */
        // speedster
        MeleeSpeedFactor = 1.0f;
        FocusCostFactor = 1.0f;
        DodgeFactor = 1.0f;
        DashDistanceFactor = 1.0f;
        DashDamageFactor = 1.0f;
        AmmoFactor = 1.0f; // combo bruiser
        ProjectileSlowFactor = 1.0f; // combo gunslinger

        // bruiser
        MaxHealthFactor = 1.0f;
        MeleeDamageFactor = 1.0f;
        OverHealFactor = 1.0f;
        IsUndying = false;
        ProjectileChainFactor = 1.0f; // combo gunslinger
        MovementSpeedFactor = 1.0f; // combo speedster

        // gunslinger
        ReloadTimeFactor = 1.0f;
        FireRateFactor = 1.0f;
        IsRicochet = false;
        IsThirdWeaponSlot = false;
        IsDashReloading = false; // combo speedster
        IsLifeSteal = false; // combo bruiser
    }
    public void ResetToTemp()
    {
        #region Meta
        PlayTime = 0.0f;
        DeathCount = 0;
        MaxLevelReachedID = 0;
        #endregion

        #region Player
        Id = -1;
        Name = null;
        Currency = 0;

        HasFinishedTutorial = false;
        IsFirstTimeInTraining = false;

        MaxIntegrity = 250;
        CurrentIntegrity = MaxIntegrity;

        PlayerPos = Vector3.zero;

        MaxHealth = 28;
        MoveSpeed = 1;
        MeleeDamage = 2.0f;
        MeleeTime = 0.0f;
        FocusCost = 20.0f;
        DodgeChance = 0.0f;
        DashDamage = 0.0f;

        UnlockableGunIds = new SerializableDictionary<string, int>();
        EquippedPrimaryId = null;
        EquippedSideArmId = null;
        #endregion

        #region Skills
        /* enhancements */
        // speedster
        MeleeSpeedFactor = 1.0f;
        FocusCostFactor = 1.0f;
        DodgeFactor = 1.0f;
        DashDistanceFactor = 1.0f;
        DashDamageFactor = 1.0f;
        AmmoFactor = 1.0f; // combo bruiser
        ProjectileSlowFactor = 1.0f; // combo gunslinger

        // bruiser
        MaxHealthFactor = 1.0f;
        MeleeDamageFactor = 1.0f;
        OverHealFactor = 1.0f;
        IsUndying = false;
        ProjectileChainFactor = 1.0f; // combo gunslinger
        MovementSpeedFactor = 1.0f; // combo speedster

        // gunslinger
        ReloadTimeFactor = 1.0f;
        FireRateFactor = 1.0f;
        IsRicochet = false;
        IsThirdWeaponSlot = false;
        IsDashReloading = false; // combo speedster
        IsLifeSteal = false; // combo bruiser
        #endregion
    }
}
