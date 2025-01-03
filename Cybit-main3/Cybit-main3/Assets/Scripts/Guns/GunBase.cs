using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunBase : MonoBehaviour, IProfileSaveable
{
    [SerializeField] protected string _uid = "";
    public string Uid => _uid;

    [SerializeField] protected int _tier = 1;
    public int Tier => _tier;

    #region Components
    [Header("Components")]
    public GunBase[] GunTiers;
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected AudioSource _audioSource;
    [SerializeField] protected GameObject _reloadCanvas;
    [SerializeField] protected UnityEngine.UI.Image _reloadImgFill;

    [SerializeField] protected Animator _gunAnimator;
    public Animator GunAnimator => _gunAnimator;

    [SerializeField] protected SpriteRenderer _sR;
    public SpriteRenderer SR => _sR;

    [SerializeField] protected GunPickup _pickup;
    public GunPickup Pickup => _pickup;

    protected ProjectileBase _projectilePrefab;
    #endregion

    #region VFX
    [Header("VFX")]
    [SerializeField] protected GameObject _highlightVFX;
    public GameObject HighlightVFX => _highlightVFX;

    [SerializeField] protected GameObject _fluffVFX;
    public GameObject FluffVFX => _fluffVFX;

    protected GameObject _muzzleFlash;
    #endregion

    #region Audio
    [Header("Audio")]
    protected AudioClip _gunShot, _reload, _empty;

    protected AudioClip[] _newWeaponAquiredAC;
    #endregion

    #region Stats
    [Header("Stats")]
    [SerializeField] protected GunType _slotType;
    public GunType SlotType => _slotType;

    protected string _name;
    protected float _fireRate;
    protected int _bulletDamage;
    protected float _bulletSpeed;
    protected float _reloadTime;
    
    protected int _currentClipSize;
    public int CurrentClipSize => _currentClipSize;

    protected int _maxClipSize;
    public int MaxClipSize => _maxClipSize;
    #endregion

    #region States
    [Header("States")]
    [SerializeField] protected bool _isOnFloor = true;
    public bool IsOnFloor { get => _isOnFloor; set => _isOnFloor = value; }

    [SerializeField] protected bool _isEquipped = false;
    public bool IsEquipped { get => _isEquipped; set => _isEquipped = value; }
    
    [SerializeField] protected bool _canFire = true;
    public bool CanFire { get => _canFire; set => _canFire = value; }

    [SerializeField] protected bool _isFull = true;
    public bool IsFull { get => _isFull; set => _isFull = value; }

    [SerializeField] protected bool _isFiring = false;
    public bool IsFiring { get => _isFiring; set => _isFiring = value; }
    
    [SerializeField] protected bool _isEmpty = false;
    [SerializeField] protected bool _isReloading = false;
    #endregion

    protected virtual void Start()
    {
        InitializeGun();
        _isFull = true;
    }

    public IEnumerator FadeOutWeapon()
    {
        float fadeDuration = 1f;

        Color startColor = _sR.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeDuration);
            _sR.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the sprite is fully transparent at the end
        _sR.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
    }
    public IEnumerator FadeInWeapon()
    {
        float fadeDuration = 1f;

        Color startColor = _sR.color;
        float startAlpha = startColor.a; // Store the initial alpha value
        float targetAlpha = 1f; // Target opacity to fade to
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration); // Interpolating from initial alpha to target alpha
            _sR.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the sprite is fully opaque at the end
        _sR.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }

    public abstract void InitializeGun();
    public abstract void Shoot();

    public virtual IEnumerator DoReloadAnimation()
    {
        float elapsedTime = 0f;
        float originalSize = 0.0f;
        float targetSize = 1.0f;

        _reloadCanvas.SetActive(true);
        while (elapsedTime < _reloadTime)
        {
            _reloadImgFill.fillAmount = Mathf.Lerp(originalSize, targetSize, elapsedTime / _reloadTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _reloadImgFill.fillAmount = targetSize;
        _reloadCanvas.SetActive(false);
    }
    public virtual void Equip()
    {
        _isEquipped = true;
        _canFire = true;
    }

    protected virtual void Load(Profile data) { }

    public void SaveData(ref Profile data)
    {
        
    }
    public void LoadData(Profile data)
    {
        Load(data);
    }
}