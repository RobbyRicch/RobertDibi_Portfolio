using System.Collections;
using UnityEngine;

public class GunShotgun : GunPrimary
{
    [Header("SO Reference")]
    [SerializeField] private GunSO _data;

    [Header("Unique Shotgun Stats")]
    [SerializeField] private int _peletCount;
    [SerializeField] private float _shotgunSpreadAngle;

    [SerializeField] private float _shakeTime = 0.1f;
    [SerializeField] private float _shakeIntensity = 1.0f;

    private CinemachineShake _cameraShake = null;
    public CinemachineShake CameraShake { get => _cameraShake; set => _cameraShake = value; }

    protected override void Start()
    {
        base.Start();
        _currentClipSize = _maxClipSize;
    }
    private void Update()
    {
        if (!IsFiring)
        {
            _gunAnimator.SetBool("isFiring", false);
        }
    }
    public override void InitializeGun()
    {
        _projectilePrefab = _data.BulletPrefab;
        _maxClipSize = _data.MaxClipSize;
        _currentClipSize = _data.CurrentClipSize;
        _fireRate = _data.FireRate;
        _bulletDamage = _data.BulletDamage;
        _bulletSpeed = _data.BulletSpeed;
        _reloadTime = _data.ReloadTime;
        _gunShot = _data.GunShot;
        _reload = _data.Reload;
        _empty = _data.Empty;
        _muzzleFlash = _data.MuzzleFlash;
/*        _fluffVFX = _data.FluffVFX;
*/    }

    private IEnumerator DoShootCooldown()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        _gunAnimator.SetBool("isFiring", false);

        _canFire = false;
        yield return new WaitForSecondsRealtime(_fireRate);
        _canFire = true;
    }
    private void ShootCooldown()
    {
        StartCoroutine(DoShootCooldown());
    }

    private IEnumerator DoReload()
    {
        _isReloading = true;
        _audioSource.clip = _reload;
        _audioSource.Play();

        yield return StartCoroutine(DoReloadAnimation());

        _currentClipSize = _maxClipSize;
        _isReloading = false;
    }
    private void Reload()
    {
        StartCoroutine(DoReload());
    }

    public override void Shoot()
    {
        if (_isReloading)
        {
            _gunAnimator.SetBool("isFiring", false);
            _isFiring = false;
            return;
        }

        _isFull = false;
        _isFiring = true;
        _gunAnimator.SetBool("isFiring", true);
        StartCoroutine(_handleFluffVFX(1));
        _cameraShake.StartCameraShake(_shakeTime, _shakeIntensity);
        Vector2 forwardDirection = _firePoint.transform.right;

        // Calculate the angle between each pellet
        float angleBetweenPellets = _shotgunSpreadAngle / (_peletCount - 1);

        for (int i = 0; i < _peletCount; i++)
        {
            // Calculate the angle for this pellet
            float pelletAngle = -_shotgunSpreadAngle / 2f + i * angleBetweenPellets;

            // Rotate the forward direction by the pellet angle
            Quaternion spreadRotation = Quaternion.AngleAxis(pelletAngle, Vector3.forward);
            Vector2 direction = spreadRotation * forwardDirection;

            // Instantiate the bullet and set its rotation
            ProjectileBase firedBullet = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            firedBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            firedBullet.SetStats(_bulletDamage, _bulletSpeed, direction);
        }

        _currentClipSize -= 1;
        _audioSource.pitch = Random.Range(0.9f, 3);

        _audioSource.PlayOneShot(_gunShot);
        _canFire = false;
        ShootCooldown();

        if (_currentClipSize < 1)
        {
            _canFire = false;
            _isEmpty = true;
            _isFiring = false;
            _gunAnimator.SetBool("isFiring", false);
            _audioSource.clip = _empty;
            _audioSource.Play();
            Reload();
        }
    }

    private IEnumerator _handleFluffVFX(float time)
    {
        _fluffVFX.SetActive(true);
        yield return new WaitForSeconds(time);
        _fluffVFX.SetActive(false);

    }
}

