using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryBasicAttack : MonoBehaviour
{
    public InputManager InputManager;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] ManaSystem _manaSystem;
    // Fields
    [SerializeField] private GameObject _LightningMuzzleFlash;
    [SerializeField] private GameObject _FireMuzzleFlash;
    private GameObject _muzzleFlash;
    [SerializeField] private GameObject LightningProj;
    [SerializeField] private GameObject FireProj;
    private GameObject _secondaryProjectile;
    //[SerializeField] private List<SerializedDictionaryList> _trails;
    [Space]
    //[SerializeField] private GameObject _projectileMuzzleFlash;
    [SerializeField] private Transform _firePoint;
    [SerializeField] Transform _target;
    [SerializeField] private LayerMask _layerToHit;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _projectileSpeed = 30f;
    private Camera _mainCamera;
    private SphereCollider _sphereCollider;

    [SerializeField] private float _cooldown = 1f;
    private float _timer = 0;

    [SerializeField] private Transform _container;
    [SerializeField] private int _amountInContainer = 5;
    private List<GameObject> _projectilesInContainer = new List<GameObject>();
    [SerializeField] private GameObject ChargeIndicator;

    [SerializeField] private int _manaCost = 10;
    [SerializeField] private bool _fired;
    private PlayerInfo _playerInfo;

    [Header("Secondary Time")]
    [SerializeField] private float TimeToHold = 2;
    private float holdTimer = 0;
    [SerializeField] GameObject HandBall;
    [SerializeField] float SizeToReach;
    private float sizeToTimeCalculation;
    private bool CanShootSecondary = false;
    private bool CanceledShot = false;
    private bool _charging;
    private GameObject _currentProjectile;


    // Start is called before the first frame update
    void Start()
    {
        //ChangeElement();

        _mainCamera = FindObjectOfType<Camera>();
        _timer = _cooldown;

        _playerInfo = GetComponentInParent<PlayerInfo>();

        //PrepareProjectiles();
        sizeToTimeCalculation = SizeToReach / TimeToHold;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.SecondaryBasicHold && _timer >= _cooldown && _manaSystem.CurrentMana >= _manaCost)
        {
            animationManager.CanSecondaryBasic = true;
            if (animationManager.SecondaryBasicAttackP1Loaded)
            {
                if (holdTimer <= TimeToHold)
                {
                    holdTimer += Time.deltaTime;
                    ChargeLightning();
                }
                else
                {
                    ChargeLightning();
                }
            }
            /* ChargeIndicator.SetActive(true);*/
        }
        else if (InputManager.SecondaryBasicHoldCanceled && CanShootSecondary && _manaSystem.Decrease(_manaCost))
        {
            foreach (var trail in _currentProjectile.GetComponent<TrailsHolder>().Trails)
            {
                trail.SetActive(true);
            }
            _currentProjectile.layer = 11;
            Transform[] tempChildren = _currentProjectile.GetComponentsInChildren<Transform>();
            foreach (var item in tempChildren)
            {
                item.gameObject.layer = 11;
            }
            _currentProjectile.GetComponentInChildren<Transform>().gameObject.layer = 11;
            _sphereCollider.enabled = true;
            if (_currentProjectile.TryGetComponent(out RicochetLightning ricochetLightning))
                ricochetLightning.enabled = true;
            holdTimer = 0;
            _charging = false;
            /*ChargeIndicator.SetActive(false);*/
            //HandBall.transform.localScale = Vector3.zero;
            if (animationManager.SecondaryBasicAttackP2Loaded)
            {
                _playerInfo.SetAttacking(true);
                Shoot();
                CanShootSecondary = false;
                SoundManager.Instance.PlaySound(SoundManager.SoundType.LightingBasic);

            }
        }
        else if (InputManager.SecondaryBasicHoldCanceled && _currentProjectile.transform.localScale.x < SizeToReach)
        {
            _currentProjectile.transform.localScale = Vector3.zero;
            _currentProjectile.SetActive(false);
            animationManager.animator.SetBool("SecondaryBasic", false);
            holdTimer = 0;
            _charging = false;
            InputManager.SecondaryBasicHoldCanceled = false;
        }
        else
        {
            _timer += Time.deltaTime;
            InputManager.SecondaryBasicHoldCanceled = false;

        }
    }

    public void Shoot()
    {
        SetTargetDestination();
        InstantiateProjectile(_firePoint);
        _timer = 0;
        _target.position = Vector3.zero;
        StartCoroutine(AttackFalseDelay());
    }

    IEnumerator AttackFalseDelay()
    {
        yield return new WaitForEndOfFrame();
        _playerInfo.SetAttacking(false);
    }

    void SetTargetDestination()
    {
        RaycastHit hit;
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        if (Physics.Raycast(ray, out hit, _attackRange, _layerToHit))
        {
            _target.position = hit.point;
        }
        else
        {
            _target.position = ray.GetPoint(_attackRange);
        }
        _firePoint.LookAt(_target);
    }
    void InstantiateProjectile(Transform firePoint)
    {
        // Muzzle Flash
        /*var mFlash = Instantiate(_muzzleFlash, firePoint.position, firePoint.rotation, firePoint) as GameObject;
        mFlash.SetActive(true);
        Destroy(mFlash, 0.2f);*/

        // Projectile
        _currentProjectile.transform.SetParent(null);
        _currentProjectile.transform.position = _firePoint.position;
        _currentProjectile.transform.rotation = _firePoint.rotation;
        _currentProjectile.SetActive(true);
        _currentProjectile.GetComponent<Rigidbody>().velocity = _currentProjectile.transform.forward * _projectileSpeed;
        if (_currentProjectile.TryGetComponent(out TimeToPoof timeToPoof))
        {
            timeToPoof.Initiate();
        }
    }
    private void PrepareProjectiles()
    {
        for (int i = 0; i < _amountInContainer; i++)
        {
            var projectile = Instantiate(_secondaryProjectile, _firePoint.position, _firePoint.rotation) as GameObject;
            projectile.transform.SetParent(_container);
            projectile.SetActive(false);
            _projectilesInContainer.Add(projectile);
        }
    }

    private void ChargeLightning()
    {
        if (!_charging)
        {
            _charging = true;
            for (int i = 0; i < _amountInContainer; i++)
            {
                if (!_projectilesInContainer[i].activeInHierarchy)
                {
                    _currentProjectile = _projectilesInContainer[i];
                    foreach (var trail in _currentProjectile.GetComponent<TrailsHolder>().Trails)
                    {
                        trail.SetActive(false);
                    }
                    _sphereCollider = _currentProjectile.GetComponent<SphereCollider>();
                    _sphereCollider.enabled = false;
                    if (_currentProjectile.TryGetComponent(out FireBallExplosion fireBallExplosion))
                    {
                        fireBallExplosion.ExplosionArea.SetActive(false);
                    }
                    if (_currentProjectile.TryGetComponent(out RicochetLightning ricochetLightning))
                    {
                        //ricochetLightning.ResetValues();
                        //ricochetLightning.enabled = false;
                    }
                    _currentProjectile.transform.SetParent(_container);
                    _currentProjectile.layer = 8;
                    Transform[] tempChildren = _currentProjectile.GetComponentsInChildren<Transform>();
                    foreach (var item in tempChildren)
                    {
                        item.gameObject.layer = 8;
                    }
                    _currentProjectile.transform.localScale = Vector3.zero;
                    _currentProjectile.transform.position = _firePoint.position;
                    _currentProjectile.transform.rotation = _firePoint.rotation;
                    _currentProjectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    _currentProjectile.SetActive(true);
                }
            }
        }

        if (_currentProjectile.transform.localScale.x < SizeToReach)
        {
            _currentProjectile.transform.position = _firePoint.position;
            _currentProjectile.transform.rotation = _firePoint.rotation;
            _currentProjectile.transform.localScale += new Vector3(sizeToTimeCalculation, sizeToTimeCalculation, sizeToTimeCalculation) * Time.deltaTime;
        }
        else
        {
            _currentProjectile.transform.position = _firePoint.position;
            _currentProjectile.transform.rotation = _firePoint.rotation;
            CanShootSecondary = true;
        }
    }

    public void ChangeElement()
    {
        switch (AbilitiesSelection.Instance.RightHandElement)
        {
            case AbilitiesSelection.ElementType.Lightning:
                _secondaryProjectile = LightningProj;
                //_muzzleFlash = _LightningMuzzleFlash;

                foreach (var item in _projectilesInContainer)
                {
                    Destroy(item.gameObject);
                }
                _projectilesInContainer.Clear();
                PrepareProjectiles();
                break;
            case AbilitiesSelection.ElementType.Fire:
                _secondaryProjectile = FireProj;
                //_muzzleFlash = _FireMuzzleFlash;
                foreach (var item in _projectilesInContainer)
                {
                    Destroy(item.gameObject);
                }
                _projectilesInContainer.Clear();
                PrepareProjectiles();
                break;
            case AbilitiesSelection.ElementType.Light:
                break;
            case AbilitiesSelection.ElementType.Earth:
                break;
            case AbilitiesSelection.ElementType.Darkness:
                break;
            case AbilitiesSelection.ElementType.Water:
                break;
            default:
                break;
        }
    }
}
