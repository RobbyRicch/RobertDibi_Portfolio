using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AttractorController : MonoBehaviour
{
    #region Predetermind Data
    [Header("Data")]
    [SerializeField] private string _movingHeavyObstacleTag = "Moving Obstacle", _pickupTag = "Pickable";
    [SerializeField] private float _maxDistance = 90.0f, _maxDistanceReturnOffset = 0.15f;
    [SerializeField] private float _attractorThrowForce = 160.0f, _throwSpeedFactor = 2.0f, _attractorReturnForce = 640.0f;
    [SerializeField] private float _playerPullImpulse = 160.0f, _attractorReelForce = 80.0f;
    [SerializeField] private float _timeToReachCurrentTarget = 0.1f;
    [SerializeField] private float _snapDistance = 1.0f, _ceasePullDistance = 20.0f;
    [SerializeField] private float _launchModifier = 4.0f;
    [SerializeField] private float _catchRange = 1.5f;
    [SerializeField] private float _handsProximityRange = 2.0f;
    [SerializeField] private float _pullPickupSpeed = 20.0f;

    [Header("Energy")]
    [SerializeField] private float _maxEnergy = 15.0f;
    [SerializeField] private float _energyThrowCost = 2.5f, _energyHoldCost = 1.0f, _energyReplanishRate = 3.0f;

    //[Header("Beam")]
    //[SerializeField] private float _beamDrawingRate = 15.0f;

    [Header("Effects")] // left = 0, right = 1.
    [SerializeField] private Transform _vfxParent;
    public Transform VfxParent => _vfxParent;

    [SerializeField] private GameObject _wallSnapEffectPrefab;
    [SerializeField] private ParticleSystem[] _throwEffects, _onHandEffects;
    public ParticleSystem[] ThrowEffects => _throwEffects;
    public ParticleSystem[] OnHandEffects => _onHandEffects;

    [SerializeField] private Vector3 _throwEffectsPosition = new(0.0f, 0.75f, 0.0f);
    [SerializeField] private Vector3[] _throwEffectsRotations = new Vector3[] { new(-90.0f, 0.0f, 0.0f), new Vector3(90.0f, 0.0f, 0.0f) };
    [SerializeField] private Vector3 _onHandEffectsPosition = new(0.0f, 0.75f, 0.0f);
    [SerializeField] private Vector3[] _onHandEffectsRotations = new Vector3[] { new(-90.0f, 0.0f, 0.0f), new Vector3(90.0f, 0.0f, 0.0f) };

    [Header("Components")]
    [SerializeField] private PlayerInputHandler _player;
    [SerializeField] private Transform _leftHandOriginTr, _rightHandOriginTr;
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private GameObject _targetIndicatorPrefab;
    [SerializeField] private LayerMask _grabbingLayer;
    #endregion

    #region Properties
    [SerializeField] private Transform _leftHandTr, _rightHandTr;
    public Transform LeftHandTr { get => _leftHandTr; set => _leftHandTr = value; }
    public Transform RightHandTr { get => _rightHandTr; set => _rightHandTr = value; }

    [SerializeField] private Transform _leftHandParent, _rightHandParent;
    public Transform LeftHandParent { get => _leftHandParent; set => _leftHandParent = value; }
    public Transform RightHandParent { get => _rightHandParent; set => _rightHandParent = value; }

    [SerializeField] private Transform _aimPointTr;
    public Transform AimPointTr => _aimPointTr;

    private Rigidbody _leftGrabbedRb, _rightGrabbedRb;
    public Rigidbody LeftGrabbedRb => _leftGrabbedRb;
    public Rigidbody RightGrabbedRb => _rightGrabbedRb;

    [SerializeField] private LineRenderer[] _inAirBeams; // 0 = left, 1 = right
    public LineRenderer[] InAirBeams => _inAirBeams;

    [SerializeField] private LineRenderer[] _leftPullBeams, _leftPulledBeams, _rightPullBeams, _rightPulledBeams; // 0 = outer, 1 = inner
    public  LineRenderer[] LeftPullBeams => _leftPullBeams; // 0 = outer, 1 = inner
    public  LineRenderer[] LeftPulledBeams => _leftPulledBeams; // 0 = outer, 1 = inner
    public LineRenderer[] RightPullBeams => _rightPullBeams; // 0 = outer, 1 = inner
    public LineRenderer[] RightPulledBeams => _rightPulledBeams; // 0 = outer, 1 = inner

    [SerializeField] private LineRenderer[] _leftActiveBeams, _rightActiveBeams; // 0 = outer, 1 = inner

    [SerializeField] private Color _playerColor;
    public Color PlayerColor { get => _playerColor; set => _playerColor = value; }

    [SerializeField] private bool _isAttractorsDisabled;
    public bool IsAttractorDisabled => _isAttractorsDisabled;

    private bool _isLeftBeamOnline = false, _isRightBeamOnline;
    public bool IsLeftBeamOnline => _isLeftBeamOnline;
    public bool IsRightBeamOnline => _isRightBeamOnline;

    private bool _isGrabbingHeavyObject;
    public bool IsGrabbingHeavyObject => _isGrabbingHeavyObject;

    private bool _isUsingLeftAttractor, _isUsingRightAttractor;
    public bool IsUsingLeftAttractor => _isUsingLeftAttractor;
    public bool IsUsingRightAttractor => _isUsingRightAttractor;
    #endregion

    #region Dynamic Data
    private Transform _currentTarget, _leftTarget, _rightTarget;
    private GameObject _currentTargetIndicator;
    private ParticleSystem _wallSnapEffect;
    private RaycastHit _currentHitTarget, _leftHit, _rightHit;

    private int _leftPhysicsCounter = 0, _rightPhysicsCounter = 0;
    private float _currentEnergy, _energyPercentage;
    private float _distanceFromLeftGrabbedObject, _distanceFromRightGrabbedObject;
    private float _leftLineLenght, _rightLineLenght;
    private bool _isThrowingLeftAttractor = false, _isThrowingRightAttractor = false;
    private bool _isLeftAttractorSnapped = false, _isRightAttractorSnapped = false;
    private bool _isLeftPressed, _isLeftCanceled, _isRightPressed, _isRightCanceled;
    #endregion

    #region Constant Data
    private const int _heavyObjectLayer = 8;
    #endregion

    #region StateMachines
    private delegate void LeftHandState();
    private LeftHandState _leftHandState;

    private delegate void RightHandState();
    private RightHandState _rightHandState;

    private delegate void LeftPhysicsState();
    private LeftPhysicsState _leftPhysicsState;

    private delegate void RightPhysicsState();
    private RightPhysicsState _rightPhysicsState;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        _leftHandState = LeftHandIdle;
        _rightHandState = RightHandIdle;
        _leftPhysicsState = LeftStandby;
        _rightPhysicsState = RightStandby;

        _currentEnergy = _maxEnergy;
        _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
    }
    private void Start()
    {
        /*Transform leftThrowEffect = _throwEffects[0].transform;
        leftThrowEffect.SetParent(_leftHandTr);

        Transform rightThrowEffect = _throwEffects[1].transform;
        rightThrowEffect.SetParent(_rightHandTr);*/

        InitializeThrowEffect();
        InitializeOnHandEffect();
        InitializeBeams();
        InitializeWallSnapEffect();
    }
    private void Update()
    {
        if (_isAttractorsDisabled)
            return;

        CatchTarget();

        _leftHandState.Invoke();
        _rightHandState.Invoke();

        /*// Input Start --------------- // old mouse interactions
        if (Input.GetMouseButtonDown(0))
            _isLeftPressed = true;
        else
            _isLeftPressed = false;

        if (Input.GetMouseButtonDown(1))
            _isRightPressed = true;
        else
            _isRightPressed = false;

        // Input Cancel -------------- //
        if (Input.GetMouseButtonUp(0))
            _isLeftCanceled = true;
        else
            _isLeftCanceled = false;

        if (Input.GetMouseButtonUp(1))
            _isRightCanceled = true;
        else
            _isRightCanceled = false;*/

        ReturnHands();
        /*AimBeam();*/

        if (CheckAttractorUseage())
            HoldEnergySpend();
        else
            ReplanishEnergy();
    }
    private void FixedUpdate()
    {
        if (_isAttractorsDisabled)
            return;

        _leftPhysicsState.Invoke();
        _rightPhysicsState.Invoke();
    }
    #endregion

    #region Left Hand States
    private void LeftHandIdle()
    {
        if (_isLeftPressed)
        {
            ThrowLeftAttractor();
            //_leftTempAimDirection = _aimPointTr.forward;

            _leftHandState = LeftHandThrowing;
            return;
        }
    }
    private void LeftHandThrowing()
    {
        if (_isThrowingLeftAttractor)
            UpdateThrownLeftHandPos();

        if (_isLeftCanceled || _currentEnergy < _energyHoldCost)
        {
            _leftHandState = LeftHandReturning;
            return;
        }

        if (_inAirBeams[0].gameObject.activeInHierarchy) // update unsnapped beam
            UpdateBeam(_inAirBeams[0], true);
        else if (_leftActiveBeams[0].gameObject.activeInHierarchy) // update snapped beam
            UpdateBeam(_leftActiveBeams[0], _leftActiveBeams[1], true);
    }
    private void LeftHandReturning()
    {
        ReturnLeftAttractor();
        _leftHandState = LeftHandIdle;
        return;
    }
    #endregion

    #region Right Hand States
    private void RightHandIdle()
    {
        if (_isRightPressed)
        {
            ThrowRightAttractor();
            //_rightTempAimDirection = _aimPointTr.forward;

            _rightHandState = RightHandThrowing;
            return;
        }
    }
    private void RightHandThrowing()
    {
        if (_isThrowingRightAttractor)
            UpdateThrownRightHandPos();

        if (_isRightCanceled || _currentEnergy < _energyHoldCost)
        {
            _rightHandState = RightHandReturning;
            return;
        }

        if (_inAirBeams[1].gameObject.activeInHierarchy) // update unsnapped beam
            UpdateBeam(_inAirBeams[1], false);
        else if (_rightActiveBeams[0].gameObject.activeInHierarchy) // update snapped beam
            UpdateBeam(_rightActiveBeams[0], _rightActiveBeams[1], false);
    }
    private void RightHandReturning()
    {
        ReturnRightAttractor();
        _rightHandState = RightHandIdle;
        return;
    }
    #endregion

    #region LeftHand Physics States
    private void LeftStandby()
    {
        if (_isLeftAttractorSnapped && _isGrabbingHeavyObject)
        {
            _leftPhysicsState = LeftHoldingHeavyObject;
            return;
        }
        else if (_isLeftAttractorSnapped && !_isGrabbingHeavyObject)
        {
            _leftPhysicsState = LeftHoldingLightObject;
            return;
        }
    }
    private void LeftHoldingHeavyObject()
    {
        if (_player.Controller.IsUsingPickupLeft)
        {
            _leftPhysicsCounter = 0;
            _leftPhysicsState = LeftStandby;
            CancelAttractorLeft(true);
            return;
        }
        if (!_isLeftAttractorSnapped)
        {
            _leftPhysicsCounter = 0;
            Vector3 dir = _player.Controller.CrosshairParent.transform.forward.normalized;
            _playerRb.AddForce(_attractorThrowForce * 2 * _leftLineLenght * dir / _launchModifier, ForceMode.Impulse);
            _leftPhysicsState = LeftStandby;
            return;
        }

        Vector3 direction = _leftHandTr.position - _playerRb.position;
        float distanceFromHeavyObject = Vector3.Distance(_leftHandTr.position, _playerRb.position);

        if (_leftPhysicsCounter == 0)
            _playerRb.AddForce(_playerPullImpulse * direction, ForceMode.Impulse);
        else if (_leftPhysicsCounter > 0 && distanceFromHeavyObject > _ceasePullDistance)
            _playerRb.AddForce(_attractorReelForce * direction, ForceMode.Acceleration);

        if (_leftGrabbedRb is not null)
        {
            //return; // get's wanted behaviour, but at what cost? -_o
            _playerRb.AddForce(_playerPullImpulse / 2 * Time.fixedDeltaTime * direction, ForceMode.Impulse); // need to figure 
        }

        _leftLineLenght = GetLineLength(_leftActiveBeams[0]);

        _leftPhysicsCounter++;
    }
    private void LeftHoldingLightObject()
    {
        if (_player.Controller.IsUsingPickupLeft || _leftGrabbedRb == null)
        {
            _leftPhysicsCounter = 0;
            _leftPhysicsState = LeftStandby;
            CancelAttractorLeft(true);
            return;
        }
        if (!_isLeftAttractorSnapped)
        {
            _leftPhysicsCounter = 0;
            _leftPhysicsState = LeftStandby;
            return;
        }

        /*if (_leftTarget.transform.CompareTag(_pickupTag))
        {
            Vector3 targetPosition = _playerRb.position;
            Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, _pullPickupSpeed * Time.deltaTime);
            _leftTarget.transform.position = newPosition;
            return;
        }*/

        Vector3 direction = _playerRb.position - _leftGrabbedRb.position;
        float distanceFromHeavyObject = Vector3.Distance(_playerRb.position, _leftGrabbedRb.position);

        if (_leftPhysicsCounter == 0)
            _leftGrabbedRb.AddForce(_playerPullImpulse /** Time.fixedDeltaTime*/ * direction, ForceMode.Impulse);
        else if (_leftPhysicsCounter > 0 && distanceFromHeavyObject > _ceasePullDistance)
            _leftGrabbedRb.AddForce(_attractorReelForce /** Time.fixedDeltaTime*/ * direction, ForceMode.Acceleration);

        _leftPhysicsCounter++;
    }
    #endregion

    #region RightHand Physics States
    private void RightStandby()
    {
        if (_isRightAttractorSnapped && _isGrabbingHeavyObject)
        {
            _rightPhysicsState = RightHoldingHeavyObject;
            return;
        }
        else if (_isRightAttractorSnapped && !_isGrabbingHeavyObject)
        {
            _rightPhysicsState = RightHoldingLightObject;
            return;
        }
    }
    private void RightHoldingHeavyObject()
    {
        if (_player.Controller.IsUsingPickupRight)
        {
            _rightPhysicsCounter = 0;
            _rightPhysicsState = RightStandby;
            CancelAttractorRight(true);
            return;
        }
        if (!_isRightAttractorSnapped)
        {
            _rightPhysicsCounter = 0;
            Vector3 dir = _player.Controller.CrosshairParent.transform.forward.normalized;
            _playerRb.AddForce(_attractorThrowForce * 2 * _rightLineLenght * dir / _launchModifier, ForceMode.Impulse);
            _rightPhysicsState = RightStandby;
            return;
        }

        Vector3 direction = _rightHandTr.position - _playerRb.position;
        float distanceFromHeavyObject = Vector3.Distance(_rightHandTr.position, _playerRb.position);

        if (_rightPhysicsCounter == 0)
            _playerRb.AddForce(_playerPullImpulse * direction, ForceMode.Impulse);
        else if (_rightPhysicsCounter > 0 && distanceFromHeavyObject > _ceasePullDistance)
            _playerRb.AddForce(_attractorReelForce * direction, ForceMode.Acceleration);

        if (_rightGrabbedRb is not null)
        {
            //return; // get's wanted behaviour, but at what cost? -_o
            _playerRb.AddForce(_playerPullImpulse / 2 * Time.fixedDeltaTime * direction, ForceMode.Impulse); // need to figure 
        }

        _rightLineLenght = GetLineLength(_rightActiveBeams[0]);

        _rightPhysicsCounter++;
    }
    private void RightHoldingLightObject()
    {
        if (_player.Controller.IsUsingPickupRight || _rightGrabbedRb == null)
        {
            _rightPhysicsCounter = 0;
            _rightPhysicsState = RightStandby;
            CancelAttractorRight(true);
            return;
        }
        if (!_isRightAttractorSnapped)
        {
            _rightPhysicsCounter = 0;
            _rightPhysicsState = RightStandby;
            return;
        }

        Vector3 direction = _playerRb.position - _rightGrabbedRb.position;
        float distanceFromHeavyObject = Vector3.Distance(_playerRb.position, _rightGrabbedRb.position);

        if (_rightPhysicsCounter == 0)
            _rightGrabbedRb.AddForce(_playerPullImpulse/* * Time.fixedDeltaTime*/ * direction, ForceMode.Impulse);
        else if (_rightPhysicsCounter > 0 && distanceFromHeavyObject > _ceasePullDistance)
            _rightGrabbedRb.AddForce(_attractorReelForce/* * Time.fixedDeltaTime*/ * direction, ForceMode.Acceleration);

        _rightPhysicsCounter++;
    }
    #endregion

    #region Initializations
    private void InitializeThrowEffect()
    {
        Transform leftThrowEffect = _throwEffects[0].transform;
        leftThrowEffect.SetParent(_leftHandOriginTr);
        leftThrowEffect.SetLocalPositionAndRotation(_throwEffectsPosition, Quaternion.Euler(_throwEffectsRotations[0]));

        Transform rightThrowEffect = _throwEffects[1].transform;
        rightThrowEffect.SetParent(_rightHandOriginTr);
        rightThrowEffect.SetLocalPositionAndRotation(_throwEffectsPosition, Quaternion.Euler(_throwEffectsRotations[1]));
    }
    private void InitializeOnHandEffect()
    {
        Transform leftOnHandEffect = _onHandEffects[0].transform;
        leftOnHandEffect.SetParent(_leftHandTr);
        leftOnHandEffect.SetLocalPositionAndRotation(_onHandEffectsPosition, Quaternion.Euler(_onHandEffectsRotations[0]));

        Transform rightOnHandEffect = _onHandEffects[1].transform;
        rightOnHandEffect.SetParent(_rightHandTr);
        rightOnHandEffect.SetLocalPositionAndRotation(_onHandEffectsPosition, Quaternion.Euler(_onHandEffectsRotations[1]));
    }
    private void InitializeBeams()
    {
        /* In Air Beams */ // no snap
        _inAirBeams[0].transform.SetParent(_leftHandTr);
        _inAirBeams[0].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _inAirBeams[1].transform.SetParent(_rightHandTr);
        _inAirBeams[1].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        /* Active Beams */ // snapped
        _leftActiveBeams[0].transform.SetParent(_leftHandTr);
        _leftActiveBeams[0].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _leftActiveBeams[1].transform.SetParent(_leftHandTr);
        _leftActiveBeams[1].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _rightActiveBeams[0].transform.SetParent(_rightHandTr);
        _rightActiveBeams[0].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _rightActiveBeams[1].transform.SetParent(_rightHandTr);
        _rightActiveBeams[1].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        /* Left Pull Beams */ // snapped
        _leftPullBeams[0].transform.SetParent(_leftHandTr);
        _leftPullBeams[0].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _leftPullBeams[1].transform.SetParent(_leftHandTr);
        _leftPullBeams[1].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        /* Left Pulled Beams */ // snapped
        _leftPulledBeams[0].transform.SetParent(_leftHandTr);
        _leftPulledBeams[0].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _leftPulledBeams[1].transform.SetParent(_leftHandTr);
        _leftPulledBeams[1].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        /* Right Pull Beams */ // snapped
        _rightPullBeams[0].transform.SetParent(_rightHandTr);
        _rightPullBeams[0].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _rightPullBeams[1].transform.SetParent(_rightHandTr);
        _rightPullBeams[1].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        /* Right Pulled Beams */ // snapped
        _rightPulledBeams[0].transform.SetParent(_rightHandTr);
        _rightPulledBeams[0].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        _rightPulledBeams[1].transform.SetParent(_rightHandTr);
        _rightPulledBeams[1].transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

    }
    private void InitializeWallSnapEffect()
    {
        _wallSnapEffect = CreateWallSnapEffect();
    }
    public void ReInitializeAll()
    {
        InitializeThrowEffect();
        InitializeOnHandEffect();
        InitializeBeams();
    }
    #endregion

    #region Calculations and Checkups
    private bool CheckAttractorUseage()
    {
        return _isUsingLeftAttractor || _isUsingRightAttractor ? true : false;
    }
    private int GetTargetLayer(GameObject target)
    {
        int layer = target.layer;

        if (_grabbingLayer == (_grabbingLayer | (1 << layer)))
            return layer;
        else
            return -1;
    }
    private bool IsGrabbingLayer(GameObject target)
    {
        int layer = target.layer;
        return _grabbingLayer == (_grabbingLayer | (1 << layer)) ? true : false;
    }
    public bool IsLeftGrabbingPickup(Transform pickableGO)
    {
        if (_leftTarget is not null)
            return _leftTarget == pickableGO ? true : false;
        else
            return false;
    }
    public bool IsRightGrabbingPickup(Transform pickableGO)
    {
        if (_rightTarget is not null)
            return _rightTarget == pickableGO ? true : false;
        else
            return false;
    }
    private bool IsObjectHeavy(int targetLayer, int heavyLayer)
    {
        //return heavyLayer == (heavyLayer | (1 << targetLayer)) ? true : false;
        return heavyLayer == targetLayer ? true : false;
    }
    private void CatchTarget()
    {
        float halfDistance = _maxDistance / 2;
        Vector2 handsScale = new(1 * _catchRange, 1 * _catchRange);
        Vector2 handsScale2 = new(1 * _catchRange, 1);

        // Perform a box cast from the starting position with the specified layer mask and distance
        RaycastHit hit;
        if (Physics.BoxCast(_aimPointTr.position, new Vector3(handsScale.x / 2.0f, handsScale.y / 2.0f, halfDistance / 10.0f), _aimPointTr.forward, out hit, _player.Controller.CrosshairParent.rotation, _maxDistance))
        {
            //Debug.Log($"hit: x-{hit.transform.position.x}, z-{hit.transform.position.z}");
            _currentHitTarget = hit;
            _currentTarget = hit.transform;
            //IndicateTarget(hit);
            _timeToReachCurrentTarget *= halfDistance;
        }
        else
        {
            //Debug.Log($"Looking for target");
            //Destroy(_currentTargetIndicator);
            _currentTarget = null;
            _currentTarget = hit.transform;
        }
    }
    private float GetLineLength(LineRenderer line)
    {
        float lenght = 0;

        Vector3[] linePositions = new Vector3[2];
        line.GetPositions(linePositions);

        lenght = Vector3.Distance(linePositions[0], linePositions[1]);
        return lenght;
    }
    private void ReturnHands()
    {
        if (_leftHandTr.position != _leftHandOriginTr.position && (!_isThrowingLeftAttractor && !_isUsingLeftAttractor))
            _leftHandTr.position = Vector3.MoveTowards(_leftHandTr.position, _leftHandOriginTr.position, _attractorReturnForce/* * Time.deltaTime*/);

        if (_rightHandTr.position != _rightHandOriginTr.position && (!_isThrowingRightAttractor && !_isUsingRightAttractor))
            _rightHandTr.position = Vector3.MoveTowards(_rightHandTr.position, _rightHandOriginTr.position, _attractorReturnForce/* * Time.deltaTime*/);
    }
    private bool CheckLeftHandProximity() // make sure l hand is close to l hand origin
    {
        Vector3 distanceVector = _leftHandTr.position - _leftHandOriginTr.position;
        float distance = distanceVector.magnitude;

        if (distance <= _handsProximityRange)
            return true;
        else
            return false;
    }
    private bool CheckRightHandProximity() // make sure r hand is close to r hand origin
    {
        Vector3 distanceVector = _rightHandTr.position - _rightHandOriginTr.position;
        float distance = distanceVector.magnitude;

        if (distance <= _handsProximityRange)
            return true;
        else
            return false;
    }
    public float GetEnergyPercentage()
    {
        return _energyPercentage;
    }
    #endregion

    #region Beam Manipulations
    private void SwitchToPullBeam(bool isLeft)
    {
        int index = isLeft ? 0 : 1;
        switch (index)
        {
            case 0:
                _leftActiveBeams[0] = _leftPullBeams[0];
                _leftActiveBeams[1] = _leftPullBeams[1];
                _leftActiveBeams[0].gameObject.SetActive(true);
                _leftActiveBeams[1].gameObject.SetActive(true);

                _leftPulledBeams[0].gameObject.SetActive(false);
                _leftPulledBeams[1].gameObject.SetActive(false);
                break;

            case 1:
                _rightActiveBeams[0] = _rightPullBeams[0];
                _rightActiveBeams[1] = _rightPullBeams[1];
                _rightActiveBeams[0].gameObject.SetActive(true);
                _rightActiveBeams[1].gameObject.SetActive(true);

                _rightPulledBeams[0].gameObject.SetActive(false);
                _rightPulledBeams[1].gameObject.SetActive(false);
                break;
        }
        
    }
    private void SwitchToPulledBeam(bool isLeft)
    {
        int index = isLeft ? 0 : 1;
        switch (index)
        {
            case 0:
                _leftActiveBeams[0] = _leftPulledBeams[0];
                _leftActiveBeams[1] = _leftPulledBeams[1];
                _leftActiveBeams[0].gameObject.SetActive(true);
                _leftActiveBeams[1].gameObject.SetActive(true);

                _leftPullBeams[0].gameObject.SetActive(false);
                _leftPullBeams[1].gameObject.SetActive(false);
                break;

            case 1:
                _rightActiveBeams[0] = _rightPulledBeams[0];
                _rightActiveBeams[1] = _rightPulledBeams[1];
                _rightActiveBeams[0].gameObject.SetActive(true);
                _rightActiveBeams[1].gameObject.SetActive(true);

                _rightPullBeams[0].gameObject.SetActive(false);
                _rightPullBeams[1].gameObject.SetActive(false);
                break;
        }
    }
    private void UpdateBeam(LineRenderer singleBeam, bool isLeft)
    {
        int index = isLeft ? 0 : 1;
        Vector3[] singleBeamPositions = new Vector3[2] { singleBeam.transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[index].position };
        singleBeam.SetPositions(singleBeamPositions);
    }
    private void UpdateBeam(LineRenderer outerBeam, LineRenderer innerBeam, bool isLeft)
    {
        int index = isLeft ? 0 : 1;
        Vector3[] outerBeamPositions = new Vector3[2] { outerBeam.transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[index].position };
        Vector3[] innerBeamPositions = new Vector3[2] { innerBeam.transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[index].position };

        outerBeam.SetPositions(outerBeamPositions);
        innerBeam.SetPositions(innerBeamPositions);
    }
    #endregion

    #region Spend and Indicators
    private void ThrowEnergySpend()
    {
        _currentEnergy -= _energyThrowCost;
        _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
        _player.Controller.InputHandler.PlayerWorldUI.ChangeEnergyBarLenght(_energyPercentage);
    }
    private void HoldEnergySpend()
    {
        _currentEnergy -= Time.deltaTime * _energyHoldCost;
        _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
        _player.Controller.InputHandler.PlayerWorldUI.ChangeEnergyBarLenght(_energyPercentage);
    }
    private void ReplanishEnergy()
    {
        if (_currentEnergy < _maxEnergy)
        {
            _currentEnergy += Time.deltaTime * _energyReplanishRate;
            _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
            _player.Controller.InputHandler.PlayerWorldUI.ChangeEnergyBarLenght(_energyPercentage);
        }
    }
    private void IndicateTarget(RaycastHit hit)
    {
        if (_currentTarget)
        {
            _currentTargetIndicator = Instantiate(_targetIndicatorPrefab, hit.transform);
            _currentTargetIndicator.transform.position = hit.point;
        }
    }
    #endregion

    #region Handling Left Attractor
    private void ThrowLeftAttractor()
    {
        if (_currentEnergy < _energyThrowCost || !CheckLeftHandProximity())
            return;

        //SoundManager.Instance.PlayHookSound(SoundManager.Instance.HookUseSound);

        // play throw effects
        _throwEffects[0].gameObject.SetActive(true);
        _onHandEffects[0].gameObject.SetActive(true);

        // inAir Beam
        Vector3[] leftInAirBeamsPositions = new Vector3[2] { _inAirBeams[0].transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[0].position };
        _inAirBeams[0].SetPositions(leftInAirBeamsPositions);
        _inAirBeams[0].gameObject.SetActive(true);

        for (int i = 0; i < _player.Data.ModelData.LeftHandMeshes.Length; i++)
            _player.Data.ModelData.LeftHandMeshes[i].gameObject.SetActive(false);

        if (_currentTarget)
        {
            _leftTarget = _currentTarget;
            _distanceFromLeftGrabbedObject = Vector3.Distance(transform.position, _currentTarget.position);
        }

        _leftHit = _currentHitTarget;
        _isThrowingLeftAttractor = true;
        _isUsingLeftAttractor = true;
        _leftHandTr.SetParent(transform.parent);
        _leftHandTr.gameObject.SetActive(true);

        ThrowEnergySpend();
    }
    public void ReturnLeftAttractor()
    {
        _leftHandTr.SetParent(_leftHandParent);
        _leftHit.point = _leftHandTr.position;
        _leftHandTr.position = _leftHandOriginTr.position;

        Vector3[] leftInAirBeamsPositions = new Vector3[2] { _inAirBeams[0].transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[0].position };
        _inAirBeams[0].SetPositions(leftInAirBeamsPositions);

        Vector3[] leftBaseBeamPositions = new Vector3[2] { _leftActiveBeams[0].transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[0].position };
        _leftActiveBeams[0].SetPositions(leftBaseBeamPositions);

        Vector3[] leftSecondaryBeamPositions = new Vector3[2] { _leftActiveBeams[1].transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[0].position };
        _leftActiveBeams[1].SetPositions(leftSecondaryBeamPositions);

        _isThrowingLeftAttractor = false;
        _isUsingLeftAttractor = false;
        _leftTarget = null;
        _leftGrabbedRb = null;
        _distanceFromLeftGrabbedObject = 0;
        _isLeftAttractorSnapped = false;

        _throwEffects[0].gameObject.SetActive(false);
        _onHandEffects[0].gameObject.SetActive(false);
        _inAirBeams[0].gameObject.SetActive(false);
        _leftActiveBeams[0].gameObject.SetActive(false);
        _leftActiveBeams[1].gameObject.SetActive(false);
        _leftHandTr.gameObject.SetActive(false);

        for (int i = 0; i < _player.Data.ModelData.LeftHandMeshes.Length; i++)
            _player.Data.ModelData.LeftHandMeshes[i].gameObject.SetActive(true);

        CancelAttractorLeft(true);
    }
    private void UpdateThrownLeftHandPos()
    {
        RaycastHit hit;
        Vector3 targetPosition;
        float newHandDistanceFromGrabbedObjectObject;

        if (_leftTarget)
        {
            hit = _leftHit;
            targetPosition = hit.point;
            Debug.Log("left Target Shoot");

            Vector3 direction = targetPosition - _leftHandTr.position;
            float distance = direction.magnitude;

            _leftHandTr.position = Vector3.MoveTowards(_leftHandTr.position, targetPosition, _attractorThrowForce * _throwSpeedFactor);
            _onHandEffectsRotations[0] = _leftHandTr.rotation.eulerAngles; // continue from here

            if (IsGrabbingLayer(hit.transform.gameObject) && !_isLeftAttractorSnapped && distance <= _snapDistance)
            {
                // choose what type of beam
                _isGrabbingHeavyObject = IsObjectHeavy(hit.transform.gameObject.layer, _heavyObjectLayer);
                if (_isGrabbingHeavyObject)
                    SwitchToPulledBeam(true);
                else
                    SwitchToPullBeam(true);

                // Snap the object to the target position
                _leftHandTr.position = targetPosition;
                _leftHandTr.SetParent(hit.transform);
                _isLeftAttractorSnapped = true;
                _isThrowingLeftAttractor = false;
                PoolWallSnapEffect(targetPosition, hit.transform.rotation);
                StartCoroutine(TurnOffWithDelay(_wallSnapEffect.gameObject, 2));

                if (hit.rigidbody is not null)
                    _leftGrabbedRb = hit.rigidbody;

                _onHandEffects[0].gameObject.SetActive(false);
                _inAirBeams[0].gameObject.SetActive(false);
                _leftActiveBeams[0].gameObject.SetActive(true);
                _leftActiveBeams[1].gameObject.SetActive(true);
            }
            else if (!IsGrabbingLayer(hit.transform.gameObject) || !_isLeftAttractorSnapped && distance <= _snapDistance)
            {
                newHandDistanceFromGrabbedObjectObject = Vector3.Distance(_leftHandTr.position, targetPosition);

                if (newHandDistanceFromGrabbedObjectObject < _snapDistance)
                    _leftHandState = LeftHandReturning;
            }
        }
        else
        {
            /* cancel left attractor if it reached it's max distance without a target */

            float distance = Vector3.Distance(_player.Data.ModelData.UpperArmsOrigin[0].position, _leftHandTr.position);
            if (distance >= _maxDistance - _maxDistanceReturnOffset) // low value to offset a bit before the max value for more accuracy
            {
                CancelAttractorLeft(true);
                return;
            }

            targetPosition = _aimPointTr.position + (_aimPointTr.forward * _maxDistance);
            Debug.Log("left unTarget Shoot");

            _leftHandTr.position = Vector3.MoveTowards(_leftHandTr.position, targetPosition, _attractorThrowForce * _throwSpeedFactor);

            UpdateBeam(_inAirBeams[0], true);
            /*Vector3[] leftInAirBeamPositions = new Vector3[2] { _inAirBeams[0].transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[0].position };
            _inAirBeams[0].SetPositions(leftInAirBeamPositions);*/
        }
    }
    #endregion

    #region Handling Right Attractor
    private void ThrowRightAttractor()
    {
        if (_currentEnergy < _energyThrowCost || !CheckRightHandProximity())
            return;

        //SoundManager.Instance.PlayHookSound(SoundManager.Instance.HookUseSound);

        // play throw effects
        _throwEffects[1].gameObject.SetActive(true);
        _onHandEffects[1].gameObject.SetActive(true);

        // inAir Beam
        Vector3[] rightInAirBeamsPositions = new Vector3[2] { _inAirBeams[1].transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[1].position };
        _inAirBeams[1].SetPositions(rightInAirBeamsPositions);
        _inAirBeams[1].gameObject.SetActive(true);

        for (int i = 0; i < _player.Data.ModelData.RightHandMeshes.Length; i++)
            _player.Data.ModelData.RightHandMeshes[i].gameObject.SetActive(false);

        if (_currentTarget)
        {
            _rightTarget = _currentTarget;
            _distanceFromRightGrabbedObject = Vector3.Distance(transform.position, _currentTarget.position);
        }

        _rightHit = _currentHitTarget;
        _isThrowingRightAttractor = true;
        _isUsingRightAttractor = true;
        _rightHandTr.SetParent(transform.parent);
        _rightHandTr.gameObject.SetActive(true);

        ThrowEnergySpend();
        /*ShowRightBeam();*/

        //if (!SoundManager.Instance.HookSource.isPlaying)
        //    SoundManager.Instance.PlayHookSound(SoundManager.Instance.HookUseSound);

    }
    public void ReturnRightAttractor()
    {
        _rightHandTr.SetParent(_rightHandParent);
        _rightHit.point = _rightHandTr.position;
        _rightHandTr.position = _rightHandOriginTr.position;

        Vector3[] rightInAirBeamsPositions = new Vector3[2] { _inAirBeams[1].transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[1].position };
        _inAirBeams[1].SetPositions(rightInAirBeamsPositions);

        Vector3[] rightBaseBeamPositions = new Vector3[2] { _rightActiveBeams[0].transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[1].position };
        _rightActiveBeams[0].SetPositions(rightBaseBeamPositions);
        
        Vector3[] rightSecondaryBeamPositions = new Vector3[2] { _rightActiveBeams[1].transform.parent.position, _player.Data.ModelData.UpperArmsOrigin[1].position };
        _rightActiveBeams[1].SetPositions(rightSecondaryBeamPositions);

        _isThrowingRightAttractor = false;
        _isUsingRightAttractor = false;
        _rightTarget = null;
        _rightGrabbedRb = null;
        _distanceFromRightGrabbedObject = 0;
        _isRightAttractorSnapped = false;

        _throwEffects[1].gameObject.SetActive(false);
        _onHandEffects[1].gameObject.SetActive(false);
        _inAirBeams[1].gameObject.SetActive(false);
        _rightActiveBeams[0].gameObject.SetActive(false);
        _rightActiveBeams[1].gameObject.SetActive(false);
        _rightHandTr.gameObject.SetActive(false);

        for (int i = 0; i < _player.Data.ModelData.RightHandMeshes.Length; i++)
            _player.Data.ModelData.RightHandMeshes[i].gameObject.SetActive(true);

        CancelAttractorRight(true);
    }
    private void UpdateThrownRightHandPos()
    {
        RaycastHit hit;
        Vector3 targetPosition;
        float newHandDistanceFromGrabbedObjectObject;

        if (_rightTarget)
        {
            hit = _rightHit;
            targetPosition = hit.point;
            Debug.Log("right Target Shoot");

            Vector3 direction = targetPosition - _rightHandTr.position;
            float distance = direction.magnitude;

            _rightHandTr.position = Vector3.MoveTowards(_rightHandTr.position, targetPosition, _attractorThrowForce * _throwSpeedFactor);

            if (IsGrabbingLayer(hit.transform.gameObject) && !_isRightAttractorSnapped && distance <= _snapDistance)
            {
                // choose what type of beam
                _isGrabbingHeavyObject = IsObjectHeavy(hit.transform.gameObject.layer, _heavyObjectLayer);
                if (_isGrabbingHeavyObject)
                    SwitchToPulledBeam(false);
                else
                    SwitchToPullBeam(false);

                // Snap the object to the target position
                _rightHandTr.position = targetPosition;
                _rightHandTr.SetParent(hit.transform);
                _isRightAttractorSnapped = true;
                _isThrowingRightAttractor = false;
                PoolWallSnapEffect(targetPosition, hit.transform.rotation);
                StartCoroutine(TurnOffWithDelay(_wallSnapEffect.gameObject, 2));

                if (hit.rigidbody is not null)
                    _rightGrabbedRb = hit.rigidbody;

                _onHandEffects[1].gameObject.SetActive(false);
                _inAirBeams[1].gameObject.SetActive(false);
                _rightActiveBeams[0].gameObject.SetActive(true);
                _rightActiveBeams[1].gameObject.SetActive(true);
            }
            else if (!IsGrabbingLayer(hit.transform.gameObject) || !_isRightAttractorSnapped && distance <= _snapDistance)
            {
                newHandDistanceFromGrabbedObjectObject = Vector3.Distance(_rightHandTr.position, targetPosition);

                if (newHandDistanceFromGrabbedObjectObject < _snapDistance)
                    _rightHandState = RightHandReturning;
            }
        }
        else
        {
            /* cancel left attractor if it reached it's max distance without a target */

            float distance = Vector3.Distance(_player.Data.ModelData.UpperArmsOrigin[1].position, _rightHandTr.position);
            if (distance >= _maxDistance - _maxDistanceReturnOffset) // low value to offset a bit before the max value for more accuracy
            {
                CancelAttractorRight(true);
                return;
            }

            targetPosition = _aimPointTr.position + (_aimPointTr.forward * _maxDistance);
            Debug.Log("left unTarget Shoot");

            _rightHandTr.position = Vector3.MoveTowards(_rightHandTr.position, targetPosition, _attractorThrowForce * _throwSpeedFactor);
            UpdateBeam(_inAirBeams[1], false);
        }
    }
    #endregion

    #region Change Left Attractor States
    public void StartAttractorLeft(bool isPressed)
    {
        _isLeftPressed = isPressed;
        _isLeftCanceled = !isPressed;
    }
    public void CancelAttractorLeft(bool isCanceled)
    {
        _isLeftCanceled = isCanceled;
        _isLeftPressed = !isCanceled;
    }
    public void DisableAttractor(bool shouldDisable)
    {
        _isAttractorsDisabled = shouldDisable;

        if (shouldDisable)
            return;

        _currentEnergy = _maxEnergy;
        _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
        _player.Controller.InputHandler.PlayerWorldUI.ChangeEnergyBarLenght(_energyPercentage);
    }
    #endregion

    #region Change Right Attractor States
    public void StartAttractorRight(bool isPressed)
    {
        _isRightPressed = isPressed;
        _isRightCanceled = !isPressed;
    }
    public void CancelAttractorRight(bool isCanceled)
    {
        _isRightCanceled = isCanceled;
        _isRightPressed = !isCanceled;
    }
    #endregion

    #region Effects
    private void PoolWallSnapEffect(Vector3 pos, Quaternion rot)
    {
        _wallSnapEffect.transform.SetParent(null);
        _wallSnapEffect.gameObject.SetActive(true);
        _wallSnapEffect.transform.SetPositionAndRotation(pos, rot);
    }
    private ParticleSystem CreateWallSnapEffect()
    {
        GameObject newWallSnapEffectGO = Instantiate(_wallSnapEffectPrefab.gameObject, _vfxParent);
        ParticleSystem wallSnapEffect = newWallSnapEffectGO.GetComponent<ParticleSystem>();
        ParticleSystemRenderer particleRenderer = wallSnapEffect.GetComponent<ParticleSystemRenderer>();
        ParticleSystem.MainModule mainModule = wallSnapEffect.main;
        mainModule.startColor = _player.SetupData.ColorData.BaseEmissionColor;
        Material coloredMat = new Material(particleRenderer.trailMaterial);
        coloredMat.SetColor("_EmissionColor", _player.SetupData.ColorData.BaseEmissionColor);
        particleRenderer.trailMaterial = coloredMat;
        return wallSnapEffect;
    }
    #endregion

    #region Misc
    private IEnumerator DestroyWithDelay(GameObject objectToDestroy, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(objectToDestroy);
    }
    private IEnumerator TurnOffWithDelay(GameObject objectToTurnOff, float time)
    {
        yield return new WaitForSeconds(time);
        objectToTurnOff.SetActive(false);
        objectToTurnOff.transform.SetParent(_vfxParent);
    }
    #endregion

    #region Gizmos
    private void DrawAim(Vector3 originPos, Quaternion originRotation, float distance)
    {
        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(originPos, originRotation, Vector3.one);
        Gizmos.DrawWireCube(new Vector3(0, 0, distance / 2), new Vector3(1f, 1f, distance));
    }
    private void OnDrawGizmos()
    {
        DrawAim(_aimPointTr.position, _player.Controller.CrosshairParent.rotation, _maxDistance);
    }
    #endregion

    /*private void AimBeam()
    {
        if (_isLeftBeamOnline)
        {
            _player.Data.HandBeams[0][0].transform.LookAt(_player.Data.HandBeams[0][1].transform);
            _player.Data.HandBeams[0][1].transform.LookAt(_player.Data.HandBeams[0][0].transform.position);
        }

        if (_isRightBeamOnline)
        {
            _player.Data.HandBeams[1][0].transform.LookAt(_player.Data.HandBeams[1][1].transform);
            _player.Data.HandBeams[1][1].transform.LookAt(_player.Data.HandBeams[1][0].transform.position);
        }
    }*/
}
