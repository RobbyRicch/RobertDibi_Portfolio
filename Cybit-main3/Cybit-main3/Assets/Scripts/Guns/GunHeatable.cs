using System.Collections;
using UnityEngine;

public class GunHeatable : GunPrimary
{
    [Header("SO Reference")]
    [SerializeField] protected GunSO _data;

    [Header("Unique Heatable States")]
    [SerializeField] protected float _chargeTime;
    [SerializeField] protected bool _isOverheated;
    [SerializeField] protected bool _isCharging;
    [SerializeField] protected bool _isCharged;
    protected int _chargeCounter = 0;

    [SerializeField] private float _shakeTime = 0.015f;
    [SerializeField] private float _shakeIntensity = 0.6f;

    private CinemachineShake _cameraShake = null;
    public CinemachineShake CameraShake { get => _cameraShake; set => _cameraShake = value; }

    protected bool _isStartingFire = true;
    public bool IsStartingFire { get => _isStartingFire; set => _isStartingFire = value; }

    protected virtual void Update()
    {
        CheckFluffVFX();
    }

    protected void CheckFluffVFX()
    {
        if (_isFiring && _isStartingFire)
        {
            ParticleSystem particleSystem = _fluffVFX.GetComponent<ParticleSystem>();
            //particleSystem.Play();
            _fluffVFX.gameObject.SetActive(true);
            _isStartingFire = false;
        }
    }

    public override void InitializeGun()
    {
        _projectilePrefab = _data.BulletPrefab;
        _gunAnimator.runtimeAnimatorController = _data.GunAnimatorController;
        _maxClipSize = _data.MaxClipSize; //we are gonna declare clip size as heat in this script 
        _currentClipSize = _data.CurrentClipSize; //we are gonna declare clip size as heat in this script 
        _fireRate = _data.FireRate;
        _bulletDamage = _data.BulletDamage;
        _bulletSpeed = _data.BulletSpeed;
        _reloadTime = _data.ReloadTime;
        _gunShot = _data.GunShot;
        _reload = _data.Reload;
        _empty = _data.Empty;
    }

    protected IEnumerator DoShootCooldown()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        _canFire = false;
        yield return new WaitForSecondsRealtime(_fireRate);
        _canFire = true;
    }
    protected void ShootCooldown()
    {
        StartCoroutine(DoShootCooldown());
    }
    protected virtual IEnumerator DoReload()
    {
        _isReloading = true;
        _audioSource.clip = _reload;
        _audioSource.Play();

        yield return StartCoroutine(DoReloadAnimation());

        _currentClipSize = 0;
        _isReloading = false;
        _isOverheated = false;
        _gunAnimator.SetBool("isFiring", false);
        _isFiring = false;

        if (_fluffVFX)
        {
            /*ParticleSystem particleSystem = _fluffVFX.GetComponent<ParticleSystem>();
            particleSystem.Stop();*/
            _fluffVFX.gameObject.SetActive(false);
        }
        
        _isStartingFire = true;
    }
    protected IEnumerator BeginChargeUp()
    {
        _isCharging = true;
        _gunAnimator.SetBool("IsCharging", true);
        yield return new WaitForSeconds(_chargeTime);
        _isCharging = false;
        _isCharged = true;
        FluffVFX.SetActive(true);
        _chargeCounter = 0;
    }
    protected void Reload()
    {
        StartCoroutine(DoReload());
    }

    public override void Shoot()
    {
        if (!_isCharged && _chargeCounter > 0)
        {
            return;
        }
        else if (!_isCharged && !_isOverheated && _chargeCounter < 1)
        {
            _chargeCounter++;
            StartCoroutine(BeginChargeUp());
        }
        else if (_isCharged && !_isOverheated)
        {
            _isFiring = true;

            _cameraShake.StartCameraShake(_shakeTime, _shakeIntensity);
            _gunAnimator.SetBool("IsCharging", false);
            _gunAnimator.SetBool("isFiring", true);

            Vector2 direction = _firePoint.right;

            if (_muzzleFlash)
                _muzzleFlash.SetActive(true);

            ProjectileBase firedBullet = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            firedBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            firedBullet.SetStats(_bulletDamage, _bulletSpeed, direction);

            _currentClipSize++;
            _canFire = false;
            _audioSource.pitch = (Random.Range(1, 2));
            _audioSource.PlayOneShot(_gunShot);
            ShootCooldown();
            Destroy(firedBullet, 1.25f);
            if (_currentClipSize >= _maxClipSize)
            {
                _canFire = false;
                _isOverheated = true;
                _isFiring = false;
                _gunAnimator.SetBool("isFiring", false);
                _gunAnimator.SetBool("IsCharging", false);
                _audioSource.clip = _empty;
                _audioSource.Play();
                Reload();
            }
        }
        /*else
        {
            _gunAnimator.SetBool("isFiring", false);
            _isFiring = false;
            ParticleSystem particleSystem = _fluffVFX.GetComponent<ParticleSystem>();
            particleSystem.Stop();
            _fluffVFX.gameObject.SetActive(false);
            _isStartingFire = true;
        }*/
    }
}
