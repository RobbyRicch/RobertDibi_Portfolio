using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicAttack : MonoBehaviour
{
    public InputManager InputManager;
    [SerializeField] AnimationManager animationManager;
    [SerializeField] private StaminaSystem _staminaSystem;
    // Fields
    [SerializeField] private GameObject _LightningMuzzleFlash;
    [SerializeField] private GameObject _FireMuzzleFlash;
    private GameObject _muzzleFlash;

    [Header("Projectiles")]
    [SerializeField] private GameObject LightningProj;
    [SerializeField] private GameObject FireProj;
    [Header("Proj Feedbacks Effects")]
    [SerializeField] private GameObject LightningProjFeedback;
    [SerializeField] private GameObject FireProjFeedback;

    private GameObject _basicProjectile;
    public GameObject _basicProjectileFeedback;
    //[SerializeField] private GameObject _projectileMuzzleFlash;
    [SerializeField] private Transform _firePoint;
    [SerializeField] Transform _target;
    [SerializeField] private LayerMask _layerToHit;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _projectileSpeed = 30f;
    private Camera _mainCamera;

    [SerializeField] private float _cooldown = 1f;
    private float _timer = 0;

    [SerializeField] private Transform _container;
    [SerializeField] private int _amountInContainer = 5;
    private List<GameObject> _projectilesInContainer = new List<GameObject>();

    [SerializeField] private int _StaminaCost = 10;
    [SerializeField] private bool _fired;
    private PlayerInfo _playerInfo;

    public bool IsShooting = false;
    [SerializeField] private GameObject SphereCastObj;
    //[SerializeField] private AnimationCurve curve;

    // Start is called before the first frame update
    void Start()
    {
        //ChangeElement();

        _mainCamera = FindObjectOfType<Camera>();
        _timer = _cooldown;

        _playerInfo = GetComponentInParent<PlayerInfo>();

        //PrepareProjectiles();
    }

    // Update is called once per frame
    void Update()
    {
        if ((InputManager.AttackPressed || InputManager.AttackHold) && _timer >= _cooldown)
        {
            animationManager.CanBasic = true;
            if (animationManager.BasicAttackLoaded && _staminaSystem.Decrease(_StaminaCost))
            {
                _playerInfo.SetAttacking(true);
                Shoot();
                SoundManager.Instance.PlaySound(SoundManager.SoundType.FireBasic);
                SoundManager.Instance.PlaySound(SoundManager.SoundType.LightingBasic);
            }
        }
        else
        {
            _timer += Time.deltaTime;
            IsShooting = false;
        }
    }

    public void Shoot()
    {
        IsShooting = true;
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
    void SetNewTargetDestination()
    {
        //Physics.SphereCast()
    }
    void InstantiateProjectile(Transform firePoint)
    {
        // Muzzle Flash
        /*var mFlash = Instantiate(_muzzleFlash, firePoint.position, firePoint.rotation, firePoint) as GameObject;
        mFlash.SetActive(true);
        Destroy(mFlash, 0.2f);*/

        // Projectile
        for (int i = 0; i < _amountInContainer; i++)
        {
            if (!_projectilesInContainer[i].activeInHierarchy)
            {
                _projectilesInContainer[i].transform.SetParent(_container);
                _projectilesInContainer[i].transform.position = firePoint.position;
                _projectilesInContainer[i].transform.rotation = firePoint.rotation;
                _projectilesInContainer[i].SetActive(true);
                //_projectilesInContainer[i].GetComponent<Rigidbody>().velocity = _projectilesInContainer[i].transform.forward * _projectileSpeed;
                _projectilesInContainer[i].GetComponent<TimeToPoof>().Initiate();
                return;
            }
        }
    }
    private void PrepareProjectiles()
    {
        for (int i = 0; i < _amountInContainer; i++)
        {
            var projectile = Instantiate(_basicProjectile, _firePoint.position, _firePoint.rotation) as GameObject;
            projectile.transform.SetParent(_container);
            projectile.SetActive(false);
            _projectilesInContainer.Add(projectile);
        }
    }

    public void ChangeElement()
    {
        switch (AbilitiesSelection.Instance.RightHandElement)
        {
            case AbilitiesSelection.ElementType.Lightning:
                foreach (var item in _projectilesInContainer)
                {
                    Destroy(item.gameObject);
                }
                _projectilesInContainer.Clear();
                _basicProjectile = LightningProj;
                _basicProjectileFeedback = LightningProjFeedback;
                _muzzleFlash = _LightningMuzzleFlash;
                PrepareProjectiles();
                break;
            case AbilitiesSelection.ElementType.Fire:
                foreach (var item in _projectilesInContainer)
                {
                    Destroy(item.gameObject);
                }
                _projectilesInContainer.Clear();
                _basicProjectile = FireProj;
                _basicProjectileFeedback = FireProjFeedback;
                _muzzleFlash = _FireMuzzleFlash;
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
