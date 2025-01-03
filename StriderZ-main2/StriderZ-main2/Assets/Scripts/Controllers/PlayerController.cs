using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DeathCause { Other, Deathline }

public class PlayerController : MonoBehaviour
{
    #region Components
    [Header("Components")]
    [SerializeField] private PlayerInputHandler _inputHandler;
    public PlayerInputHandler InputHandler => _inputHandler;

    [SerializeField] private Animator _animator;
    public Animator Animator { get => _animator; set => _animator = value; }

    [SerializeField] private Rigidbody _rb;
    public Rigidbody Rb => _rb;

    [SerializeField] private Transform _crosshairParent;
    public Transform CrosshairParent => _crosshairParent;

    [SerializeField] private BodyTilt _bodyTilter;
    public BodyTilt BodyTilter => _bodyTilter;

    [SerializeField] private BlendTreeController _blendTreeController;
    public BlendTreeController BlendTreeController => _blendTreeController;
    #endregion

    #region Controller Values
    [Header("Controller Values")]
    [SerializeField] private float _speed = 3200.0f, _firstGearSpeedCap = 45.0f, _secondGearSpeedCap = 70.0f;
    public float Speed => _speed;
    public float FirstGearSpeedCap => _firstGearSpeedCap;
    public float SecondGearSpeedCap => _secondGearSpeedCap;

    [SerializeField] private float _additionalSpeed = 1f;
    public float AdditionalSpeed { get => _additionalSpeed; set => _additionalSpeed = value; }

    [SerializeField] private float _floatTime = 0.5f; // in seconds
    public float FloatTime => _floatTime;

    [SerializeField] private float _grappleSpeedBoostFactor = 1.35f;
    [SerializeField] private float _deathHeight = -15.0f;
    [SerializeField] private float _currentGravityForce = 25.0f, _origialGravityForce = 25.0f, _minGravityForce = 5.0f;
    [SerializeField] private float _targetCutoffHeight = 7.0f;
    [SerializeField] private float _timeToDisintegrate = 1.0f;
    public float TimeToDisintegrate => _timeToDisintegrate;

    [SerializeField] private LayerMask _groundLayer, _dynamicLayer;
    [SerializeField] private float _groundCheckDistance = 4;
    #endregion

    #region Static Values
    private float _originalYPos = 1.5f;
    public float OriginalYPos => _originalYPos;

    private Vector3 _spawnPoint = Vector3.zero;
    public Vector3 SpawnPoint  => _spawnPoint;

    private Vector2[] _dPadBtns = new Vector2[] { new(-1.0f, 0.0f), new(0.0f, -1.0f), new(1.0f, 0.0f), new(0.0f, 1.0f) };
    private IEnumerator _gravityDelay;

    private const string _keyboardName = "Keyboard:/Keyboard";
    #endregion

    #region Dynamic Values
    private string _playerControllerName;
    private Vector3 _moveInput = Vector2.zero, _moveDirection = Vector3.zero;
    private Vector3 _lookInput = Vector2.zero, _lookRotation = Vector3.zero;
    private Vector2 _dPadInput = Vector2.zero;

    private Vector3 _playerLastObstaclePos; // for solo purposes
    public Vector3 PlayerLastObstaclePos => _playerLastObstaclePos; // for solo purposes
    #endregion

    #region GameState Booleans
    [SerializeField] private bool _isInLobby = false;
    public bool IsInLobby { get => _isInLobby; set => _isInLobby = value; }
    #endregion

    #region PlayerState Booleans
    private bool _isSoloPlayer = false;
    public bool IsSoloPlayer => _isSoloPlayer;

    private bool _isGrounded, _isDynamicGrounded;
    public bool IsGrounded => _isGrounded;
    public bool IsDynamicGrounded => _isDynamicGrounded;

    private bool _isPlayerPausing;
    public bool IsPlayerPausing { get => _isPlayerPausing; set => _isPlayerPausing = value; }

    private bool _isUsingAttractorLeft = false, _isUsingAttractorRight = false; // duplicate information with AttractorController
    public bool IsUsingPickupLeft { get => _isUsingPickupLeft; set => _isUsingPickupLeft = value; }
    public bool IsUsingPickupRight { get => _isUsingPickupRight; set => _isUsingPickupRight = value; }

    private bool _isAlive = true, _isInDanger = false, _isDead = false;
    public bool IsAlive { get => _isAlive; set => _isAlive = value; }
    public bool IsInDanger { get => _isInDanger; set => _isInDanger = value; }
    public bool IsDead { get => _isDead; set => _isDead = value; }

