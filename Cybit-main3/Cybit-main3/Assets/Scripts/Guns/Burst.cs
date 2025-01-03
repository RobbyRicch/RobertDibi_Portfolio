using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Burst : GunBase
{
    [SerializeField] private GunSO _data;

    [Header("Unique Burst Stats")]
    [SerializeField] private int _numberOfBursts;
    [SerializeField] private float _timeBetweenBursts;

    [Header("Unique Burst States")]
    [SerializeField] private bool _isBursting;


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
        if (!_isReloading && !_isBursting)
        {

            _isBursting = true;
            _isFull = false;
            _isFiring = true;
            _gunAnimator.SetBool("isFiring", true);

            StartCoroutine(BurstCoroutine());
            _isBursting = false;
            _canFire = false;
            ShootCooldown();

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
        else
        {
            _gunAnimator.SetBool("isFiring", false);
            _isFiring = false;
        }

    }

    private IEnumerator BurstCoroutine()
    {
        Vector2 direction = _firePoint.right;
        for (int i = 0; i < _numberOfBursts; i++)
        {
            ProjectileBase firedBullet = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            firedBullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Rigidbody2D bulletRb = firedBullet.GetComponent<Rigidbody2D>();
            float bulletSpeed = _bulletSpeed;
            bulletRb.velocity = direction * bulletSpeed;

            _currentClipSize -= 1;
            _audioSource.pitch = Random.Range(1f, 2f);
            _audioSource.PlayOneShot(_gunShot);
            Destroy(firedBullet, 1.25f);
            yield return new WaitForSeconds(_timeBetweenBursts);
        }

    }
}