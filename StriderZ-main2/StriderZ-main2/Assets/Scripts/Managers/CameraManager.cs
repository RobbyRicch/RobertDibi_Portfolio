using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance => _instance;

    private delegate void CameraState();
    private CameraState _cameraState;

    #region Properties
    [Header("Properties")]
    [SerializeField] private Camera _mainCam;
    public Camera MainCam => _mainCam;

    private Vector3 _deathLineAndGroundContactPoint = Vector3.zero;
    public Vector3 DeathLineAndGroundContactPoint => _deathLineAndGroundContactPoint;

    [SerializeField] private float _closeUpDuration = 2f;
    public float CloseUpDuration => _closeUpDuration;

    [SerializeField] private float _winZoomCameraHeight = 12.5f, _winZoomCameraTilt = 30.0f;
    public float WinZoomCameraHeight => _winZoomCameraHeight;
    public float WinZoomCameraTilt => _winZoomCameraTilt;

    private bool _isScreenShaking = false, _isZommingToLastPlayer = false, _isReturningToOriginLocation = false, _shouldFollowVictor = false;
    public bool IsScreenShaking => _isScreenShaking;
    public bool IsClosingOnPlayer => _isZommingToLastPlayer;
    public bool IsReturningToOriginLocation => _isReturningToOriginLocation;
    public bool ShouldFollowVictor { get => _shouldFollowVictor; set => _shouldFollowVictor = value; }
    #endregion

    #region Private Fields
    [Header("Camera Data")]
    [SerializeField] private Transform _cameraStopTransform;
    [SerializeField] private LayerMask _scrumbleRaycastLayer;
    [SerializeField] private Vector3 _cameraOffset = new(0.0f, 55.0f, -35.0f);
    [SerializeField] private Vector3 _arenaCameraOffset = new(0.0f, 55.0f, -90.0f);
    [SerializeField] private float _deepenDistance = 35f, _cameraTargetDepth = -60.0f, _timeToDeepen = 3.0f;
    [SerializeField] private float _rayDistance = 100f;
    [SerializeField] private float _cameraResetDelayInSeconds = 1.0f;

    [Header("Camera Movement Data")]
    [SerializeField] private float _cameraMoveDelay = 0.5f;
    [SerializeField] private float _minZoom = 55.0f, _maxZoom = 85.0f, _maxZoomDistance = 60.0f;
    [SerializeField] private float _arenaMinZoom = 55.0f, _arenaMaxZoom = 85.0f, _arenaMaxZoomDistance = 60.0f;

    /*[Header("Camera Arena Transition")]
    [SerializeField] private float _arenaTransitionDuration = 2.0f;*/

    [Header("Camera Victory Sequence Data")]
    [SerializeField] private float _returnAfterVictoryDelay = 2f;
    [SerializeField] private float _panSpeed = 10f, _slowMotionScale = 0.5f, _maxCameraHeight = 25.0f, _zCameraOffset = 10.0f, _cameraReturnDelay = 2f;

    private List<Transform> _targets;
    private Ray _cameraRay;
    private Vector3 _cameraOriginPos/*, _cameraStopPos*/;
    private Vector3 _targetsCenterPos, _cameraVelocity;
    private Vector3 _previousPosition, _currentVelocity;
    private float _velocityMagnitude;
    private float _greatestDistanceBetweenPlayers;
    private bool _hasDeepen = false;
    #endregion

    #region Debug
    [Header("Debug")]
    [SerializeField] private bool _isDebugMessagesOn;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        _instance = this;
        Initialize();
    }
    private void OnEnable()
    {
        /*EventManager.OnGameStart += OnGameStart;
        EventManager.OnRoundEnd += OnRoundEnd;
        EventManager.OnRoundEndWithDraw += OnRoundEndWithDraw;
        EventManager.OnRoundSetUp += OnRoundSetUp;
        EventManager.OnRoundStart += OnRoundStart;*/
    }
    private void LateUpdate()
    {
        _cameraState.Invoke();
        CalculateVelocity();
    }
    private void OnDisable()
    {
        /*EventManager.OnGameStart -= OnGameStart;
        EventManager.OnRoundEnd -= OnRoundEnd;
        EventManager.OnRoundEndWithDraw -= OnRoundEndWithDraw;
        EventManager.OnRoundSetUp -= OnRoundSetUp;
        EventManager.OnRoundStart -= OnRoundStart;*/
    }
    #endregion

    #region Camera States
    private void NeutralState()
    {
        if (_isDebugMessagesOn) Debug.Log("Camera in Neutral State");

    }
    private void RaceState()
    {
        if (_isDebugMessagesOn) Debug.Log("Camera in Race State");

        PopulateCameraTargets();
        MoveCamera();
        Zoom();
    }
    private void ArenaState()
    {
        if (_isDebugMessagesOn) Debug.Log("Camera in Arena State");

        PopulateCameraTargets();
        MoveCamera();
        ArenaZoom();

        /*if (!GroundScrumbleManager.Instance.IsArenaScrumbling)
            GroundScrumbleManager.Instance.ScrumbleArena();*/
    }
    #endregion

    private void Initialize()
    {
        _targets = new();
        _cameraOriginPos = _mainCam.transform.position;
        _previousPosition = transform.position;
        /*_cameraStopPos = _cameraStopTransform.position;*/
        _cameraState = NeutralState;
    }

    #region Camera Private Calculations
    private void PopulateCameraTargets()
    {
        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        if (_targets.Count != allPlayersAlive.Count)
        {
            _targets.Clear();
            for (int i = 0; i < allPlayersAlive.Count; i++)
            {
                if (!_targets.Contains(allPlayersAlive[i].transform))
                    _targets.Add(allPlayersAlive[i].transform);
            }
        }
    }
    private void SetTargetsCenterPointAndGreatesetDistanceByVertical()
    {
        if (_targets.Count == 1)
            _targetsCenterPos = _targets[0].position;

        Bounds bounds = new Bounds(_targets[0].position, Vector3.zero);

        for (int i = 0; i < _targets.Count; i++)
        {
            bounds.Encapsulate(_targets[i].position);
            _greatestDistanceBetweenPlayers = bounds.size.z; // get greatest distance between players
        }

        _targetsCenterPos = bounds.center;
    }
    private void SetTargetsCenterPointAndGreatesetDistanceByHorizontal()
    {
        if (_targets.Count == 1)
            _targetsCenterPos = _targets[0].position;

        Bounds bounds = new Bounds(_targets[0].position, Vector3.zero);

        for (int i = 0; i < _targets.Count; i++)
        {
            bounds.Encapsulate(_targets[i].position);
            _greatestDistanceBetweenPlayers = bounds.size.x; // get greatest distance between players
        }

        _targetsCenterPos = bounds.center;
    }
    private void MoveCameraToTargetsRace()
    {
        if (_targets.Count == 0)
            return;

        if (!_hasDeepen && _mainCam.transform.position.z > _deepenDistance)
        {
            StartCoroutine(LerpCameraDepth(_timeToDeepen));
            _hasDeepen = true;
        }

        SetTargetsCenterPointAndGreatesetDistanceByVertical();

        Vector3 newPos = _targetsCenterPos + _cameraOffset;
        _mainCam.transform.position = Vector3.SmoothDamp(_mainCam.transform.position, newPos, ref _cameraVelocity, _cameraMoveDelay);
    }
    private void MoveCameraToTargetsArena()
    {
        if (_targets.Count == 0)
            return;

        SetTargetsCenterPointAndGreatesetDistanceByHorizontal();

        Vector3 newPos = _targetsCenterPos + _arenaCameraOffset;
        _mainCam.transform.position = Vector3.SmoothDamp(_mainCam.transform.position, newPos, ref _cameraVelocity, _cameraMoveDelay);
    }
    private void CalculateVelocity()
    {
        // Calculate velocity based on position changes
        Vector3 currentPosition = transform.position;
        _currentVelocity = (currentPosition - _previousPosition) / Time.deltaTime;

        // Calculate magnitude of the velocity vector
        _velocityMagnitude = _currentVelocity.magnitude;

        // Update previous position for the next frame
        _previousPosition = currentPosition;
    }
    #endregion

    #region Camera Public Calculations
    public bool ShouldScrumbleAtGroundPoint()
    {
        /* _cameraRay = _mainCam.ScreenPointToRay(UIManager.Instance.DeathLineUI.transform.position);
         RaycastHit hit;

         //Debug.DrawLine(_cameraRay.origin, _cameraRay.direction * _rayDistance, Color.red);
         if (Physics.Raycast(_cameraRay, out hit, _rayDistance, (int)_scrumbleRaycastLayer))
         {
             Debug.DrawLine(_cameraRay.origin, hit.point, Color.red);
             _deathLineAndGroundContactPoint = hit.point;

             if (_isDebugMessagesOn) Debug.Log("Should Scrumble Ground");
             return true;
         }
         Debug.DrawRay(_cameraRay.origin, _cameraRay.direction * _rayDistance, Color.red);

         if (_isDebugMessagesOn) Debug.Log("Shouldn't Scrumble Ground");
         */
        return false;
    }
    public float GetViewportWidth()
    {
        float distanceFromCamera = Vector3.Distance(transform.position, _mainCam.transform.position);
        float viewportWidth = distanceFromCamera * Mathf.Tan(_mainCam.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2f;
        return viewportWidth;
    }
    public Vector3 GetVelocity()
    {
        return _currentVelocity;
    }
    public float GetVelocityMagnitude()
    {
        return _velocityMagnitude;
    }
    #endregion

    #region Camera Private Behaviors
    private void ChangeState(GameStates currentGameState)
    {
        switch (currentGameState)
        {
            case GameStates.PreGame:
                _cameraState = NeutralState;
                break;
            case GameStates.MidGame:
                _cameraState = RaceState;
                break;
            case GameStates.LateGame:
                _cameraState = ArenaState;
                break;
            default:
                break;
        }
    }
    private void MoveCamera()
    {
        switch (GameManager.Instance.CurrentGameState)
        {
            case GameStates.LateGame:
                MoveCameraToTargetsArena();
                break;
            default:
                MoveCameraToTargetsRace();
                break;
        }
    }
    private void Zoom()
    {
        float newZoom = Mathf.Lerp(_minZoom, _maxZoom, _greatestDistanceBetweenPlayers / _maxZoomDistance);
        _cameraOffset.y = Mathf.Lerp(_cameraOffset.y, newZoom, Time.deltaTime);
    }
    private void ArenaZoom()
    {
        float newZoom = Mathf.Lerp(_arenaMinZoom, _arenaMaxZoom, _greatestDistanceBetweenPlayers / _arenaMaxZoomDistance);
        _arenaCameraOffset.y = Mathf.Lerp(_arenaCameraOffset.y, newZoom, Time.deltaTime);
    }
    #endregion

    #region Camera Public Behaviors
    public void ChangeState(int currentGameState)
    {
        switch ((GameStates)currentGameState)
        {
            case GameStates.PreGame:
                _cameraState = NeutralState;
                break;
            case GameStates.MidGame:
                _cameraState = RaceState;
                break;
            case GameStates.LateGame:
                _cameraState = ArenaState;
                break;
            default:
                break;
        }
    } // to be removed later
    public void FollowPlayer()
    {
        Transform followTransform = PlayerManager.Instance.LastPlayerAlive.transform;

        Vector3 targetPos = new(followTransform.position.x, _winZoomCameraHeight, followTransform.position.z - _zCameraOffset);
        _mainCam.transform.position = targetPos;

        Vector3 targetRot = new(_winZoomCameraTilt, followTransform.position.y, followTransform.eulerAngles.z);
        _mainCam.transform.eulerAngles = targetRot;
    }
    #endregion

    #region Camera Victory Sequence
    private IEnumerator PanAndZoomCameraToLastPlayerOverTime()
    {
        _isZommingToLastPlayer = true;
        Vector3 victoriousPlayerPos = PlayerManager.Instance.LastPlayerAlive.transform.position;
        Vector3 targetPos = new(victoriousPlayerPos.x, _maxCameraHeight, victoriousPlayerPos.z - _zCameraOffset);

        float elapsedTime = 0;
        float duration = _closeUpDuration;
        while (elapsedTime < duration)
        {
            victoriousPlayerPos = PlayerManager.Instance.LastPlayerAlive.transform.position;
            targetPos = new(victoriousPlayerPos.x, _maxCameraHeight, victoriousPlayerPos.z - _zCameraOffset);

            _mainCam.transform.position = Vector3.Lerp(_mainCam.transform.position, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _mainCam.transform.position = targetPos;


        Transform followTransform = PlayerManager.Instance.LastPlayerAlive.transform;

        Vector3 startFollowPos = _mainCam.transform.position;
        Vector3 targetFollowPos = new(followTransform.position.x, _winZoomCameraHeight, followTransform.position.z - _zCameraOffset);

        Vector3 startFollowRot = _mainCam.transform.eulerAngles;
        Vector3 targetFollowRot = new(_winZoomCameraTilt, followTransform.position.y, followTransform.eulerAngles.z);

        float newElapsedTime = 0;
        while (newElapsedTime < 0.5f)
        {
            _mainCam.transform.position = Vector3.Lerp(startFollowPos, targetFollowPos, newElapsedTime / 0.5f);
            _mainCam.transform.eulerAngles = Vector3.Lerp(startFollowRot, targetFollowRot, newElapsedTime / 0.5f);

            newElapsedTime += Time.deltaTime;
            yield return null;
        }
        _mainCam.transform.position = targetFollowPos;
        _mainCam.transform.eulerAngles = targetFollowRot;

        _isZommingToLastPlayer = false;
    }
    /*public IEnumerator ReturnToOriginalPos()
    {
        yield return new WaitForSeconds(_cameraReturnDelay);

        _isReturningToOriginLocation = true;
        Vector3 targetPos = _cameraOriginPos;

        float elapsedTime = 0;
        float duration = _returnAfterVictoryDelay;
        while (elapsedTime < duration)
        {
            _mainCam.transform.position = Vector3.Lerp(_mainCam.transform.position, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _mainCam.transform.position = targetPos;
        _isReturningToOriginLocation = false;
    }*/
    public void VictoryCameraSequenceNew()
    {
        StartCoroutine(PanAndZoomCameraToLastPlayerOverTime());
    }
    #endregion

    #region Camera Coroutines
    private IEnumerator LerpCameraDepth(float duration)
    {
        float time = 0;
        float startPosition = _cameraOffset.z;
        Vector3 newOffset = _cameraOffset;

        while (time < duration)
        {
            newOffset = _cameraOffset;
            newOffset.z = Mathf.Lerp(startPosition, _cameraTargetDepth, time / duration);
            _cameraOffset = newOffset;
            time += Time.deltaTime;
            yield return null;
        }
        _cameraOffset = newOffset;
    }
    private IEnumerator ResetCameraInDelay(float seconds) // should lerp
    {
        yield return new WaitForSeconds(seconds);
        _mainCam.transform.position = _cameraOriginPos;
    }
    public IEnumerator CameraShake(Vector3 playerVelocity, float duration, float magnitude)
    {
        //Vector3 orignalPosition = _mainCam.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float x = _mainCam.transform.position.x + playerVelocity.x / 10 * magnitude;
            float y = _mainCam.transform.position.y + playerVelocity.y / 10 * magnitude;

            _mainCam.transform.position = new Vector3(x, y, _mainCam.transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return 0;
        }
        _mainCam.transform.position = _cameraOriginPos;
    }
    public IEnumerator CameraShake(float shakeDuration, float returnDuration, float magnitude)
    {
        //Vector3 orignalPosition = _mainCam.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            float x = _mainCam.transform.position.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = _mainCam.transform.position.y + UnityEngine.Random.Range(-1f, 1f) * magnitude;

            _mainCam.transform.position = new Vector3(x, y, _mainCam.transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return 0;
        }

        float elpasedTime2 = 0f;

        while (elpasedTime2 < returnDuration)
        {
            float t = elpasedTime2 / returnDuration;
            _mainCam.transform.position = Vector3.Lerp(_mainCam.transform.position, _cameraOriginPos, t);
            elpasedTime2 += Time.deltaTime;
            yield return null;
        }
        _mainCam.transform.position = _cameraOriginPos;
    }
    public IEnumerator CameraShake(float duration, float magnitude)
    {
        //Vector3 orignalPosition = _mainCam.transform.position;
        _isScreenShaking = true;
        float elapsedTime = 0f;
        Vector3 previousCamPos = _mainCam.transform.position;

        while (elapsedTime < duration)
        {
            float x = _mainCam.transform.position.x + UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = _mainCam.transform.position.y + UnityEngine.Random.Range(-1f, 1f) * magnitude;

            _mainCam.transform.position = new Vector3(x, y, _mainCam.transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return 0;
        }
        _mainCam.transform.position = previousCamPos;
        _isScreenShaking = false;
    }
    /*public IEnumerator TransitionToArena(Transform newCameraTr)
    {
        Vector3 targetPos = newCameraTr.position;

        float elapsedTime = 0;
        float duration = _arenaTransitionDuration;

        while (elapsedTime < duration)
        {
            _mainCam.transform.position = Vector3.Lerp(_mainCam.transform.position, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _mainCam.transform.position = targetPos;
    }*/
    /*private IEnumerator WaitForFinishShakingThenGoToVictoriousPlayer()
    {
        yield return new WaitUntil(() => !_isScreenShaking);
        VictoryCameraSequenceNew();
    }*/
    #endregion

    #region Events
    private void OnGameStart()
    {
        ChangeState(GameStates.PreGame);
    }
    private void OnRoundStart()
    {
        ChangeState(GameStates.MidGame);
    }
    private void OnRoundEnd(PlayerInputHandler player)
    {
        ChangeState(GameStates.PreGame);

        //StartCoroutine(WaitForFinishShakingThenGoToVictoriousPlayer());
        //StartCoroutine(ResetCameraInDelay(_cameraResetDelayInSeconds));
    }
    private void OnRoundEndWithDraw()
    {
        ChangeState(GameStates.PreGame);
        StartCoroutine(ResetCameraInDelay(_cameraResetDelayInSeconds));
    }
    private void OnRoundSetUp()
    {
        _shouldFollowVictor = false;
    }
    #endregion

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (_targets.Count > 0)
        {
            Bounds bounds = new Bounds(_targets[0].position, Vector3.zero);

            for (int i = 0; i < _targets.Count; i++)
            {
                bounds.Encapsulate(_targets[i].position);
            }

            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }*/
}