    private bool _isSilenced = false, _isRooted = false;
    public bool IsSilenced { get => _isSilenced; set => _isSilenced = value; }
    public bool IsRooted { get => _isRooted; set => _isRooted = value; }

    [SerializeField] private bool _isChangingModel = false, _isChangingPallet = false, _isChangingTrail = false;
    public bool IsChangingModel { get => _isChangingModel; set => _isChangingModel = value; }
    public bool IsChangingPallet { get => _isChangingPallet; set => _isChangingPallet = value; }
    public bool IsChangingTrail { get => _isChangingTrail; set => _isChangingTrail = value; }

    #endregion

    private IEnumerator _disintegrate, _recreate;
    public IEnumerator Disintegrate => _disintegrate;
    public IEnumerator Recreate => _recreate;

    private bool _isReady = false; // to change for better logic / naming
    public bool IsReady { get => _isReady; set => _isReady = value; }

    private bool _isStunned = false, _isUsingPickupLeft = false, _isUsingPickupRight = false;
    public bool IsStunned { get => _isStunned; set => _isStunned = value; }

    private IEnumerator _winSequence;
    [SerializeField] private bool _isDebugMessagesOn;

    private bool _isDoingEmote = false;

    #region Monobehaviour Callbacks
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _disintegrate = DisintegrateOverTime();
        _recreate = RecreateOverTime();
    }
    private void OnEnable()
    {
        EventManager.OnPlayerWin += OnPlayerWin;
    }
    private void Start()
    {
        InitializePlayer();
        MouseInitialization();
        //_inputHandler.Data.WinCount = ScoreManager.Instance.PlayersScore[_inputHandler.SetupData.ID]; // Initialize Score
        SetModelAndColor();
        //ChangeColor(); for lobby
        _inputHandler.SetColorsOnModel(); // finish initialization
        EventManager.InvokePlayerJoined(_inputHandler);
    }
    private void Update()
    {
        if (!_isAlive && !_isDead)
        {
            _isDead = true;
            Die();
        }
        else if (_isDead)
        {
            return;
        }

        if (!_inputHandler.IsPlayerInputsDisable)
            GetMoveDirection();
        else
            _moveDirection = Vector3.zero;

        if (_playerControllerName == _keyboardName)
        {
            AimAndRotateWithMouse();
            MouseLeftAttractorTrigger();
            MouseRightAttractorTrigger();
        }
        else
        {
            AimAndRotateWithController();
        }

        DoDPadAction();
        DoAnimations();

        DangerStateIndicator();

        _isAlive = GetIsAlive();
    }
    private void FixedUpdate()
    {
        if (!_isAlive)
            return;

        CheckGround();
        HandlePlayerMovement();
    }
    private void OnDisable()
    {
        EventManager.OnPlayerWin -= OnPlayerWin;
    }
    #endregion

    #region Get Movement & Aim Inputs + Emotes
    public void OnMove(InputAction.CallbackContext context)
    {
        if (_inputHandler.IsPlayerInputsDisable || _isStunned || _isRooted)
        {
            _moveInput = Vector2.zero;
            return;
        }
        _moveInput = context.ReadValue<Vector2>(); // read move input on moving mapped key (left stick)

        _animator.SetFloat("X", _moveDirection.x);
        _animator.SetFloat("Y", _moveDirection.z);

        if (_moveInput.magnitude > 0.1f)
            _animator.SetBool("Is_Moving", true);
        else
            _animator.SetBool("Is_Moving", false);
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        if (_isStunned) return;

        _lookInput = context.ReadValue<Vector2>(); // read look input on moving mapped key (right stick)
    }
    public void OnDPad(InputAction.CallbackContext context)
    {
        InputAction.CallbackContext? nullContext = context;
        if (_isStunned || nullContext == null) return;

        _dPadInput = context.ReadValue<Vector2>();
    }
    #endregion

    #region Player Interactions
    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        if (_isStunned || _isSilenced) return;

        bool isPressed = context.ReadValue<float>() != 0;

        if (!isPressed && context.ReadValue<float>() == 0)
        {
            _inputHandler.Attractor.CancelAttractorLeft(true);
            _isUsingAttractorLeft = false;
        }
        else if (isPressed && !_isUsingPickupLeft)
        {
            _inputHandler.Attractor.StartAttractorLeft(true);
            _isUsingAttractorLeft = true;
        }
    }
    public void OnRightTrigger(InputAction.CallbackContext context)
    {
        if (_isStunned || _isSilenced) return;

        bool isPressed = context.ReadValue<float>() != 0;

        if (!isPressed && context.ReadValue<float>() == 0)
        {
            _inputHandler.Attractor.CancelAttractorRight(true);
            _isUsingAttractorRight = false;
        }
        else if (isPressed && !_isUsingPickupRight)
        {
            _inputHandler.Attractor.StartAttractorRight(true);
            _isUsingAttractorRight = true;
        }
    }
    public void OnLeftShoulderBtn(InputAction.CallbackContext context)
    {
        bool isPressed = context.ReadValue<float>() != 0;

        if (_isInLobby && context.started && isPressed) // for lobby
        {
            return;
        }

        if (!_isInLobby && _isAlive && context.started && isPressed) // for in game
        {
            if (_isStunned || _isSilenced || _isUsingAttractorLeft) return;

            if (_inputHandler.Data.PickupItem) // verify if there is a pickable picked up by the player
            {
                _isUsingPickupLeft = true;
                UsePickup();
            }
            else // if no pickable has been picked up by the player
            {

            }
        }
    }
    public void OnRightShoulderBtn(InputAction.CallbackContext context)
    {
        bool isPressed = context.ReadValue<float>() != 0;

        if (_isInLobby && context.started && isPressed) // for lobby
        {
            return;
        }

        if (!_isInLobby && _isAlive && context.started && isPressed) // for in game
        {
            if (_isStunned || _isSilenced || _isUsingAttractorRight) return;

            if (_inputHandler.Data.PickupItem) // verify if there is a pickable picked up by the player
            {
                _isUsingPickupRight = true;
                UsePickup();
                return;
            }
            else // if no pickable has been picked up by the player
            {
                return;
            }
        }
    }
    public void OnSouthBtn(InputAction.CallbackContext context)
    {
        bool isPressed = context.ReadValue<float>() != 0;

        if (_isInLobby && context.started && isPressed) // for lobby
        {
            return;
        }

        if (!_isInLobby && _isAlive && context.started && isPressed) // for in game
        {
            if (!_isStunned) return;

            return;
        }
    }
    public void OnWestBtn(InputAction.CallbackContext context)
    {
        bool isPressed = context.ReadValue<float>() != 0;

        if (_isInLobby && context.started && isPressed) // for lobby
        {
            return;
        }

        if (!_isInLobby && _isAlive && context.started && isPressed) // for in game
        {
            if (_isStunned) return;

            return;
        }
    }
    public void OnNorthBtn(InputAction.CallbackContext context)
    {
        bool isPressed = context.ReadValue<float>() != 0;

        if (_isInLobby && context.started && isPressed) // for lobby
        {
            return;
        }

        if (!_isInLobby && _isAlive && context.started && isPressed) // for in game
        {
            if (_isStunned) return;

            return;
        }
    }
    public void OnEastBtn(InputAction.CallbackContext context)
    {
        bool isPressed = context.ReadValue<float>() != 0;
        bool isGameMode3 = GameManager.Instance.CurrentGameMode == GameModeType.IndiciumGarden;

        if (_isInLobby && context.started && isPressed) // for lobby
        {
            return;
        }

        if (!_isInLobby && _isAlive && context.started && isPressed) // for in game
        {
            if (_isStunned) return;

            return;
        }

        if (isGameMode3 && !_isInLobby && context.started && isPressed)
        {
            UseWeapon();
            return;
        }
    }
    public void OnStartBtn(InputAction.CallbackContext context)
    {
        bool isPressed = context.ReadValue<float>() != 0;
        int titleScreenBuildIndex = 0;
        int currentSceneBuildIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

        if (currentSceneBuildIndex > titleScreenBuildIndex && _isAlive && context.started && isPressed) // for in game
        {
            PauseGame();
            return;
        }
    }
    #endregion

    #region Initializations
    private void InitializePlayer()
    {
        _inputHandler.Data.NickNameTMPro.text = _inputHandler.Data.Nickname;
        _playerControllerName = _inputHandler.SetupData.Input.devices[0].ToString();
        _gravityDelay = GravityDelay();
    }
    private void MouseInitialization()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true; // to be false
    }
    public void SetModelAndColor()
    {
        for (int i = 0; i < _inputHandler.Data.Models.Length; i++)
        {
            _inputHandler.Data.Models[i].SetActive(false);

            if (i == (int)_inputHandler.SetupData.ChosenModelType)
            {
                GameObject activeModel = _inputHandler.Data.Models[i].gameObject;
                activeModel.SetActive(true);
                _inputHandler.Data.ModelData = activeModel.GetComponent<ModelData>();
                _inputHandler.SetColorsOnModel();
                _animator = _inputHandler.Data.ModelData.Animator;
                _inputHandler.Attractor.LeftHandTr = _inputHandler.Data.ModelData.AttractorHands[0];
                _inputHandler.Attractor.RightHandTr = _inputHandler.Data.ModelData.AttractorHands[1];
                _inputHandler.Attractor.ReInitializeAll();
            }
        }
    }
    public void InitializeOnJoin()
    {
        InitializePlayer();
        MouseInitialization();
        //_inputHandler.Data.WinCount = ScoreManager.Instance.PlayersScore[_inputHandler.SetupData.ID]; // Initialize Score
        SetModelAndColor();
        //ChangeColor(); for lobby

        if (PlayerSetupManager.Instance.AllPlayersSetupData.Count == 1)
            _isSoloPlayer = true;

        _inputHandler.SetColorsOnModel(); // finish initialization
    }
    public void SetSoloPlayer(bool isSolo)
    {
        _isSoloPlayer = isSolo;
    }
    #endregion

    #region Controller Handling
    public void ResetInputs()
    {
        _moveInput = Vector2.zero;
        _lookInput = Vector2.zero;
        _dPadInput = Vector2.zero;
    }
    public void SetNewSpawnPoint(Transform playerTr)
    {
        _spawnPoint = playerTr.position;
        transform.SetPositionAndRotation(playerTr.position, playerTr.rotation);
    }
    #endregion

    #region Runtime Input Calculations
    private void GetMoveDirection()
    {
        _moveDirection = new(_moveInput.x, 0f, _moveInput.y);
    }
    private void AimAndRotateWithController()
    {
        _lookRotation = _lookInput.normalized;

        if (_lookRotation.magnitude != 0)
        {
            float angles = Mathf.Atan2(_lookRotation.x, _lookRotation.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angles, 0);

            _crosshairParent.transform.localRotation = Quaternion.Slerp(_crosshairParent.transform.localRotation, targetRotation, 15.0f * Time.deltaTime);
            /*_inputHandler.GrappleController.AimPointTr.transform.localRotation = Quaternion.Euler(0, angles, 0);*/
            _inputHandler.Data.ModelData.BodyMesh.transform.parent.rotation = Quaternion.Euler(0, angles, 0);
        }
    }
    private void AimAndRotateWithMouse()
    {
        if (_inputHandler.IsPlayerInputsDisable)
            return;

        Vector3 mousePosition = Input.mousePosition;
        //ClampMouseToPlayer(mousePosition); // clamp mouse movement

        // Raycast from the camera to the mouse position
        Ray ray = CinemachineManager.Instance.MainCam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // check if it hits something
        {
            // 4.5 is fixed y height - should be changed later
            Vector3 direction = new Vector3(hit.point.x, 2.5f, hit.point.z) - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            _crosshairParent.transform.localRotation = Quaternion.Slerp(_crosshairParent.transform.localRotation,
            targetRotation, 15.0f * Time.deltaTime);

            _inputHandler.Data.ModelData.BodyMesh.transform.parent.rotation = targetRotation;
        }

    }
    private void MouseLeftAttractorTrigger()
    {
        if (_inputHandler.IsPlayerInputsDisable)
            return;

        if (_isUsingAttractorLeft && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Debug.Log("Grapple left canceled.");
            _inputHandler.Attractor.CancelAttractorLeft(true);
            _isUsingAttractorLeft = false;
        }
        else if (Mouse.current.leftButton.wasPressedThisFrame && !_isUsingAttractorLeft)
        {
            _isUsingAttractorLeft = true;
            Debug.Log("Grapple left Started.");
            _inputHandler.Attractor.StartAttractorLeft(true);
            //StartCoroutine(UseLeftGrappleWithAnimation());
        }
        //else Debug.LogError("Grapple left failed: No input started, preformed or canceled.");
    }
    private void MouseRightAttractorTrigger()
    {
        if (_inputHandler.IsPlayerInputsDisable)
            return;

        if (_isUsingAttractorRight && Mouse.current.rightButton.wasReleasedThisFrame)
        {
            Debug.Log("Grapple right canceled.");
            /*// aaab */
            _inputHandler.Attractor.CancelAttractorRight(true);
            _isUsingAttractorRight = false;
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame && !_isUsingAttractorRight)
        {
            _isUsingAttractorRight = true;
            Debug.Log("Grapple right started.");
            /*// aaab */
            _inputHandler.Attractor.StartAttractorRight(true);
            //StartCoroutine(UseRightGrappleWithAnimation());
        }
        //else Debug.LogError("Grapple right failed: No input started, preformed or canceled.");
    }
    private void DoDPadAction()
    {
        if (_isDoingEmote)
            return;

        if (_playerControllerName == _keyboardName)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                //Debug.Log($"left arrow" + _inputHandler.Data.ID);
            }
            else if (Keyboard.current.digit2Key.wasPressedThisFrame)
            {
                _animator.SetTrigger("Emote 01");
                //Debug.Log($"down arrow" + _inputHandler.Data.ID);
            }
            else if (Keyboard.current.digit3Key.wasPressedThisFrame)
            {
                _animator.SetTrigger("Emote 02");
                //Debug.Log($"right arrow" + _inputHandler.Data.ID);
            }
            else if (_dPadInput == _dPadBtns[3] && Keyboard.current.digit4Key.wasPressedThisFrame)
            {
                _animator.SetTrigger("Emote 03");
                //Debug.Log($"up arrow" + _inputHandler.Data.ID);
            }
        }
        else
        {
            if (_dPadInput == _dPadBtns[0]/* && Gamepad.current.dpad.left.wasPressedThisFrame*/)
            {
                //Debug.Log($"left arrow " + _inputHandler.Data.ID);
            }
            else if (_dPadInput == _dPadBtns[1]/* && Gamepad.current.dpad.down.wasPressedThisFrame*/)
            {
                _animator.SetTrigger("Emote 01");
                _animator.ResetTrigger("Emote 02");
                _animator.ResetTrigger("Emote 03");
                _isDoingEmote = true;

                StartCoroutine(ResetEmoteBool());
                //Debug.Log($"down arrow" + _inputHandler.Data.ID);
            }
            else if (_dPadInput == _dPadBtns[2]/* && Gamepad.current.dpad.right.wasPressedThisFrame*/)
            {
                _animator.SetTrigger("Emote 02");
                _animator.ResetTrigger("Emote 01");
                _animator.ResetTrigger("Emote 03");
                _isDoingEmote = true;

                StartCoroutine(ResetEmoteBool());
                //Debug.Log($"right arrow" + _inputHandler.Data.ID);
            }
            else if (_dPadInput == _dPadBtns[3]/* && Gamepad.current.dpad.up.wasPressedThisFrame*/)
            {
                _animator.SetTrigger("Emote 03");
                _animator.ResetTrigger("Emote 02");
                _animator.ResetTrigger("Emote 01");
                _isDoingEmote = true;

                StartCoroutine(ResetEmoteBool());
                //Debug.Log($"up arrow" + _inputHandler.Data.ID);
            }
        }
    }
    private void UsePickup()
    {
        if (!_inputHandler.Data.PickupItem)
            return;

        _inputHandler.Data.PickupItem.Use();

        if (_inputHandler.Data.PickupItem.IsUsed)
            _inputHandler.Data.PickupItem = null;
    }
    private void UseWeapon()
    {
        if (!_inputHandler.Data.CurrentWeapon)
            return;

        _inputHandler.Data.CurrentWeapon.UseWeapon();
    }
    private void PauseGame()
    {
        EventManager.InvokePlayerPause(_inputHandler);
    }
    #endregion

    #region Runtime Calculations
    private IEnumerator ResetEmoteBool()
    {
        yield return new WaitForSeconds(1);
        _isDoingEmote = false;
    }
    private void DoAnimations()
    {
        _blendTreeController.BlendAnimations(_inputHandler, _moveInput.x, _moveInput.y);
        _bodyTilter.TiltBody(_inputHandler, _moveInput.x, _moveInput.y);
    }
    private void Die()
    {
        GameObject effect02 = Instantiate(_inputHandler.Data.PlayerElimVFX, transform.position, Quaternion.identity, null);
        SoundManager.Instance.PlayPlayerSound(SoundManager.Instance.PlayerDeath, true, 2.462f);
        Destroy(effect02, 2);

        //maybe should add isDied = true
        if (!_isSoloPlayer) // one player fix
        {
            _isReady = false;
            _isStunned = false;

            _rb.velocity = Vector3.zero;
        }

        //StartCoroutine(CameraManager.Instance.CameraShake(0.45f, 1.0f)); replace camera
        _inputHandler.PlayerWorldUI.PlayerUI.SetActive(false);
        _inputHandler.Attractor.CancelAttractorLeft(true);
        _inputHandler.Attractor.CancelAttractorRight(true);
        _inputHandler.Attractor.ReturnLeftAttractor();
        _inputHandler.Attractor.ReturnRightAttractor();
        _inputHandler.Attractor.DisableAttractor(true);
        _inputHandler.IsPlayerInputsDisable = true;

        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        foreach (PlayerInputHandler player in allPlayersAlive)
        {
            if (player.Attractor.LeftGrabbedRb == _rb)
                player.Attractor.CancelAttractorLeft(true);

            if (player.Attractor.RightGrabbedRb == _rb)
                player.Attractor.CancelAttractorRight(true);
        }

        EventManager.InvokePlayerDeath(_inputHandler);

        if (_isDebugMessagesOn) Debug.Log($"player died {_inputHandler.SetupData.ID}");
    }
    public void Revive()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        GameObject respawnVfx = Instantiate(_inputHandler.Data.RespawnVfx, transform.position, Quaternion.identity, transform);
        // need to destroy respawnVfx after some done

        CinemachineManager.Instance.TargetGroup.AddMember(_inputHandler.Data.TrackCamTr, CinemachineManager.Instance.TargetWeight, CinemachineManager.Instance.TargetRadius);

        _isAlive = true;
        _isDead = false;
        _isStunned = false;
        _isRooted = false;
        _inputHandler.PlayerWorldUI.PlayerUI.SetActive(true);
        _inputHandler.Attractor.DisableAttractor(false);

        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        allPlayersAlive.Add(_inputHandler);
        allPlayersAlive.Sort(PlayerManager.Instance.CompareByID);
    }
    public float CalculatePath()
    {
        if (!LaserRushGameMode.Instance.EndPoint)
            return 0;

        Vector3 targetPos = LaserRushGameMode.Instance.EndPoint.position;

        float distanceToFinish = Vector3.Distance(transform.position, targetPos);
        return distanceToFinish;

/* Calculate real path - not 
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, path))
        {
            float pathDistance = 0f;

            for (int i = 1; i < path.corners.Length; i++)
                pathDistance += Vector3.Distance(path.corners[i - 1], path.corners[i]);

            // Debug.Log("Path distance: " + pathDistance);
            return pathDistance;
        }
        else
        {
            // Debug.Log("Path calculation failed.");
            return 0;
        }*/
    }
    #endregion

    #region RunTime Physics Calculation
    private void CheckGround()
    {
        RaycastHit hit;
        Vector3 checkPos = transform.position;
        checkPos.y += 2;
        if (Physics.Raycast(checkPos, Vector3.down, out hit, _groundCheckDistance, _groundLayer))
        {
            GameObject objectHit = hit.transform.gameObject;
            int gameObjectLayer = objectHit.layer;

            if ((_dynamicLayer.value & (1 << gameObjectLayer)) > 0)
            {
                Vector3 newPos = hit.point;
                newPos.y = hit.point.y + hit.transform.lossyScale.y / 2 + 2.0f;
                transform.position = newPos;

                _isDynamicGrounded = true;
            }
            else
            {
                _isDynamicGrounded = false;
            }

            if (_isSoloPlayer)
                _playerLastObstaclePos = hit.transform.position;

            _isGrounded = true;
            StopCoroutine(_gravityDelay);
            _gravityDelay = null;
            _gravityDelay = GravityDelay();

            _rb.useGravity = false;

            if (_inputHandler.Data.FallHoverVfx.activeInHierarchy)
                _inputHandler.Data.FallHoverVfx.SetActive(false);

            if (GameManager.Instance.CurrentGameMode != GameModeType.Lobby && transform.position.y != _originalYPos)
                transform.position = new Vector3(transform.position.x, _originalYPos, transform.position.z);

            // freeze unwanted movement & rotation on following axises
            if (transform.position.y >= hit.transform.position.y)
            {
                _rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            }
            else
            {
                _rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                //_rb.AddForce(_flaotUpForce * Time.fixedDeltaTime * transform.up, ForceMode.Force);
            }

            if (_isDebugMessagesOn) Debug.Log($"{name} is grounded");
        }
        else
        {
            if (GameManager.Instance.CurrentGameState != GameStates.Preperations && !_inputHandler.Data.FallHoverVfx.activeInHierarchy && !_isInDanger) // also checks if sarted falling
            {
                _inputHandler.Data.FallHoverVfx.SetActive(true);
                StartCoroutine(PlayDangerIndicatorAfterFallHover());
                _currentGravityForce = _origialGravityForce;
                _rb.AddForce(_currentGravityForce * Vector3.up, ForceMode.Impulse);
            }

            //_rb.useGravity = true;
            if (_isGrounded)
            {
                StartCoroutine(_gravityDelay);
                _isGrounded = false;
            }
            _rb.AddForce(_currentGravityForce * Vector3.down, ForceMode.Acceleration);
        }
    }
    private void HandlePlayerMovement()
    {
        if (_rb.velocity.magnitude >= _firstGearSpeedCap)
            _rb.AddForce(_moveDirection * Time.fixedDeltaTime * _speed * _additionalSpeed * _grappleSpeedBoostFactor, ForceMode.Force);
        else
            _rb.AddForce(_moveDirection * Time.fixedDeltaTime * _speed * _additionalSpeed, ForceMode.Force);

        if (_rb.velocity.magnitude > _secondGearSpeedCap)
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, _secondGearSpeedCap);
    }
    #endregion

    #region Danger State
    private void DangerStateIndicator()
    {
        if (_inputHandler.Data.DangerCounter > 0 && _inputHandler.Data.DangerTime > 0)
        {
            _inputHandler.Data.DangerTime -= Time.deltaTime;

            if (!_inputHandler.Data.DangerVfx.activeInHierarchy)
                _inputHandler.Data.DangerVfx.SetActive(true);
        }
        else if (_inputHandler.Data.DangerCounter > 0 && _inputHandler.Data.DangerTime <= 0)
        {
            _inputHandler.Data.DangerCounter = 0;
            _inputHandler.Data.DangerVfx.SetActive(false);
            _isInDanger = false;
            //_inputHandler.PlayerWorldUI.ChangeEnergyColor(_inputHandler.SetupData.ColorData.EmissionColor);
        }
    }
    public void SetDangerTime()
    {
        _isInDanger = true;
        _inputHandler.Data.DangerCounter++;
        _inputHandler.Data.DangerTime = _inputHandler.Data.GlobalDangerTime;
    }
    public void SetDangerTime(float customDangerTime)
    {
        _isInDanger = true;
        _inputHandler.Data.DangerTime = customDangerTime;
    }
    #endregion

    #region Disintegrate
    private IEnumerator DisintegrateOverTime()
    {
        float timeElapsed = 0;
        float cutoffHeight = _inputHandler.AllMaterialInstances[0].GetFloat("_Cutoff_Height");
        float startCutOff = cutoffHeight;
        float targetCutoffHeight = -1.0f;
        while (timeElapsed < _timeToDisintegrate)
        {
            cutoffHeight = Mathf.Lerp(startCutOff, targetCutoffHeight, timeElapsed / _timeToDisintegrate);
            timeElapsed += Time.deltaTime;

            for (int i = 0; i < _inputHandler.AllMaterialInstances.Count; i++)
            {
                Material mat = _inputHandler.AllMaterialInstances[i];
                mat.SetFloat("_Cutoff_Height", cutoffHeight);
            }
            
            yield return null;
        }
        cutoffHeight = targetCutoffHeight;
        for (int i = 0; i < _inputHandler.AllMaterialInstances.Count; i++)
        {
            Material mat = _inputHandler.AllMaterialInstances[i];
            mat.SetFloat("_Cutoff_Height", cutoffHeight);
        }

        _disintegrate = null;
        _disintegrate = DisintegrateOverTime();
    }
    private IEnumerator RecreateOverTime()
    {
        float timeElapsed = 0;
        float cutoffHeight = _inputHandler.AllMaterialInstances[0].GetFloat("_Cutoff_Height");
        float startCutOff = cutoffHeight;
        float targetCutoffHeight = _targetCutoffHeight;
        while (timeElapsed < _timeToDisintegrate)
        {
            cutoffHeight = Mathf.Lerp(startCutOff, targetCutoffHeight, timeElapsed / _timeToDisintegrate);
            timeElapsed += Time.deltaTime;

            for (int i = 0; i < _inputHandler.AllMaterialInstances.Count; i++)
            {
                Material mat = _inputHandler.AllMaterialInstances[i];
                mat.SetFloat("_Cutoff_Height", cutoffHeight);
            }

            yield return null;
        }
        cutoffHeight = targetCutoffHeight;
        for (int i = 0; i < _inputHandler.AllMaterialInstances.Count; i++)
        {
            Material mat = _inputHandler.AllMaterialInstances[i];
            mat.SetFloat("_Cutoff_Height", cutoffHeight);
        }

        _recreate = null;
        _recreate = RecreateOverTime();
    }
    #endregion

    #region Getters
    private bool GetIsAlive()
    {
        if (transform.position.y > _deathHeight)
            return true;
        else
            return false;
    }
    #endregion

    #region Coroutines
    private IEnumerator GravityDelay()
    {
        _currentGravityForce = _minGravityForce;
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        if (_isDebugMessagesOn) Debug.Log($"{name} is not grounded");
        yield return new WaitForSeconds(_floatTime);

        _currentGravityForce = _origialGravityForce;
        _rb.useGravity = true;
        if (_isDebugMessagesOn) Debug.Log($"{name} is grounded");
        // free y position constraints

    }
    private IEnumerator PlayDangerIndicatorAfterFallHover()
    {
        if (_isInDanger)
            yield break;

        yield return new WaitUntil(() => !_inputHandler.Data.FallHoverVfx.activeInHierarchy);

        SetDangerTime();
    }
    private IEnumerator WinSequence()
    {
        /* replace camera
         * yield return new WaitUntil(() => !CameraManager.Instance.IsScreenShaking);
         */

        
        UIManager.Instance.WinAlert.SetActive(true);
        yield return new WaitForSeconds(1);
        UIManager.Instance.WinAlert.SetActive(false);
        //_inputHandler.IsPlayerInputsDisable = true;
        EventManager.InvokeGameModeConcluded();
    }
    #endregion

    private void OnPlayerWin(PlayerInputHandler player)
    {
        if (player != _inputHandler)
            return;

        _winSequence = WinSequence();
        StartCoroutine(_winSequence);
    }
    #region Currently Unused
    [SerializeField] private Vector2 _clampedMouseAreaSize = new(5.0f, 5.0f);

    public IEnumerator UseLeftGrappleWithAnimation()
    {
        yield return new WaitForSeconds(0.25f); // do animation then shoot grapple

        /*aaab*/

        /*if (_isGrapplingLeft)
            _inputHandler.GrappleController.StartGrappleLeft();
        else
            yield break;*/

    }
    public IEnumerator UseRightGrappleWithAnimation()
    {
        yield return new WaitForSeconds(0.25f); // do animation then shoot grapple

        /*aaab*//*

        if (_isGrapplingRight)
            _inputHandler.GrappleController.StartGrappleRight();
        else
            yield break;*/

    }
    public void CustomDeathEffect(DeathCause cause)
    {
        GameObject deathVFX = null;
        AudioClip deathSound = null;
        switch (cause)
        {
            case DeathCause.Other:
                deathVFX = Instantiate(_inputHandler.Data.PlayerElimVFX, transform.position, Quaternion.identity, null);
                deathSound = SoundManager.Instance.PlayerDeath;
                break;
            case DeathCause.Deathline:
                deathVFX = Instantiate(_inputHandler.Data.DeathlineDeathVFXGO, transform.position, Quaternion.identity, null);
                deathSound = SoundManager.Instance.DeathlineDeath;
                break;
            default:
                deathVFX = Instantiate(_inputHandler.Data.PlayerElimVFX, transform.position, Quaternion.identity, null);
                deathSound = SoundManager.Instance.PlayerDeath;
                break;
        }

        if (deathSound != null)
            SoundManager.Instance.PlayPlayerSound(deathSound, true, 2.462f);

        if (deathVFX != null)
            Destroy(deathVFX, 2);
    }
    private void ClampMouseToPlayer(Vector3 mousePosition)
    {
        // Convert the player's world position to screen coordinates
        Vector3 playerScreenPos = CinemachineManager.Instance.MainCam.WorldToScreenPoint(transform.position);

        // Define the clamping area in screen coordinates
        Rect clampedArea = new Rect(
            playerScreenPos.x - _clampedMouseAreaSize.x / 2f,
            playerScreenPos.y - _clampedMouseAreaSize.y / 2f,
            _clampedMouseAreaSize.x,
            _clampedMouseAreaSize.y
        );

        // Get the mouse position in screen coordinates
        Vector3 mouseScreenPos = mousePosition;

        // Clamp the mouse position to the defined area
        float clampedX = Mathf.Clamp(mouseScreenPos.x, clampedArea.x, clampedArea.x + clampedArea.width);
        float clampedY = Mathf.Clamp(mouseScreenPos.y, clampedArea.y, clampedArea.y + clampedArea.height);

        Cursor.SetCursor(null, new Vector2(clampedX, clampedY), CursorMode.Auto);
    } // currently unused

    /*private void GrappleInteractions()
    {
        if (_isUsingAttractorLeft || _isUsingAttractorRight)
            _animator.SetTrigger("Grapple");

        aaab

        if (_isGrapplingLeft)
            _inputHandler.GrappleController.PerformGrappleLeft();

        if (_isGrapplingRight)
            _inputHandler.GrappleController.PerformGrappleRight();

    }*/
    #endregion
}
