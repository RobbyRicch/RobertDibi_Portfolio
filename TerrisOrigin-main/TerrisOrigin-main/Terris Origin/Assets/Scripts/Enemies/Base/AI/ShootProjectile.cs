using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _firePoint;
    [SerializeField] Transform _target;
    //[SerializeField] private LayerMask _layerToHit;
    //[SerializeField] private float _attackRange;
    [SerializeField] private float _projectileSpeed = 15f;
    [Range(0, 100)]
    [SerializeField] private int _accuracy = 100;
    [SerializeField] private float _maxAccuracyOffset = 1;
    private float _finalAccuracyOffset;
    [SerializeField] private float _maxFireTime = 1;
    private float _finalFireTime;
    private float _fireTimer;
    private bool _isFiring;

    // Start is called before the first frame update
    void Start()
    {
        _target = GetComponent<EnemyAI>().Player.GetComponent<PlayerInfo>().TargetingPoint;
    }

    void Update()
    {
        if (_isFiring)
        {
            if (_fireTimer >= _finalFireTime)
            {
                ActivateShot();
                _fireTimer = 0;
                _isFiring = false;
            }
            else
            {
                _fireTimer += Time.deltaTime;
            }
        }
        else
        {
            _fireTimer = 0;
        }
    }

    public void Shoot()
    {
        _finalFireTime = Random.Range(0, _maxFireTime);
        _isFiring = true;
    }

    private void ActivateShot()
    {
        SetTargetDestination();
        InstantiateProjectile(_firePoint);
    }

    void SetTargetDestination()
    {
        _finalAccuracyOffset = _maxAccuracyOffset / 100 * (100 - _accuracy);

        Vector3 tempOffset = new Vector3(Random.Range(-_finalAccuracyOffset, _finalAccuracyOffset), 0, 0);
        _firePoint.LookAt(_target.position + tempOffset);
    }

    void InstantiateProjectile(Transform firePoint)
    {
        var projectile = Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
        projectile.transform.SetParent(null);
        projectile.SetActive(true);
        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward * _projectileSpeed;
        projectile.GetComponent<TimeToPoof>().Initiate();
    }
}
