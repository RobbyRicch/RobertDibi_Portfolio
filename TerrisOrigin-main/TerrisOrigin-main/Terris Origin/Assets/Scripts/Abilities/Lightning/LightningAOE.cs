using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightningAOE : MonoBehaviour
{
    private InputManager _inputManager;
    private Camera _mainCamera;

    [SerializeField] private float _cooldown = 8f;
    private float _timer;
    [SerializeField] private float _delay = 0.3f;
    private float _delayTimer;
    private bool _initiatedAbility;
    [SerializeField] private int _manaCost = 20;

    [SerializeField] private ManaSystem _manaSystem;
    [SerializeField] private AnimationManager animationManager;
    [SerializeField] private GameObject _lightningWrathOBJ;
    [SerializeField] private GameObject _lightningAOEOBJ;
    [SerializeField] private GameObject _AOEVisualizer;
    private Vector3 _spawnPos;
    private Vector3 _hitPos;

    private bool _visualizing;
    private GameObject _tempVisualizerObj;

    [SerializeField] private LayerMask _layerToHit;
    [SerializeField] private float _attackRange = 10;
    [SerializeField] private float _attackMaxHeight = 5;

    [SerializeField] private float _aOERadius = 5;
    [SerializeField] private float _lastingTime = 2;

    [SerializeField] private Image _cooldownDot, _cooldownSymbol;

    private PlayerInfo _playerInfo;
    private bool GroundHit = false;

    private void Awake()
    {
        _inputManager = GetComponentInParent<InputManager>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        _playerInfo = GetComponentInParent<PlayerInfo>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _timer = _cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        // Showing the Hit Zone while Holding Button
        if (_inputManager.WrathHold && _timer >= _cooldown && _manaSystem.CurrentMana >= _manaCost)
        {
            animationManager.CanWrathAnim = true;
            if (animationManager.LightningWrathP1Loaded)
                HitZone();
        }
        // On Button Release Initiates Attack
        else if (_inputManager.WrathHoldCanceled && _timer >= _cooldown && animationManager.LightningWrathP2Loaded && _manaSystem.Decrease(_manaCost))
        {
            CancelHitZone();
            _playerInfo.SetAttacking(true);
            _initiatedAbility = true;
            _timer = 0;
            _inputManager.WrathHoldCanceled = false;
            ActivateAttack();
            SoundManager.Instance.PlaySound(SoundManager.SoundType.LightningWrath);
        }
        else
        {
            if (_timer < _cooldown)
                _inputManager.WrathHoldCanceled = false;
            _timer += Time.deltaTime;
        }

        // Creates the Explosion After Initiation Delay
        if (_initiatedAbility)
        {
            _delayTimer += Time.deltaTime;
            if (_delayTimer >= _delay)
            {
                _initiatedAbility = false;
                _delayTimer = 0;
                InstantiateExplosion();
                _playerInfo.SetAttacking(false);
            }
        }

        CooldownUI();
    }

    private void HitZone()
    {
        SetSpawnLocation();
        if (!_visualizing)
        {
            _visualizing = true;
            _tempVisualizerObj = Instantiate(_AOEVisualizer, _spawnPos, Quaternion.identity);
            _tempVisualizerObj.transform.SetParent(null);
            //_tempVisualizerObj.transform.localScale = new Vector3(_aOERadius, _tempVisualizerObj.transform.localScale.y, _aOERadius);
        }
        if (GroundHit)
        {
            _tempVisualizerObj.transform.position = _spawnPos;
        }
        else
        {
            SetSpawnHeight(_spawnPos);
            _tempVisualizerObj.transform.position = _hitPos;
        }
    }

    private void CancelHitZone()
    {
        _visualizing = false;
        Destroy(_tempVisualizerObj);
    }

    private void ActivateAttack()
    {
        SetSpawnLocation();
        InstantiateProjectile();
    }

    private void SetSpawnLocation()
    {
        RaycastHit hit;
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        if (Physics.Raycast(ray, out hit, _attackRange, _layerToHit))
        {
            _spawnPos = hit.point;
            GroundHit = true;
        }
        else
        {
            _spawnPos = ray.GetPoint(_attackRange);
            GroundHit = false;
        }

        Debug.DrawLine(ray.origin, _spawnPos, Color.cyan, 3);
    }

    private void InstantiateProjectile()
    {
        GameObject projectile;
        if (GroundHit)
            projectile = Instantiate(_lightningWrathOBJ, _spawnPos, Quaternion.identity);
        else
            projectile = Instantiate(_lightningWrathOBJ, _hitPos, Quaternion.identity);

        projectile.transform.SetParent(null);
        SetSpawnHeight(projectile.transform.position);
        projectile.transform.position = _spawnPos;
        Destroy(projectile, 1f);
    }

    private void InstantiateExplosion()
    {
        var aoeObj = Instantiate(_lightningAOEOBJ, _hitPos, Quaternion.identity);
        aoeObj.transform.SetParent(null);
        aoeObj.transform.localScale = new Vector3(_aOERadius, _aOERadius, _aOERadius);
        Destroy(aoeObj, _lastingTime);
    }

    private void SetSpawnHeight(Vector3 pos)
    {
        RaycastHit hit;
        Ray ray = new Ray(pos, Vector3.up * -1);
        if (Physics.Raycast(ray, out hit, 100, _layerToHit))
        {
            _spawnPos = hit.point + new Vector3(0, _attackMaxHeight, 0);
            _hitPos = hit.point;
        }
        else
        {
            _spawnPos += new Vector3(0, _attackMaxHeight, 0);
            _hitPos = _spawnPos - new Vector3(0, _attackMaxHeight, 0);
        }

        Debug.DrawLine(ray.origin, _spawnPos, Color.yellow, 3);

        Debug.DrawLine(_spawnPos, _hitPos, Color.green, 3);
    }

    private void CooldownUI()
    {
        if (_timer >= _cooldown)
        {
            _cooldownDot.enabled = true;
            _cooldownSymbol.color = Color.white;
        }
        else
        {
            _cooldownDot.enabled = false;
            _cooldownSymbol.color = new Color(0.75f, 0.75f, 0.75f, 1);
        }
    }
}
