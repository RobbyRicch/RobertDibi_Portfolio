using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunPistol : GunSideArm
{
    [SerializeField] private GunSO _data;

    protected override void Start()
    {
        base.Start();
        _currentClipSize = _maxClipSize;
    }
    public override void InitializeGun()
    {
        _projectilePrefab = _data.BulletPrefab;
        _gunAnimator.runtimeAnimatorController = _data.GunAnimatorController;
        _maxClipSize = _data.MaxClipSize;
        _currentClipSize = _data.CurrentClipSize;
        _fireRate = _data.FireRate;
        _bulletDamage = _data.BulletDamage;
        _bulletSpeed = _data.BulletSpeed;
        _reloadTime = _data.ReloadTime;
        _gunShot = _data.GunShot;
        _reload = _data.Reload;
        _empty = _data.Empty;
        _newWeaponAquiredAC = _data.NewWeaponAudio;
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
        Vector2 direction = _firePoint.right;

        ProjectileBase firedBullet = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firedBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        firedBullet.SetStats(_bulletDamage, _bulletSpeed, direction);

        if (_muzzleFlash != null)
        {
            _muzzleFlash.SetActive(true);
        }

        _currentClipSize -= 1;
        _canFire = false;
        _audioSource.pitch = (Random.Range(0.9f, 3));
        _audioSource.PlayOneShot(_gunShot);
        ShootCooldown();

        if (_currentClipSize <= 0)
        {
            _gunAnimator.SetBool("isFiring", false);
            _canFire = false;
            _isEmpty = true;
            _isFiring = false;
            _audioSource.clip = _empty;
            _audioSource.Play();

            Reload();
            return;
        }
    }

    private IEnumerator _handleFluffVFX(float time)
    {
        _fluffVFX.SetActive(true);
        yield return new WaitForSeconds(time);
        _fluffVFX.SetActive(false);

    }
}
