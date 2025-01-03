using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LightningDash : MonoBehaviour
{
    //private OldInputSystemHandler _oldInputSystemHandler;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerMotor _playerMotor;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private AnimationManager animationManager;
    //private ManaHandler _manaHandler;

    public bool isDashing;
    public float _dashingSpeed = 30f;
    public float _coolDown = 3f;
    private float _cooldownTimer = 0;
    public float _dashingTimeLength = 0.3f;
    private float _dashingtimer = 0;
    private Vector3 _spawnPos;
    private Vector3 _hitPos;
    private bool _visualizing = false;
    private float DashXRotation = 0f;
    [SerializeField] private int _manaCost = 10;
    [SerializeField] private float _Range;
    [SerializeField] private float maxDashAngle = 30f;
    [SerializeField] private float minDashAngle = 30f;
    [SerializeField] private LayerMask _layerToHit;

    [SerializeField] private ManaSystem _manaSystem;
    [SerializeField] private Image _cooldownDot, _cooldownSymbol;
    [SerializeField] private float _dashMaxHeight = 5;
    [SerializeField] private float _aOERadius = 3;

    //[SerializeField] ParticleSystem ForwardDashParticles;
    [SerializeField] GameObject DashBoxVolume;
    [SerializeField] GameObject DashUI;
    [SerializeField] GameObject DashIndicator;
    private GameObject _tempVisualizerObj;

    private bool GroundHit = false;
    //[SerializeField] Image DashFill;
    void Start()
    {
        //_manaHandler = GetComponentInParent<ManaHandler>();
        _cooldownTimer = _coolDown;
    }

    void Update()
    {
        //Hold Dash Button
        if (_cooldownTimer >= _coolDown && _inputManager.DashHold && _manaSystem.CurrentMana >= _manaCost)
        {
            animationManager.CanDashAnim = true;
            DashZone();
            DashUI.SetActive(true);
            //Time.timeScale = 0.5f;
        }
        else if (_cooldownTimer >= _coolDown && _inputManager.DashHoldCanceled && _manaSystem.Decrease(_manaCost) && animationManager.LightningDashLoaded)
        {
            if (!isDashing)
            {
                CancelDashZone();
                animationManager.LightningDashLoaded = false;
                _inputManager.DashHoldCanceled = false;
                OnStartDash();
                SoundManager.Instance.PlaySound(SoundManager.SoundType.LightningDash);
            }
        }
        else
        {
            _inputManager.DashHoldCanceled = false;
            _cooldownTimer += Time.deltaTime;
        }
        //Tap Dash Bitton
        if (_cooldownTimer >= _coolDown && _inputManager.DashPress && _manaSystem.CurrentMana >= _manaCost)
        {
            if (!isDashing)
            {
                _manaSystem.Decrease(_manaCost);
                OnStartDash();
                SoundManager.Instance.PlaySound(SoundManager.SoundType.LightningDash);
            }
        }
        else
        {
            if (_cooldownTimer < _coolDown + 1)
            {
                _cooldownTimer += Time.deltaTime;
            }
            //DashFill.fillAmount = _cooldownTimer / _coolDown;
        }
        HandleDash();

        CooldownUI();
    }

    void DashZone()
    {
        SetSpawnLocation();
        if (!_visualizing)
        {
            _visualizing = true;
            _tempVisualizerObj = Instantiate(DashIndicator, _spawnPos, Quaternion.identity);
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
    private void CancelDashZone()
    {
        _visualizing = false;
        Destroy(_tempVisualizerObj);
    }
    private void SetSpawnLocation()
    {
        RaycastHit hit;
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        if (Physics.Raycast(ray, out hit, _Range, _layerToHit))
        {
            _spawnPos = hit.point;
            GroundHit = true;
        }
        else
        {
            _spawnPos = ray.GetPoint(_Range);
            GroundHit = false;
        }

        Debug.DrawLine(ray.origin, _spawnPos, Color.cyan, 3);
    }
    private void SetSpawnHeight(Vector3 pos)
    {
        RaycastHit hit;
        Ray ray = new Ray(pos, Vector3.up * -1);
        if (Physics.Raycast(ray, out hit, 100, _layerToHit))
        {
            _spawnPos = hit.point + new Vector3(0, _dashMaxHeight, 0);
            _hitPos = hit.point;
            //Debug.Log(_spawnPos);
        }
        else
        {
            _spawnPos += new Vector3(0, _dashMaxHeight, 0);
            _hitPos = _spawnPos - new Vector3(0, _dashMaxHeight, 0);
            //Debug.Log("2");
        }

        Debug.DrawLine(ray.origin, _spawnPos, Color.yellow, 3);
        Debug.DrawLine(_spawnPos, _hitPos, Color.green, 3);
    }

    void HandleDash()
    {
        if (isDashing)
        {
            if (_dashingtimer <= _dashingTimeLength)
            {
                Vector3 newDir;
                if (GroundHit)
                    newDir = (_spawnPos + Vector3.up) - transform.position;
                else
                    newDir = (_hitPos + Vector3.up) - transform.position;

                _dashingtimer += Time.deltaTime;
                _controller.Move(newDir * _dashingSpeed * Time.deltaTime);
                _playerMotor.EnableGravity(false);
                //SoundManager.Instance.PlaySound(SoundManager.SoundType.LightningDash);
            }
            else
            {
                OnEndDash();
            }
        }
    }
    void OnStartDash()
    {
        //Time.timeScale = 1f;
        isDashing = true;
        DashBoxVolume.SetActive(true);
        DashUI.SetActive(true);
        PlayDashParicles();
        //_inputManager.DashPress = false;
        //_manaHandler.UseManaOnce(_manaCost);
    }
    void OnEndDash()
    {
        _cooldownTimer = 0;
        _dashingtimer = 0;
        isDashing = false;
        DashBoxVolume.SetActive(false);
        DashUI.SetActive(false);
        _playerMotor.EnableGravity(true);
    }
    void PlayDashParicles()
    {
        Vector2 inputVector = _playerMotor.moveDirection;

        if (inputVector.y > 0 && Mathf.Abs(inputVector.x) <= inputVector.y)
        {
            // Forward & Forward Diagnols
            //ForwardDashParticles.Play();
            return;
        }
        //ForwardDashParticles.Play();
    }

    private void CooldownUI()
    {
        if (_cooldownTimer >= _coolDown)
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

    private void OnValidate()
    {
        _inputManager = GetComponentInParent<InputManager>();
        _controller = GetComponentInParent<CharacterController>();
        _playerMotor = GetComponentInParent<PlayerMotor>();
    }
}
