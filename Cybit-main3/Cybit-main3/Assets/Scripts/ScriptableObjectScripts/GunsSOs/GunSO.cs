using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseGun", menuName = "Scriptable Objects/Weapon Types/Base Gun", order = 0)]

public class GunSO : ScriptableObject
{
    #region Stats
    [Header("Stats")]
    [SerializeField] private GunType _slotType;
    public GunType SlotType => _slotType;

    [SerializeField] private int _bulletDamage;
    public int BulletDamage => _bulletDamage;

    [SerializeField] private float _bulletSpeed;
    public float BulletSpeed => _bulletSpeed;

    [SerializeField] private int _maxClipSize;
    public int MaxClipSize => _maxClipSize;

    [SerializeField] private int _currentClipSize;
    public int CurrentClipSize => _currentClipSize;

    [SerializeField] private float _fireRate;
    public float FireRate => _fireRate;

    [SerializeField] private float _reloadTime;
    public float ReloadTime => _reloadTime;

    #endregion

    #region Refrences
    [Header("Refrences")]
    [SerializeField] private ProjectileBase _bulletPrefab;
    public ProjectileBase BulletPrefab => _bulletPrefab;

    [SerializeField] private RuntimeAnimatorController _gunAnimator;
    public RuntimeAnimatorController GunAnimatorController => _gunAnimator;

    [SerializeField] protected AudioClip _gunShot;
    public AudioClip GunShot => _gunShot;

    [SerializeField] protected AudioClip _reload;
    public AudioClip Reload => _reload;

    [SerializeField] protected AudioClip _empty;
    public AudioClip Empty => _empty;

    [Header("Optional VFX")]
    [SerializeField] private GameObject _muzzleFlash;
    public GameObject MuzzleFlash => _muzzleFlash;

    [SerializeField] private GameObject _fluffVFX;
    public GameObject FluffVFX => _fluffVFX;

    [SerializeField] private AudioClip[] _newWeaponAudio;

    public AudioClip[] NewWeaponAudio => _newWeaponAudio;
    #endregion
}
