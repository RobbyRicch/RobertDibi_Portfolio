using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralGun : GunBase
{
    [SerializeField] private GunSO _data;

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

        _muzzleFlash = _data.MuzzleFlash;
        _fluffVFX = _data.FluffVFX;
    }

    private IEnumerator DoShootCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        _gunAnimator.SetBool("isFiring", false);

        _canFire = false;
        yield return new WaitForSeconds(_fireRate);
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
        yield return new WaitForSeconds(_reloadTime);
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
        else if (_currentClipSize < 1)
        {
            Reload();
            return;
        }

        _isFull = false;
        _isFiring = true;
        _gunAnimator.SetBool("isFiring", true);

        Vector2 direction = _firePoint.right;

        ProjectileBase firedBullet = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firedBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Rigidbody2D bulletRb = firedBullet.GetComponent<Rigidbody2D>();

        if (_muzzleFlash != null)
            _muzzleFlash.SetActive(true);

        _currentClipSize -= 1;
        float bulletSpeed = _bulletSpeed;
        bulletRb.velocity = direction * bulletSpeed;
        _canFire = false;
        _audioSource.pitch = (Random.Range(0.9f, 3));
        _audioSource.PlayOneShot(_gunShot);
        ShootCooldown();
        Destroy(firedBullet, 1.25f);

        if (_currentClipSize <= 0)
        {
            _canFire = false;
            _isEmpty = true;
            _isFiring = false;
            _gunAnimator.SetBool("isFiring", false);
            _audioSource.clip = _empty;
            _audioSource.Play();
        }
    }

}
