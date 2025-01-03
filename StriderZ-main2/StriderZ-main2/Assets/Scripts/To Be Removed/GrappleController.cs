using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    #region Grapple Data
    [Header("Grapple Data")]
    [SerializeField] private GameObject _grapplePrefab;
    [SerializeField] private LayerMask _grappleHitLayer;

    [SerializeField] private Color _playerColor;
    public Color PlayerColor { get => _playerColor; set => _playerColor = value; }

    [SerializeField] private float _currentEnergy, _energyPercentage;
    [SerializeField] private float _maxEnergy = 15.0f, _throwEnergyCost = 3.0f, _energyDepleteRate = 2.0f, _energyRecoveryRate = 3.0f;
    [SerializeField] private float _grappleDistance = 90.0f, _throwGrappleForce = 60.0f, _grappleSpeed = 60.0f, _grappleReturnSpeed = 0.1f,  _grappleShootSpeed = 0.1f;
    [SerializeField] private float _playerGrappleMinDistance = 2f, _distanceToStopReeling = 12.5f, _grabbedPlayerAddedForce = 10.0f;

    [SerializeField] private bool _isGrabbingDynamicObject = false;
    public bool IsGrapplingPlayer => _isGrabbingDynamicObject;

    private float _grappableDetectorWidth = 1f, _grappableDetectorHeight = 1f;
    #endregion

    #region Grapple Component
    [Header("Grapple Components")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _leftGrappleOriginTr, _rightGrappleOriginTr;
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private Material _grappleMat;

    private Rigidbody _leftGrappledPlayerRb, _rightGrappledPlayerRb;
    public Rigidbody LeftGrappledPlayerRb => _leftGrappledPlayerRb;
    public Rigidbody RightGrappledPlayerRb => _rightGrappledPlayerRb;
    
    private Transform _leftGrappleTip, _rightGrappleTip;
    private LineRenderer _leftGrappleLr, _rightGrappleLr;

    private int _leftGrappleTriggerCounter = 0, _rightGrappleTriggerCounter = 0;
    private Vector3 _posBeforeGrappling, _leftGrappleEndPoint, _rightGrappleEndPoint;
    private Vector3 _leftVisualizeStartFromEndPoint, _rightVisualizeStartFromEndPoint;
    private Vector3 _lastLeftAnchoredPos = Vector3.up, _lastRightAnchoredPos = Vector3.up, _lastLeftAnchoredDir = Vector3.zero, _lastRightAnchoredDir = Vector3.zero;

    private bool _isRightBeamActive = false, _isLeftBeamActive = false;
    private bool _hasLeftGrappleHitTarget = false, _hasRightGrappleHitTarget = false;
    private bool _isLeftGrappleExisting = false, _isRightGrappleExisting = false;
    private bool _isUsingLeftGrapple = false, _isUsingRightGrapple = false;
    private bool _isGrappleLeftAttached = false, _isGrappleRightAttached = false;
    private bool _isOtherPlayerGotReeled = false;
    private delegate void PhysicsState(Vector3 lastAnchoredPlayerDir);
    private PhysicsState _attachedDynamicState;
    #endregion

    [SerializeField] private bool _isDebugMessagesOn;

    private void Awake()
    {
        // check left grapple in order to set standard for both.
        _leftGrappleTip = _grapplePrefab.transform.GetChild(0);
        _grappableDetectorWidth = _leftGrappleTip.localScale.x;
        _grappableDetectorHeight = _leftGrappleTip.localScale.y;
        _leftGrappleTip = null;
        _currentEnergy = _maxEnergy;
        _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
    }
    private void Update()
    {
        if (_playerController.IsReady && !_isUsingLeftGrapple && !_isUsingRightGrapple  && _currentEnergy < _maxEnergy)
        {
            _currentEnergy += Time.deltaTime * _energyRecoveryRate;
            _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
            //UIManager.Instance.ChangeEnergyBarLenght(_playerController.InputHandler.Config.ID, _energyPercentage);
            _playerController.InputHandler.PlayerWorldUI.ChangeEnergyBarLenght(_energyPercentage);
        }
    }
    private void FixedUpdate()
    {
        if (!_leftGrappleLr && !_rightGrappleLr)
            _posBeforeGrappling = _playerRb.position;

        if (_lastLeftAnchoredPos != Vector3.up && _isGrappleLeftAttached)
            LeftGrapplePullFixed();

        if (_lastRightAnchoredPos != Vector3.up && _isGrappleRightAttached)
            RightGrapplePullFixed();
    }

    private void NullState()
    {

    }
    private void ThrowOnTarget(Vector3 lastAnchoredPlayerDir)
    {
        _leftGrappledPlayerRb.AddForce(lastAnchoredPlayerDir.normalized * _throwGrappleForce, ForceMode.Impulse);
    }
    private void TargetWasCloseNowFar(Vector3 lastAnchoredPlayerDir)
    {
        CancelGrappleLeft();
        _isOtherPlayerGotReeled = false;
    }
    private void TargetFar(Vector3 lastAnchoredPlayerDir)
    {
        _leftGrappledPlayerRb.AddForce(lastAnchoredPlayerDir.normalized * _grappleSpeed, ForceMode.Force);
    }
    private void TargetClose(Vector3 lastAnchoredPlayerDir)
    {
        _isOtherPlayerGotReeled = true;

        //Vector3 desiredPosition = _playerRb.position + _lastAnchoredPlayerDir.normalized * _distanceToStopReeling;
        //Vector3 pushForce = (lastAnchoredPlayerDir.position - desiredPosition).normalized * _grabbedPlayerAddedForce;
        //_leftGrappledPlayerRb.AddForce(pushForce, ForceMode.Force);
    }
    
    private void LeftGrapplePullFixed()
    {
        float distanceFromAnchor = Vector3.Distance(_playerRb.position, _lastLeftAnchoredPos);

        if ((!_leftGrappledPlayerRb || !_isGrabbingDynamicObject) && distanceFromAnchor > _distanceToStopReeling)
        {
            if (_leftGrappleTriggerCounter == 0)
                _playerRb.AddForce(_lastLeftAnchoredDir.normalized * _throwGrappleForce, ForceMode.Impulse);
            else if (_leftGrappleTriggerCounter > 0)
                _playerRb.AddForce(_lastLeftAnchoredDir.normalized * _grappleSpeed, ForceMode.Acceleration);
        }
        else
        {
            Vector3 _lastAnchoredPlayerDir = transform.position - _leftGrappledPlayerRb.transform.position;
            
            if (_isUsingLeftGrapple && _leftGrappleLr && _isLeftBeamActive)
            {
                _leftGrappleLr.SetPosition(0, _leftGrappleOriginTr.position);
                _leftGrappleLr.SetPosition(1, _leftGrappledPlayerRb.position);
                _leftGrappleEndPoint = _leftGrappledPlayerRb.position;
                _lastLeftAnchoredPos = _leftGrappleEndPoint;
                _leftGrappleTip.position = _leftGrappleEndPoint;
            }

            distanceFromAnchor = Vector3.Distance(_playerRb.position, _lastLeftAnchoredPos);
            Vector3 desiredPosition = _playerRb.position + _lastAnchoredPlayerDir.normalized * _distanceToStopReeling;

            if (_leftGrappleTriggerCounter == 0)
            {
                _leftGrappledPlayerRb.AddForce(_lastAnchoredPlayerDir.normalized * _throwGrappleForce, ForceMode.Impulse);
            }
            else if (_isOtherPlayerGotReeled && distanceFromAnchor > _distanceToStopReeling * 3)
            {
                CancelGrappleLeft();
                _isOtherPlayerGotReeled = false;
            }
            else if (_leftGrappleTriggerCounter > 0 && distanceFromAnchor > _distanceToStopReeling)
            {
                _leftGrappledPlayerRb.AddForce(_lastAnchoredPlayerDir.normalized * _grappleSpeed, ForceMode.Force);
            }
            else if (_leftGrappleTriggerCounter > 0 && distanceFromAnchor < _distanceToStopReeling)
            {
                _isOtherPlayerGotReeled = true;

                Vector3 pushForce = (_leftGrappledPlayerRb.position - desiredPosition).normalized * _grabbedPlayerAddedForce;
                _leftGrappledPlayerRb.AddForce(pushForce, ForceMode.Force);
            }
        }

        Vector3 newPlayerPos = _playerRb.position;
        newPlayerPos.y = _posBeforeGrappling.y;
        _playerRb.position = newPlayerPos;

        _leftGrappleTriggerCounter++;
    }
    private void RightGrapplePullFixed()
    {
        float distanceFromAnchor = Vector3.Distance(_playerRb.position, _lastRightAnchoredPos);

        if ((!_rightGrappledPlayerRb || !_isGrabbingDynamicObject) && distanceFromAnchor > _distanceToStopReeling)
        {
            if (_rightGrappleTriggerCounter == 0)
                _playerRb.AddForce(_lastRightAnchoredDir.normalized * _throwGrappleForce, ForceMode.Impulse);
            else if (_rightGrappleTriggerCounter > 0)
                _playerRb.AddForce(_lastRightAnchoredDir.normalized * _grappleSpeed, ForceMode.Force);
        }
        else
        {
            Vector3 _lastAnchoredPlayerDir = transform.position - _rightGrappledPlayerRb.transform.position;

            if (_isUsingRightGrapple && _rightGrappleLr && _isRightBeamActive)
            {
                _rightGrappleLr.SetPosition(0, _rightGrappleOriginTr.position);
                _rightGrappleLr.SetPosition(1, _rightGrappledPlayerRb.position);
                _rightGrappleEndPoint = _rightGrappledPlayerRb.position;
                _lastRightAnchoredPos = _rightGrappleEndPoint;
                _rightGrappleTip.position = _rightGrappleEndPoint;
            }

            if (_rightGrappleTriggerCounter == 0)
                _rightGrappledPlayerRb.AddForce(_lastAnchoredPlayerDir.normalized * _throwGrappleForce, ForceMode.Impulse);
            else if (_rightGrappleTriggerCounter > 0)
                _rightGrappledPlayerRb.AddForce(_lastAnchoredPlayerDir.normalized * _grappleSpeed, ForceMode.Force);
        }

        Vector3 newPlayerPos = _playerRb.position;
        newPlayerPos.y = _posBeforeGrappling.y;
        _playerRb.position = newPlayerPos;

        _rightGrappleTriggerCounter++;
    }

    public void StartGrappleLeft()
    {
        if (_isUsingLeftGrapple || _currentEnergy <= _throwEnergyCost)
            return;

        RaycastHit hit;

        Vector3 targetPos;
        targetPos = _leftGrappleOriginTr.position + _leftGrappleOriginTr.forward * _grappleDistance;

        float halfDistance = _grappleDistance / 2;

        if (_playerController.IsReady)
        {
            _currentEnergy -= _throwEnergyCost;
            _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
            //UIManager.Instance.ChangeEnergyBarLenght(_playerController.InputHandler.Config.ID, _energyPercentage);
            _playerController.InputHandler.PlayerWorldUI.ChangeEnergyBarLenght(_energyPercentage);
        }

        //if (!SoundManager.Instance.HookSource.isPlaying)
        //    SoundManager.Instance.PlayHookSound(SoundManager.Instance.HookUseSound);

        if (Physics.BoxCast(_leftGrappleOriginTr.position, new(_grappableDetectorWidth / 2.0f, _grappableDetectorHeight / 2.0f, halfDistance / 10.0f), _leftGrappleOriginTr.forward, out hit, _playerController.CrosshairParent.rotation, _grappleDistance, _grappleHitLayer))
        {
            targetPos = hit.point;
            _hasLeftGrappleHitTarget = true;
            Debug.Log("New Grapple Left Hit!");


            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("GrappableDynamicObject"))
            {
                Debug.Log("New Grapple Left Hit Player!");

                _leftGrappledPlayerRb = hit.collider.GetComponentInParent<Rigidbody>();
                _isGrabbingDynamicObject = true;
            }
            else
            {
                Debug.Log("New Grapple Left Didn't Hit Player!");

                if (_leftGrappledPlayerRb)
                    _leftGrappledPlayerRb = null;

                _isGrabbingDynamicObject = false;
            }

            Debug.Log("Hit grapple created");
            SetLeftGrappleLR(targetPos);
        }
        else
        {
            _hasLeftGrappleHitTarget = false;
            Debug.Log("Unhit grapple created");
            SetLeftGrappleLR(targetPos);
        }

        _isUsingLeftGrapple = true;
    }
    public void PerformGrappleLeft()
    {
        if (_isUsingLeftGrapple && _leftGrappleLr && _isLeftBeamActive)
        {
            _leftGrappleLr.SetPosition(0, _leftGrappleOriginTr.position);

            if (_playerController.IsReady)
            {
                _currentEnergy -= Time.deltaTime * _energyDepleteRate;
                _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
                //UIManager.Instance.ChangeEnergyBarLenght(_playerController.InputHandler.Config.ID, _energyPercentage);
                _playerController.InputHandler.PlayerWorldUI.ChangeEnergyBarLenght(_energyPercentage);
            }

            if (_currentEnergy <= 0)
            {
                CancelGrappleLeft();
                return;
            }
        }
        else
        {
            Debug.Log("No LineRenderer");
            return;
        }

        _lastLeftAnchoredDir = _lastLeftAnchoredPos - transform.position;
    }
    public void CancelGrappleLeft()
    {
        //SoundManager.Instance.PlayHookSound(SoundManager.Instance.HookReleaseSound);
        _lastLeftAnchoredPos = Vector3.up;
        _lastLeftAnchoredDir = Vector3.zero;
        _leftGrappleTriggerCounter = 0;

        if (_leftGrappleLr)
        {
            Destroy(_leftGrappleTip.gameObject);
            Destroy(_leftGrappleLr.gameObject);
        }

        if (_leftGrappledPlayerRb)
            _leftGrappledPlayerRb = null;

        _isLeftGrappleExisting = false;
        _isUsingLeftGrapple = false;
        _isGrappleLeftAttached = false;
        _isLeftBeamActive = false;
    }

    public void StartGrappleRight()
    {
        if (_isUsingRightGrapple || _currentEnergy <= _throwEnergyCost)
            return;

        RaycastHit hit;
        Vector3 targetPos;

        targetPos = _rightGrappleOriginTr.position + _rightGrappleOriginTr.forward * _grappleDistance;

        float halfDistance = _grappleDistance / 2.0f;

        if (_playerController.IsReady)
        {
            _currentEnergy -= _throwEnergyCost;
            _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
            //UIManager.Instance.ChangeEnergyBarLenght(_playerController.InputHandler.Config.ID, _energyPercentage);
            _playerController.InputHandler.PlayerWorldUI.ChangeEnergyBarLenght(_energyPercentage);
        }

        //if (!SoundManager.Instance.HookSource.isPlaying)
        //    SoundManager.Instance.PlayHookSound(SoundManager.Instance.HookUseSound);

        if (Physics.BoxCast(_rightGrappleOriginTr.position, new(_grappableDetectorWidth / 2.0f, _grappableDetectorHeight / 2.0f, halfDistance / 10.0f), _rightGrappleOriginTr.forward, out hit, _playerController.CrosshairParent.rotation, _grappleDistance, _grappleHitLayer))
        {
            //SoundManager.Instance.PlayHookSound(SoundManager.Instance.HookHitSound);
            targetPos = hit.point;
            _hasRightGrappleHitTarget = true;
            Debug.Log("New Grapple Right Hit!");


            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("GrappableDynamicObject"))
            {
                Debug.Log("New Grapple Right Hit Player!");

                _rightGrappledPlayerRb = hit.collider.GetComponentInParent<Rigidbody>();
                _isGrabbingDynamicObject = true;
            }
            else
            {
                Debug.Log("New Grapple Right Didn't Hit Player!");

                if (_rightGrappledPlayerRb)
                    _rightGrappledPlayerRb = null;

                _isGrabbingDynamicObject = false;
            }

            Debug.Log("Hit grapple created");
            SetRightGrappleLR(targetPos);
        }
        else
        {
            _hasRightGrappleHitTarget = false;
            Debug.Log("Unhit grapple created");
            SetRightGrappleLR(targetPos);
        }

        _isUsingRightGrapple = true;
    }
    public void PerformGrappleRight()
    {
        if (_isUsingRightGrapple && _rightGrappleLr && _isRightBeamActive)
        {
            _rightGrappleLr.SetPosition(0, _rightGrappleOriginTr.position);

            if (_playerController.IsReady)
            {
                _currentEnergy -= Time.deltaTime * _energyDepleteRate;
                _energyPercentage = (_currentEnergy / _maxEnergy) * 100;
                //UIManager.Instance.ChangeEnergyBarLenght(_playerController.InputHandler.Config.ID, _energyPercentage);
                _playerController.InputHandler.PlayerWorldUI.ChangeEnergyBarLenght(_energyPercentage);
            }

            if (_currentEnergy <= 0)
            {
                CancelGrappleRight();
                return;
            }
        }
        else
        {
            Debug.Log("No LineRenderer");
            return;
        }

        _lastRightAnchoredDir = _lastRightAnchoredPos - transform.position;
    }
    public void CancelGrappleRight()
    {
        //SoundManager.Instance.PlayHookSound(SoundManager.Instance.HookReleaseSound);/
        _lastRightAnchoredPos = Vector3.up;
        _lastRightAnchoredDir = Vector3.zero;
        _rightGrappleTriggerCounter = 0;

        if (_rightGrappleLr)
        {
            Destroy(_rightGrappleTip.gameObject);
            Destroy(_rightGrappleLr.gameObject);
        }

        if (_rightGrappledPlayerRb)
            _rightGrappledPlayerRb = null;

        _isRightGrappleExisting = false;
        _isUsingRightGrapple = false;
        _isGrappleRightAttached = false;
        _isRightBeamActive = false;
    }

    private void SetLeftGrappleLR(Vector3 targetPos)
    {
        if (_isLeftGrappleExisting)
            return;

        // Create Line -------------------------------
        _isLeftGrappleExisting = true;
        GameObject leftGrapple = Instantiate(_grapplePrefab, _leftGrappleOriginTr, true);
        _leftGrappleLr = leftGrapple.GetComponent<LineRenderer>();
        _leftGrappleTip = leftGrapple.transform.GetChild(0);

        // Set Line -------------------------------
        _leftGrappleLr.material.color = Color.white;
        _leftGrappleLr.material.SetColor("_EmissionColor", Color.white);
        _leftGrappleEndPoint = _leftGrappleOriginTr.position;
        _leftGrappleTip.position = _leftGrappleEndPoint;
        StartCoroutine(ThrowLeftLineRendererEndPoint(targetPos, _grappleShootSpeed));

        if (_hasLeftGrappleHitTarget)
        {
            _leftGrappleLr.material.color = _playerColor;
            _leftGrappleLr.material.SetColor("_EmissionColor", _playerColor);
            _lastLeftAnchoredPos = targetPos;
            _lastLeftAnchoredDir = _lastLeftAnchoredPos - transform.position;
            _leftGrappleTip.parent = null;
        }
        // ----------------------------------------
    }
    private void SetRightGrappleLR(Vector3 targetPos)
    {
        if (_isRightGrappleExisting)
            return;

        // Create Line -------------------------------
        _isRightGrappleExisting = true;
        GameObject rightGrapple = Instantiate(_grapplePrefab, _rightGrappleOriginTr, true);
        _rightGrappleLr = rightGrapple.GetComponent<LineRenderer>();
        _rightGrappleTip = rightGrapple.transform.GetChild(0);

        // Set Line -------------------------------
        _rightGrappleLr.material.color = Color.white;
        _rightGrappleLr.material.SetColor("_EmissionColor", Color.white);
        _rightGrappleEndPoint = _rightGrappleOriginTr.position;
        _rightGrappleTip.position = _rightGrappleEndPoint;
        StartCoroutine(ThrowRightLineRendererEndPoint(targetPos, _grappleShootSpeed));

        if (_hasRightGrappleHitTarget)
        {
            _rightGrappleLr.material.color = _playerColor;
            _rightGrappleLr.material.SetColor("_EmissionColor", _playerColor);
            _lastRightAnchoredPos = targetPos;
            _lastRightAnchoredDir = _lastRightAnchoredPos - transform.position;
            _rightGrappleTip.parent = null;
        }
        // ----------------------------------------
    }

    private IEnumerator ThrowLeftLineRendererEndPoint(Vector3 targetPos, float duration)
    {
        float time = 0;
        Vector3 startPos = _leftGrappleOriginTr.position;
        while (_leftGrappleLr && time < duration)
        {
            _leftGrappleEndPoint = Vector3.Lerp(startPos, targetPos, time / duration);
            _leftGrappleTip.position = _leftGrappleEndPoint;

            if (_hasLeftGrappleHitTarget)
                _lastLeftAnchoredPos = _leftGrappleEndPoint;

            time += Time.deltaTime;
            yield return null;
        }

        if (_leftGrappleLr)
        {
            _leftGrappleEndPoint = targetPos;
            _leftGrappleTip.position = _leftGrappleEndPoint;

            if (_hasLeftGrappleHitTarget)
            {
                _lastLeftAnchoredPos = _leftGrappleEndPoint;
                _isGrappleLeftAttached = true;
                StartCoroutine(VisualizeLeftGrapple(targetPos, duration));
                _isLeftBeamActive = true;
            }
            else
                CancelGrappleLeft();
        }
    }
    private IEnumerator VisualizeLeftGrapple(Vector3 targetPos, float duration)
    {
        float time = 0;
        _leftGrappleLr.SetPosition(0, _leftGrappleEndPoint);
        _leftGrappleLr.SetPosition(1, _leftGrappleEndPoint);

        while (_leftGrappleLr && time < duration)
        {
            _leftVisualizeStartFromEndPoint = Vector3.Lerp(targetPos, _leftGrappleOriginTr.position, time / duration);
            _leftGrappleLr.SetPosition(0, _leftVisualizeStartFromEndPoint);

            time += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator ThrowRightLineRendererEndPoint(Vector3 targetPos, float duration)
    {
        float time = 0;
        Vector3 startPos = _rightGrappleOriginTr.position;
        while (_rightGrappleLr && time < duration)
        {
            _rightGrappleEndPoint = Vector3.Lerp(startPos, targetPos, time / duration);
            _rightGrappleTip.position = _rightGrappleEndPoint;

            if (_hasRightGrappleHitTarget)
                _lastRightAnchoredPos = _rightGrappleEndPoint;

            time += Time.deltaTime;
            yield return null;
        }

        if (_rightGrappleLr)
        {
            _rightGrappleEndPoint = targetPos;
            _rightGrappleTip.position = _rightGrappleEndPoint;

            if (_hasRightGrappleHitTarget)
            {
                _lastRightAnchoredPos = _rightGrappleEndPoint;
                _isGrappleRightAttached = true;
                StartCoroutine(VisualizeRightGrapple(targetPos, duration));
                _isRightBeamActive = true;
            }
            else
                CancelGrappleRight();
        } 
    }
    private IEnumerator VisualizeRightGrapple(Vector3 targetPos, float duration)
    {
        float time = 0;
        _rightGrappleLr.SetPosition(0, _rightGrappleEndPoint);
        _rightGrappleLr.SetPosition(1, _rightGrappleEndPoint);

        while (_rightGrappleLr && time < duration)
        {
            _rightVisualizeStartFromEndPoint = Vector3.Lerp(targetPos, _rightGrappleOriginTr.position, time / duration);
            _rightGrappleLr.SetPosition(0, _rightVisualizeStartFromEndPoint);

            time += Time.deltaTime;
            yield return null;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.cyan;
    //
    //    Gizmos.matrix = Matrix4x4.TRS(_leftGrappleOriginTr.position, _leftGrappleOriginTr.rotation, Vector3.one);
    //    //Gizmos.matrix = Matrix4x4.TRS(_rightGrappleOriginTr.position, _rightGrappleOriginTr.rotation, Vector3.one);
    //
    //    Gizmos.DrawWireCube(new Vector3(0, 0, _grappleDistance / 2), new Vector3(1f, 1f, _grappleDistance));
    //
    //    if (_hasLeftGrappleHitTarget || _hasRightGrappleHitTarget)
    //        Gizmos.color = Color.red;
    //    else
    //        Gizmos.color = Color.cyan;
    //}
}
