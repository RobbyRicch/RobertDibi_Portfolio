using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using TMPro;

public enum GunType
{
    Primary,
    SideArm
}
public enum FaceDirection // for sorting layer
{
    Left = 0,
    Right = 2
}
public enum InteractionType
{
    Barrier,
    Breach
}

public class Player_Controller : MonoBehaviour, IProfileSaveable // add all data
{
    #region MVC
    [Header("MVC Components")]
    [SerializeField] private Player_Animations _animations; // MVC View
    public Player_Animations Animations => _animations;

    [SerializeField] private Player_Data _data; // MVC Model
    public Player_Data Data => _data;

    [SerializeField] private Player_Deflect _deflect;
    public Player_Deflect Deflect => _deflect;

    [SerializeField] private Player_Focus _focus;
    public Player_Focus Focus => _focus;

    [SerializeField] private LinkIntegritySystem _lis;
    public LinkIntegritySystem LIS => _lis;

    [SerializeField] private Player_HUD _hud; // MVC View
    public Player_HUD HUD => _hud;

    [SerializeField] private DeathMenuManager _deathMenu; // MVC View
    public DeathMenuManager DeathMenu => _deathMenu;

    [SerializeField] private UIFader _fader; 
    public UIFader Fader => _fader;
    #endregion

    #region Unique Abilities
    [Header("Unique Abilities")]
    [SerializeField] private MeleeBase _melee;
    public MeleeBase Melee => _melee;

    [SerializeField] private UltimateBase _ultimate;
    public UltimateBase Ultimate => _ultimate;

    [SerializeField] private DashBase _dash;
    #endregion

    #region Components
    [Header("Player Character Components")]
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private SpriteRenderer _armSr;
    [SerializeField] private Collider2D _mainCollider;
    [SerializeField] private AudioSource _characterAudioSource;
    [SerializeField] private AudioClip _characterTakeDamageAC;
    [SerializeField][Range(1.0f, 3.0f)] private float _speed = 1.2f;
    [SerializeField] public TextMeshProUGUI _worldSpaceDialogueTMP;
    //[SerializeField] private bool _isAbilityHold = false;
    private Vector2 _lastMoveDirection = Vector2.zero;
    private PlayerControls _playerControls;
    private Transform _playerCamTargetOG;
    private Camera _currentCam = null;
    private Mouse _currentMouse = null;
    private bool _isArmFacingRight = true;
    private bool _canBeKnockedBack = true;
    private bool _isKnockedBack = false;
    private bool _shouldStopUpdates = false;
    private bool _isAlive = true;
    public bool IsAlive => _isAlive;    

    //bool check for tutorial
    public bool IsFirstTimeInTraining = true;

    [SerializeField] private CinemachineVirtualCamera _currentVirtualCamera;
    public CinemachineVirtualCamera CurrentVirtualCamera => _currentVirtualCamera;

    [SerializeField] private GameObject _interactionKey;
    public GameObject InteractionKey => _interactionKey;

    private Vector2 _currentMoveVector = Vector2.zero;
    public Vector2 CurrentMoveVector => _currentMoveVector;

    private bool _isInputDisabled = false;
    public bool IsInputDisabled { get => _isInputDisabled; set => _isInputDisabled = value; }

    private bool _isMovementOnlyDisabled = false;
    public bool IsMovementOnlyDisabled { get => _isMovementOnlyDisabled; set => _isMovementOnlyDisabled = value; }

    private bool _isExternalControl = false;
    public bool IsExternalControl { get => _isExternalControl; set => _isExternalControl = value; }

    private bool _isPickuping = false;
    public bool IsPickingUp { get => _isPickuping; set => _isPickuping = value; }

    [Header("Character VFX")]
    [SerializeField] private float _pickupDelay = 0.25f;

    [SerializeField] private Volume _damageVolume;
    [SerializeField] private GameObject _damageVFX;

    [SerializeField] private SpriteRenderer[] _spritesToFade;

    

    

    [SerializeField] private float _deathSceneResetDelay = 1.0f;

    #region Currency
    [Header("Currency")]
    [SerializeField] private int _currencyToAdd = 0;
    [SerializeField] private int _totalCurrencyAdded = 0;
    [SerializeField] private float _subCurrencyFadeDuration = 0.3f;
    [SerializeField] private float _subCurrencyIncrementSpeed = 50.0f;
    [SerializeField] private float _delayAfterGain = 0.5f;
    [SerializeField] private float _iconGrowDuration = 0.3f;
    [SerializeField] private Vector3 _targetGrowSize = Vector3.one;
    [SerializeField] private float _iconShrinkDuration = 0.3f;
    [SerializeField] private Vector3 _targetShrinkSize = new (0.8f, 0.8f, 0.8f);
    [SerializeField] private float _subCurrencySparkleDuration = 0.3f;
    private Coroutine _updateCurrencyRoutine = null;
    private Coroutine _gainCurrencyRoutine = null;
    private Coroutine _payCurrencyRoutine = null;
    #endregion

    #region Transforms
    [Header("Transform")]
    [SerializeField] private Transform _crosshairTr;
    public Transform CrosshairTransform => _crosshairTr;

    [SerializeField] private Transform _primaryOnHand, _primaryOffHand;
    [SerializeField] private Transform _sideArmOnHand, _sideArmOffHand;

    [SerializeField] private Transform _armPivot;
    public Transform ArmPivot => _armPivot;
    #endregion

    [Header("DamageFade - New")]
    [SerializeField] private RawImage _damageImage;
    [SerializeField] private float _fadeSpeed = 0.1f;
    [SerializeField] private float _damageIncreaseAmount = 0.2f;
    [SerializeField] private float _damageVFXTime = 1.0f;
    private Coroutine _fadeCoroutine;

    #region Guns Properties
    [Header("Guns (read only)")]
    [SerializeField] private GunBase _currentlyEquippedGun;
    public GunBase CurrentlyEquippedGun { get => _currentlyEquippedGun; set => _currentlyEquippedGun = value; }

    [SerializeField] private GunPrimary _primaryGun;
    public GunPrimary PrimaryGun { get => _primaryGun; set => _primaryGun = value; }

    [SerializeField] private GunSideArm _sideArm;
    public GunSideArm SideArm { get => _sideArm; set => _sideArm = value; }
    #endregion

    [Header("Breach")]
    private BreachRoomManager _currentBreachRoom;
    public BreachRoomManager CurrentBreachRoom { get => _currentBreachRoom; set => _currentBreachRoom = value; }

    private bool _isInBreach = false;
    public bool IsInBreach { get => _isInBreach; set => _isInBreach = value; }
    #endregion

    #region OtherComponents
    [Header("Other Components")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] public GameObject _hazyVision;
    #endregion

    #region Stats
    public int CurrentKills = 0;
    public int KillsToUpgrade = 25;

    [Header("Constant Stats")]
    [SerializeField] private float _knockBackTime = 0.15f;

    [Header("Dynamic Stats")]
    [SerializeField] private float _cursorSensetivity = 40.0f;
    private float _timeSinceUsedStamina = 0;
    private float _cursorSmoothSpeed = 40.0f;
    private float _currentMoveSpeed = 230.0f;

    [Header("Real-time Stats")]
    [SerializeField] public float _currentHealth = 0.0f;

    [SerializeField] private float _currentStamina = 0.0f;
    [SerializeField] private float _staminaRegenSpeed = 0.0f;
    public float CurrentStamina { get => _currentStamina; set => _currentStamina = value; }

    private bool _isFreezingTime = false;
    #endregion

    #region Input members
    private InputAction _moveAction, _lookAction, _fireAction, _meleeAction, _deflectAction, _focusAction, _ultimateAction, _swapWeaponAction, _reloadAction, _interactAction, _abilityAction, _pauseAction;
    private InputAction _healthCheatAction, _staminaCheatAction, _ultCheatAction, _focusCheatAction, _staticHealthCheatAction;
    #endregion

    #region Cheats
    private float _tempHealthForCheats;

    // cheat flags
    private bool _isTrainingCheatsOn = false, _isHealthCheatOn = false, _isStaminaCheatOn = false, _isUltCheatOn = false, _isFocusCheatOn = false, _isStaticHealthCheatOn = false;
    #endregion

    private bool _isInCutscene;
    public bool IsInCutscene => _isInCutscene;

    private int _restoreFromBreachCounter = 0; // temp solution

    #region Monobehaviour Callbacks
    private void Awake()
    {
        _playerControls = new PlayerControls();
        InitializeInputs();
    }
    private void OnEnable()
    {
        EnableInputs();

        EventManager.OnMouseSensetivityChange += OnMouseSensetivityChange;
        EventManager.OnUpdateCurrency += OnUpdateCurrency;
        EventManager.OnGainCurrency += OnGainCurrency;
        EventManager.OnPayCurrency += OnPayCurrency;
        EventManager.OnPlayerHit += OnPlayerHit;
        EventManager.OnTimeFreeze += OnTimeFreeze;
        EventManager.OnPickupConsumable += OnPickupConsumable;
        EventManager.OnCutscene += OnCutscene;
        EventManager.OnEndGame += OnEndGame;
        EventManager.OnSkillAquired += OnSkillAquired;
        EventManager.OnGunUpgrade += OnGunUpgrade;

        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void Start()
    {
        //SceneManager.sceneLoaded += OnSceneLoaded;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        _currentCam = Camera.main;
        _currentMouse = Mouse.current;

        GetScreenBounds();

        _playerCamTargetOG = transform;

        InitializePlayer();
        ApplyConstantHudChanges();
        OnMouseSensetivityChange(this);

    }
    private void Update()
    {
        if (!_isAlive)
            return;

        _animations.AnimateMovement(_currentMoveVector * _speed, _lastMoveDirection, _mainCollider); // requires Robby
        StaticHealthCheat();
        CheckUpdateCheats();

        if (_shouldStopUpdates)
            return;

        if (!_isInputDisabled)
            OnLook();



        if (_fireAction.IsPressed())
            OnFire();
        if (_fireAction.WasReleasedThisFrame())
            OnRelease();

        //if (!_isAbilityHold && _abilityAction.WasPressedThisFrame())
        //    OnAbility();

        ApplyConstantHudChanges();

        if (_damageImage && _damageImage.color.a > 0)
        {
            // Gradually decrease the alpha value
            Color color = _damageImage.color;
            color.a -= _fadeSpeed * Time.unscaledDeltaTime; // Use unscaledDeltaTime for consistent behavior during time scaling
            color.a = Mathf.Clamp01(color.a); // Ensure alpha stays within 0-1 range
            _damageImage.color = color;
        }

        if (_currentStamina < _data.MaxStamina)
        {
            _currentStamina += _staminaRegenSpeed * Time.unscaledDeltaTime; // Gradually regenerate stamina
            _currentStamina = Mathf.Clamp(_currentStamina, 0, _data.MaxStamina); // Ensure stamina stays within 0 and MaxStamina range
        }
    }
    private void FixedUpdate()
    {
        if (_shouldStopUpdates)
            return;

        OnMove();

        //if (_isAbilityHold && _abilityAction.IsPressed())
        //    OnAbility();
    }
    private void OnDisable()
    {
        DisableInputs();
        EventManager.OnMouseSensetivityChange -= OnMouseSensetivityChange;
        EventManager.OnUpdateCurrency -= OnUpdateCurrency;
        EventManager.OnGainCurrency -= OnGainCurrency;
        EventManager.OnPayCurrency -= OnPayCurrency;
        EventManager.OnPlayerHit -= OnPlayerHit;
        EventManager.OnTimeFreeze -= OnTimeFreeze;
        EventManager.OnPickupConsumable -= OnPickupConsumable;
        EventManager.OnCutscene -= OnCutscene;
        EventManager.OnEndGame -= OnEndGame;
        EventManager.OnSkillAquired -= OnSkillAquired;
        EventManager.OnGunUpgrade -= OnGunUpgrade;

        //SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    #endregion

    #region Input Handling
    private void InitializeInputs()
    {
        _moveAction = _playerControls.Player.Move;
        _lookAction = _playerControls.Player.Look;
        _fireAction = _playerControls.Player.Fire;
        _meleeAction = _playerControls.Player.Melee;
        _deflectAction = _playerControls.Player.Deflect;
        _swapWeaponAction = _playerControls.Player.SwapWeapon;
        _reloadAction = _playerControls.Player.Reload;
        _focusAction = _playerControls.Player.Focus;
        _interactAction = _playerControls.Player.Interaction;
        _abilityAction = _playerControls.Player.Ability;
        _ultimateAction = _playerControls.Player.Ultimate;
        _pauseAction = _playerControls.Player.Pause;

        // cheats
        _healthCheatAction = _playerControls.Player.HealthCheat;
        _staminaCheatAction = _playerControls.Player.StaminaCheat;
        _ultCheatAction = _playerControls.Player.UltCheat;
        _focusCheatAction = _playerControls.Player.FocusCheat;
        _staticHealthCheatAction = _playerControls.Player.StaticHealthCheat;

    }
    private void EnableInputs()
    {
        _moveAction.Enable();
        _lookAction.Enable();
        _fireAction.Enable();
        _meleeAction.Enable();
        _deflectAction.Enable();
        _focusAction.Enable();
        _swapWeaponAction.Enable();
        _reloadAction.Enable();
        _interactAction.Enable();
        _abilityAction.Enable();
        _ultimateAction.Enable();
        _pauseAction.Enable();

        //_playerControls.Player.Fire.performed += OnFire;
        _playerControls.Player.Melee.performed += OnMelee;
        _playerControls.Player.Deflect.performed += OnDeflect;
        _playerControls.Player.Focus.performed += OnFocus;
        _playerControls.Player.SwapWeapon.performed += OnSwapWeapon;
        _playerControls.Player.Reload.performed += OnReload;
        _playerControls.Player.Interaction.performed += OnInteraction;
        _playerControls.Player.Ability.performed += OnAbility;
        _playerControls.Player.Ultimate.performed += OnUltimate;
        _playerControls.Player.Pause.performed += OnPause;

        // cheats
        _healthCheatAction.Enable();
        _staminaCheatAction.Enable();
        _ultCheatAction.Enable();
        _focusCheatAction.Enable();
        _staticHealthCheatAction.Enable();

        _playerControls.Player.HealthCheat.performed += OnHealthCheat;
        _playerControls.Player.StaminaCheat.performed += OnStaminaCheat;
        _playerControls.Player.UltCheat.performed += OnUltCheat;
        _playerControls.Player.FocusCheat.performed += OnFocusCheat;
        _playerControls.Player.StaticHealthCheat.performed += OnStaticHealthCheat;

    }
    private void DisableInputs()
    {
        _moveAction.Disable();
        _lookAction.Disable();
        _fireAction.Disable();
        _meleeAction.Disable();
        _deflectAction.Disable();
        _focusAction.Disable();
        _swapWeaponAction.Disable();
        _reloadAction.Disable();
        _interactAction.Disable();
        _abilityAction.Disable();
        _ultimateAction.Disable();
        _pauseAction.Disable();

        //_playerControls.Player.Fire.performed -= OnFire;
        _playerControls.Player.Melee.performed -= OnMelee;
        _playerControls.Player.Deflect.performed -= OnDeflect;
        _playerControls.Player.Focus.performed -= OnFocus;
        _playerControls.Player.SwapWeapon.performed -= OnSwapWeapon;
        _playerControls.Player.Reload.performed -= OnReload;
        _playerControls.Player.Interaction.performed -= OnInteraction;
        _playerControls.Player.Ability.performed -= OnAbility;
        _playerControls.Player.Ultimate.performed -= OnUltimate;
        _playerControls.Player.Pause.performed -= OnPause;

        // cheats
        _healthCheatAction.Disable();
        _staminaCheatAction.Disable();
        _ultCheatAction.Disable();
        _focusCheatAction.Disable();
        _staticHealthCheatAction.Disable();

        _playerControls.Player.HealthCheat.performed -= OnHealthCheat;
        _playerControls.Player.StaminaCheat.performed -= OnStaminaCheat;
        _playerControls.Player.UltCheat.performed -= OnUltCheat;
        _playerControls.Player.FocusCheat.performed -= OnFocusCheat;
        _playerControls.Player.StaticHealthCheat.performed -= OnStaticHealthCheat;
    }
    #endregion

    #region Input Events
    private void OnMove()
    {
        if (_isExternalControl || !_melee.CanMelee)
            return;

        if (/*(*/_isInputDisabled || _isMovementOnlyDisabled/*) && !_isComboInProgress*/)
        {
            _rb.velocity = Vector3.zero;
            return;
        }
        else if (/*(*/_isInputDisabled || _isMovementOnlyDisabled/*) && _isComboInProgress*/)
        {
            return;
        }

        Vector2 moveVector = _moveAction.ReadValue<Vector2>();
        Vector2 moveDirection = moveVector.normalized;

        _currentMoveVector = moveVector;
        //_currentMoveDirection = moveDirection;
        _rb.velocity = _currentMoveSpeed * Time.fixedUnscaledDeltaTime * moveDirection;

        // Check if there's significant movement to update last move direction
        if (moveVector.magnitude > 0.1f)
            _lastMoveDirection = moveDirection.normalized;
    }
    public void FakeMove(Vector2 direction, float speed)
    {
        Vector2 moveVector = direction;
        Vector2 moveDirection = moveVector.normalized;

        _currentMoveVector = moveVector;
        //_currentMoveDirection = moveDirection;
        _rb.velocity = speed * Time.fixedUnscaledDeltaTime * moveDirection;

        // Check if there's significant movement to update last move direction
        if (moveVector.magnitude > 0.1f)
            _lastMoveDirection = moveDirection.normalized;
    }
    private void OnLook()
    {
        /* crosshair */
        Vector3 mousePosition = _currentMouse.position.value;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = _crosshairTr.position.z;

        // Clamp the cursor position within screen bounds
        mousePosition.x = Mathf.Clamp(mousePosition.x, Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x);
        mousePosition.y = Mathf.Clamp(mousePosition.y, Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y);

        _crosshairTr.position = Vector3.Lerp(_crosshairTr.position, mousePosition, _cursorSensetivity * Time.unscaledDeltaTime);

        /* arm */
        Vector3 cursorPosition = _crosshairTr.position;
        Vector3 direction = cursorPosition - _armPivot.position;
        _armPivot.right = direction;

        CheckArmFlip();
    }
    private void OnFire()
    {
        if (_isInputDisabled || !_currentlyEquippedGun)
            return;

        Shoot();
    }
    private void OnRelease()
    {
        if (!_currentlyEquippedGun)
            return;

        _currentlyEquippedGun.IsFiring = false;
        _currentlyEquippedGun.GunAnimator.SetBool("isFiring", false);

        if (_currentlyEquippedGun is GunHeatable heatableGun)
        {
            heatableGun.IsStartingFire = true;
            //heatableGun.FluffVFX.GetComponent<ParticleSystem>().Stop();
            heatableGun.FluffVFX.gameObject.SetActive(false);
        }
    }
    /*private void OnFire(InputAction.CallbackContext context)
    {
        if (_isInputDisabled || !_currentlyEquippedGun)
            return;

        Shoot();
    }*/
    private void OnMelee(InputAction.CallbackContext context)
    {
        if (_isInputDisabled || _isMovementOnlyDisabled || !_melee.CanMelee)
            return;

        Debug.Log("Melee");
        if (!_ultimate.IsUltStateActive)
            _melee.UseMelee(_rb, _isArmFacingRight);
        else if (_ultimate.IsUltStateActive && _ultimate.CurrentUltCharge >= _ultimate.UltChargeCost)
            _ultimate.UseUltimate();
    }
    private void OnDeflect(InputAction.CallbackContext context)
    {
        if (_isInputDisabled || _isMovementOnlyDisabled || !_melee.CanMelee)
            return;

        //if (context.interaction is HoldInteraction)
        if (_deflect.CanDeflect)
        {
            Debug.Log("Deflect");
            _deflect.Deflect(this, _animations);
        }
    }
    private void OnFocus(InputAction.CallbackContext context)
    {
        if (_isInputDisabled)
            return;

        _focus.HandleFocus();
    }
    private void OnUltimate(InputAction.CallbackContext context)
    {
        if (_isInputDisabled || _isMovementOnlyDisabled)
            return;

        if (!_ultimate.IsUltStateActive && _ultimate.CurrentUltCharge >= _ultimate.MaxUltCharge)
        {
            _ultimate.ActivateUltimateState();
        }
    }
    private void OnSwapWeapon(InputAction.CallbackContext context)
    {
        if (_isInputDisabled || _isMovementOnlyDisabled)
            return;

        SwapGuns();
    }
    private void OnReload(InputAction.CallbackContext context)
    {
        if (_isInputDisabled)
            return;
    }
    private void OnInteraction(InputAction.CallbackContext context)
    {
        EventManager.InvokeInteract();
    }
    /*private void OnAbility()
    {
        if (_isInputDisabled)
            return;

        _ability.UseAbility();
    }*/
    private void OnAbility(InputAction.CallbackContext context)
    {
        if (_isInputDisabled || _isMovementOnlyDisabled)
            return;

        _dash.UseDash();
    }
    private void OnPause(InputAction.CallbackContext context)
    {
        if (_pauseMenu.activeInHierarchy)
            _pauseMenu.SetActive(false);
        else
            _pauseMenu.SetActive(true);

        EventManager.InvokePause();
    }

    // cheats
    private void OnHealthCheat(InputAction.CallbackContext context)
    {
        // trigger health always full
        _isHealthCheatOn = !_isHealthCheatOn;
    }
    private void OnStaminaCheat(InputAction.CallbackContext context)
    {
        // trigger stamina always full
        _isStaminaCheatOn = !_isStaminaCheatOn;

    }
    private void OnUltCheat(InputAction.CallbackContext context)
    {
        // trigger ult always full
        _isUltCheatOn = !_isUltCheatOn;

    }
    private void OnFocusCheat(InputAction.CallbackContext context)
    {
        // trigger ability always full
        _isFocusCheatOn = !_isFocusCheatOn;

    }
    private void OnStaticHealthCheat(InputAction.CallbackContext context)
    {
        // trigger ability always full
        _isStaticHealthCheatOn = !_isStaticHealthCheatOn;

    }
    #endregion
    
    #region General Methods
    private void InitializePlayer()
    {
        _isAlive = true;

        _currentHealth = _data.MaxHealth;
        _currentStamina = _data.MaxStamina;
        LoseGuns();

        if (_lis) _lis.IsLinkCompromised = false;
    }
    private void GetScreenBounds()
    {
        Vector3 screenBottomLeft = _currentCam.ScreenToWorldPoint(new Vector3(0, 0, _currentCam.nearClipPlane));
        Vector3 screenTopRight = _currentCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, _currentCam.nearClipPlane));
        //_screenBounds = new Vector2(screenTopRight.x - screenBottomLeft.x, screenTopRight.y - screenBottomLeft.y);
    }
    private void OnMouseSensetivityChange(Player_Controller controller)
    {
        float systemCursorSpeed = GetSystemCursorSpeed();
        _cursorSensetivity = _cursorSmoothSpeed * systemCursorSpeed;
    }
    private float GetSystemCursorSpeed()
    {
        // Placeholder for actual system cursor speed retrieval logic
        // For now, return a default value (e.g., 1.0f)
        return 1.0f;
    }
    public void ShowUIAndCrosshair(bool shouldShow)
    {
        if (!_hud)
            return;

        if (shouldShow)
        {
            _hud.gameObject.SetActive(true);
            _crosshairTr.gameObject.SetActive(true);
        }
        else
        {
            _hud.gameObject.SetActive(false);
            _crosshairTr.gameObject.SetActive(false);
        }
    }
    public void ShowCursor(bool shouldShow)
    {
        if (shouldShow)
            _crosshairTr.gameObject.SetActive(true);
        else
            _crosshairTr.gameObject.SetActive(false);
    }
    private void ApplyConstantHudChanges()
    {
        if (!_hud)
            return;

        if (_currentHealth > _data.MaxHealth) // quickfix
            _currentHealth = _data.MaxHealth;

        _hud.UpdateHealthSlots(_currentHealth);
        _hud.UpdateStaminaBar(_data.MaxStamina, _currentStamina);
        _hud.UpdateUltBar(_ultimate.MaxUltCharge, _ultimate.CurrentUltCharge);
        _hud.UpdateFocusBar(_focus.MaxFocus, _focus.CurrentFocus);
        _hud.UpdatePrimaryGun(_primaryGun);
        _hud.UpdateSideArm(_sideArm);
    }
    private void FlipArm(bool isFacingRight)
    {
        _isArmFacingRight = !isFacingRight;
        _animations.Animator.SetBool("IsFacingRight", !isFacingRight);
        _armPivot.localScale = new Vector3(_armPivot.localScale.x, -_armPivot.localScale.y, _armPivot.localScale.z);
    }
    private void HandleArmFlipLeft()
    {
        FlipArm(_isArmFacingRight);
        if (_currentlyEquippedGun != null)
            _currentlyEquippedGun.SR.sortingOrder = 2;

        _isArmFacingRight = false;
        _deflect.FlipCollider(_isArmFacingRight);
    }
    private void HandleArmFlipRight()
    {
        // Flip the arm
        FlipArm(_isArmFacingRight);
        if (_currentlyEquippedGun != null)
            _currentlyEquippedGun.SR.sortingOrder = 0;

        _isArmFacingRight = true;
        _deflect.FlipCollider(_isArmFacingRight);
    }
    private void CheckArmFlip()
    {
        // Convert quaternion rotation to euler angles
        Vector3 armRotation = _armPivot.localRotation.eulerAngles;

        // Check if the arm is facing left
        if (armRotation.z >= 90 && armRotation.z <= 270)
        {
            if (_isArmFacingRight)
            {
                HandleArmFlipLeft();
            }
        }
        // Check if the arm is facing right
        else
        {
            if (!_isArmFacingRight)
            {
                HandleArmFlipRight();
            }
        }
    }
    private void Restore()
    {
        _currentBreachRoom.StartCoroutine(_currentBreachRoom.TransferPlayerBack());
        _currentHealth = _data.MaxHealth;
        _currentStamina = _data.MaxStamina;
        StartCoroutine(ResetBreachCounter(3));
    }

    private IEnumerator ResetBreachCounter(float time)
    {
        yield return new WaitForSeconds(time);
        _restoreFromBreachCounter = 0;
    }

    private void ResetColors()
    {
        if (_spritesToFade.Length < 1) return;
        for (int i = 0; i < _spritesToFade.Length; i++)
        {
            SpriteRenderer sR = _spritesToFade[i];
            if (sR) sR.color = Color.white;
        }
        _animations.IsFlashing = false; // Reset the flag after damage flash is complete
    }
    public void ApplySkill(StatType statType, float factor)
    {
        switch (statType)
        {
            case StatType.TimeToMeleeHit:
                /*for (int i = 0; i < _timeToHit.Length; i++)
                    _timeToHit[i] -= Mathf.RoundToInt(GetValuePercentage(_timeToHit[i], value));

                _timeToHitValue = _timeToHit[0];*/
                break;

            case StatType.FocusCost:
                _focus.FocusCost *= factor;
                break;

            case StatType.DodgeRate:
                break;
            case StatType.DashSpeed:
                break;
            case StatType.AmmoCount:
                break;

            case StatType.MaxHealth:
                _data.MaxHealth = Mathf.RoundToInt(_data.MaxHealth * factor);
                break;

            case StatType.MeleeDamage:
                _data.MeleeDamage *= factor;
                break;

            case StatType.PickupHealthRate:
                break;

            case StatType.Speed:
                _currentMoveSpeed *= factor;
                break;

            case StatType.ReloadTime:
                break;
            case StatType.FireRate:
                break;
            default:
                break;
        }
    }

    private IEnumerator DeathRoutine()
    {
        _isAlive = false;
        //_hud.gameObject.SetActive(false);

        yield return StartCoroutine(_fader.FadeOutRoutine(0.5f));
    }
    private void Die()
    {
        StartCoroutine(DeathRoutine());
        ResetColors();
        _animations.DeathAnimation();
        _lis.IsLinkCompromised = false;
        IsInputDisabled = true;
        IsMovementOnlyDisabled = true;
        HUD.BackgroundFade.SetActive(true);

        SaveManager.Instance.SaveGame(); // need to fix, see if relevant
        _deathMenu.ActivateDeathCanvas();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        return;
    }
    public void ReverseDeathVFX()
    {
        _animations.DesyncAnimation();
        //_lis.IsLinkCompromised = true;
        _lis.LinkHudAnimator.SetBool("Dead", false);
        IsInputDisabled = false;
        IsMovementOnlyDisabled = false;
        HUD.BackgroundFade.SetActive(false);
        _lis.LinkHudAnimator.Play("LinkIntegrityHUD_Idle_Anim");

        if (SceneManager.GetActiveScene().buildIndex != (int)SceneType.Training)
            _hud.gameObject.SetActive(true);
    }


    private IEnumerator ReviveRoutine()
    {
        yield return StartCoroutine(_fader.FadeInRoutine(0.5f));

        _isAlive = true;
        _hud.gameObject.SetActive(true);
    }
    private IEnumerator ReviveRoutine(UnityEngine.SceneManagement.Scene scene)
    {
        yield return StartCoroutine(_fader.FadeInRoutine(0.5f));

        _isAlive = true;

        if (scene.buildIndex != (int)SceneType.Training)
            _hud.gameObject.SetActive(true);
    }
    public void Revive(bool isRoutine)
    {
        if (isRoutine)
            StartCoroutine(ReviveRoutine());
        else
            _isAlive = true;
    }
    private void Revive(UnityEngine.SceneManagement.Scene scene)
    {
        StartCoroutine(ReviveRoutine(scene));
    }

    private IEnumerator ResetCameraTarget()
    {
        if (!_focus.IsFocusing)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            _currentVirtualCamera.Follow = _playerCamTargetOG;
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.025f);
            _currentVirtualCamera.Follow = _playerCamTargetOG;
        }
    }
    private IEnumerator ResetPickup()
    {
        _isPickuping = true;

        yield return new WaitForSecondsRealtime(_pickupDelay);
        _isPickuping = false;
    }
    private IEnumerator HandleKnockback(Vector2 normalizedAttackDirection, float knockBackPower)
    {
        if (_isKnockedBack || !_canBeKnockedBack)
            yield break;

        float elapsedTime = 0f;
        Vector3 originalPos = transform.position;
        Vector3 targetPos = originalPos + (Vector3)normalizedAttackDirection * knockBackPower;

        _isKnockedBack = true;
        while (elapsedTime < _knockBackTime)
        {
            float t = elapsedTime / _knockBackTime;
            float easeOutT = 1 - Mathf.Pow(1 - t, 3);  // Cubic ease-out
            transform.position = Vector3.Lerp(originalPos, targetPos, easeOutT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        _isKnockedBack = false;
    }

    public void StartResetCameraTarget()
    {
        StartCoroutine(ResetCameraTarget());
    }
    public void StartResetPickup()
    {
        StartCoroutine(ResetPickup());
    }
    public void SetTrainingCheats(bool shouldActivate)
    {
        _isTrainingCheatsOn = shouldActivate;
    }
    #endregion

    #region Currency
    private void UpdateCurrency()
    {
        _currencyToAdd += _data.Currency;
        _updateCurrencyRoutine = StartCoroutine(UpdateCurrencyRoutine(_data.Currency));
    }
    private IEnumerator UpdateCurrencyRoutine(int amount)
    {
        TextMeshProUGUI currencyText = _hud.CurrencyTxt;
        TextMeshProUGUI currencySubText = _hud.CurrencySubTxt;
        TextMeshProUGUI currencySubTextPlus = _hud.CurrencySubTxtPlus;
        currencySubText.text = _data.Currency.ToString();

        float elapsedTime = 0f;
        if (currencySubText.text == _data.Currency.ToString())
        {
            currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
            currencySubText.gameObject.SetActive(true);
            currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
            currencySubTextPlus.gameObject.SetActive(true);

            // fade in sub text
            while (elapsedTime < _subCurrencyFadeDuration)
            {
                currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, elapsedTime / _subCurrencyFadeDuration);
                currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, elapsedTime / _subCurrencyFadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1);
            currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1);
        }
        yield return new WaitForSeconds(_delayAfterGain / 2);

        // count up to the total amount and update CurrencyTxt
        int tempCurrencyToAdd = _currencyToAdd;
        int currentCurrency = 0;
        elapsedTime = 0f;

        while (currentCurrency < _data.Currency)
        {
            // count up currencyText
            currentCurrency = Mathf.Min(currentCurrency + Mathf.RoundToInt(_subCurrencyIncrementSpeed * 16 * Time.deltaTime), _data.Currency);
            currencyText.text = currentCurrency.ToString();

            // count down currencySubText
            tempCurrencyToAdd = Mathf.Max(tempCurrencyToAdd - Mathf.RoundToInt(_subCurrencyIncrementSpeed * 16 * Time.deltaTime), 0);
            currencySubText.text = tempCurrencyToAdd.ToString();
            yield return null;
        }
        currencyText.text = currentCurrency.ToString();
        currencySubText.text = 0.ToString();
        yield return HandleGainCurrencyIconAnimation();

        // fade out sub text
        elapsedTime = 0f;
        while (elapsedTime < _subCurrencyFadeDuration)
        {
            currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1 - (elapsedTime / _subCurrencyFadeDuration));
            currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1 - (elapsedTime / _subCurrencyFadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
        currencySubText.gameObject.SetActive(false);
        currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
        currencySubTextPlus.gameObject.SetActive(false);

        //_data.Currency += totalAmount;
        _currencyToAdd = 0;
        _totalCurrencyAdded = 0;
        _updateCurrencyRoutine = null;
    }

    private void GainCurrency(int amount)
    {
        _currencyToAdd += amount;

        if (_payCurrencyRoutine != null)
            StopCoroutine(_payCurrencyRoutine);

        if (_gainCurrencyRoutine != null)
            StopCoroutine(_gainCurrencyRoutine);

        _gainCurrencyRoutine = StartCoroutine(GainCurrencyRoutine(amount));
    }
    private IEnumerator GainCurrencyRoutine(int initialAmount)
    {
        TextMeshProUGUI currencyText = _hud.CurrencyTxt;
        TextMeshProUGUI currencySubText = _hud.CurrencySubTxt;
        TextMeshProUGUI currencySubTextPlus = _hud.CurrencySubTxtPlus;

        float elapsedTime = 0f;
        if (currencySubText.text == 0.ToString())
        {
            currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
            currencySubText.gameObject.SetActive(true);
            currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
            currencySubTextPlus.gameObject.SetActive(true);

            // fade in sub text
            while (elapsedTime < _subCurrencyFadeDuration)
            {
                currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, elapsedTime / _subCurrencyFadeDuration);
                currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, elapsedTime / _subCurrencyFadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1);
            currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1);
        }

        // continue counting until it reaches _currencyToAdd
        elapsedTime = 0f;
        float delayTimer = 0f;
        while (_totalCurrencyAdded < _currencyToAdd)
        {
            _totalCurrencyAdded = Mathf.Min(_totalCurrencyAdded + Mathf.RoundToInt(_subCurrencyIncrementSpeed * Time.deltaTime), _currencyToAdd);
            currencySubText.text = _totalCurrencyAdded.ToString();
            yield return null;
        }
        currencySubText.text = _currencyToAdd.ToString();

        while (delayTimer < _delayAfterGain)
        {
            if (_totalCurrencyAdded < _currencyToAdd)
            {
                yield return GainCurrencyRoutine(0);
                yield break;
            }
            delayTimer += Time.deltaTime;
            yield return null;
        }

        // count up to the total amount and update CurrencyTxt
        int totalAmount = _currencyToAdd;
        int tempCurrencyToAdd = _currencyToAdd;
        int currentCurrency = _data.Currency;
        elapsedTime = 0f;

        while (currentCurrency < _data.Currency + totalAmount)
        {
            // count up currencyText
            currentCurrency = Mathf.Min(currentCurrency + Mathf.RoundToInt(_subCurrencyIncrementSpeed * Time.deltaTime), _data.Currency + totalAmount);
            currencyText.text = currentCurrency.ToString();

            // count down currencySubText
            tempCurrencyToAdd = Mathf.Max(tempCurrencyToAdd - Mathf.RoundToInt(_subCurrencyIncrementSpeed * Time.deltaTime), 0);
            currencySubText.text = tempCurrencyToAdd.ToString();
            yield return null;
        }
        currencyText.text = currentCurrency.ToString();
        currencySubText.text = 0.ToString();
        yield return HandleGainCurrencyIconAnimation();

        // fade out sub text
        elapsedTime = 0f;
        while (elapsedTime < _subCurrencyFadeDuration)
        {
            currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1 - (elapsedTime / _subCurrencyFadeDuration));
            currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1 - (elapsedTime / _subCurrencyFadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
        currencySubText.gameObject.SetActive(false);
        currencySubTextPlus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
        currencySubTextPlus.gameObject.SetActive(false);

        _data.Currency += totalAmount;
        _currencyToAdd = 0;
        _totalCurrencyAdded = 0;
        _gainCurrencyRoutine = null;
    }
    private IEnumerator HandleGainCurrencyIconAnimation()
    {
        Image currencyIcon = _hud.CurrencyIcon;

        // Shrink currency icon
        Vector3 currentIconScale = currencyIcon.transform.localScale;
        float elapsedTime = 0f;
        while (elapsedTime < _iconShrinkDuration)
        {
            currencyIcon.transform.localScale = Vector3.Lerp(currentIconScale, _targetShrinkSize, elapsedTime / _iconShrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currencyIcon.transform.localScale = _targetShrinkSize;

        // Grow currency icon
        currentIconScale = currencyIcon.transform.localScale;
        elapsedTime = 0f;
        while (elapsedTime < _iconGrowDuration)
        {
            currencyIcon.transform.localScale = Vector3.Lerp(currentIconScale, _targetGrowSize, elapsedTime / _iconGrowDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currencyIcon.transform.localScale = _targetGrowSize;
    }

    private void PayCurrency(int amount)
    {
        _currencyToAdd += amount;

        if (_gainCurrencyRoutine != null)
            StopCoroutine(_gainCurrencyRoutine);

        if (_payCurrencyRoutine != null)
            StopCoroutine(_payCurrencyRoutine);

        _payCurrencyRoutine = StartCoroutine(PayCurrencyRoutine(amount));
    }
    private IEnumerator PayCurrencyRoutine(int initialAmount)
    {
        TextMeshProUGUI currencyText = _hud.CurrencyTxt;
        TextMeshProUGUI currencySubText = _hud.CurrencySubTxt;
        TextMeshProUGUI currencySubTextMinus = _hud.CurrencySubTxtMinus;

        float elapsedTime = 0f;

        // Fade in the currency subtext if it is 0
        if (currencySubText.text == 0.ToString())
        {
            currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
            currencySubText.gameObject.SetActive(true);
            currencySubTextMinus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
            currencySubTextMinus.gameObject.SetActive(true);

            while (elapsedTime < _subCurrencyFadeDuration)
            {
                currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, elapsedTime / _subCurrencyFadeDuration);
                currencySubTextMinus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, elapsedTime / _subCurrencyFadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1);
            currencySubTextMinus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1);
        }
        currencySubText.text = _currencyToAdd.ToString();
        yield return new WaitForSeconds(_delayAfterGain / 2);

        // Countdown logic for both currency text and subtext
        int tempCurrencyToAdd = _currencyToAdd;
        int currentCurrency = _data.Currency;
        int targetCurrency = _data.Currency - initialAmount;
        elapsedTime = 0f;

        // Begin counting down until currentCurrency matches targetCurrency
        while (currentCurrency > targetCurrency)
        {
            // Count down currencyText (main text)
            currentCurrency = Mathf.Max(currentCurrency - Mathf.RoundToInt(_subCurrencyIncrementSpeed * Time.deltaTime), targetCurrency);
            _data.Currency = currentCurrency; // Update actual currency value progressively
            currencyText.text = currentCurrency.ToString();

            // Count down currencySubText
            tempCurrencyToAdd = Mathf.Max(tempCurrencyToAdd - Mathf.RoundToInt(_subCurrencyIncrementSpeed * Time.deltaTime), 0);
            currencySubText.text = tempCurrencyToAdd.ToString();

            yield return null;
        }

        // Ensure final values are set after the loop
        currencyText.text = _data.Currency.ToString();
        currencySubText.text = 0.ToString();

        // Handle the icon animation
        yield return HandlePayCurrencyIconAnimation();

        // Fade out subtext after the icon animation completes
        elapsedTime = 0f;
        while (elapsedTime < _subCurrencyFadeDuration)
        {
            currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1 - (elapsedTime / _subCurrencyFadeDuration));
            currencySubTextMinus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 1 - (elapsedTime / _subCurrencyFadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currencySubText.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
        currencySubText.gameObject.SetActive(false);
        currencySubTextMinus.color = new Color(currencySubText.color.r, currencySubText.color.g, currencySubText.color.b, 0);
        currencySubTextMinus.gameObject.SetActive(false);

        _currencyToAdd = 0;
        _totalCurrencyAdded = 0;
        _payCurrencyRoutine = null;
    }
    private IEnumerator HandlePayCurrencyIconAnimation()
    {
        Image currencyIcon = _hud.CurrencyIcon;

        // Shrink currency icon
        Vector3 currentIconScale = currencyIcon.transform.localScale;
        float elapsedTime = 0f;
        while (elapsedTime < _iconShrinkDuration)
        {
            currencyIcon.transform.localScale = Vector3.Lerp(currentIconScale, _targetShrinkSize, elapsedTime / _iconShrinkDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currencyIcon.transform.localScale = _targetShrinkSize;

        // Grow currency icon
        currentIconScale = currencyIcon.transform.localScale;
        elapsedTime = 0f;
        while (elapsedTime < _iconGrowDuration)
        {
            currencyIcon.transform.localScale = Vector3.Lerp(currentIconScale, _targetGrowSize, elapsedTime / _iconGrowDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        currencyIcon.transform.localScale = _targetGrowSize;
    }
    #endregion

    #region Health Methods
    private void TakeDamage(Vector2 normalizedAttackDirection, float damage, float knockBackPower)
    {
        if (!_isAlive) return;
        _currentHealth -= damage;

        _characterAudioSource.pitch = UnityEngine.Random.Range(1f, 2f);
        _characterAudioSource.PlayOneShot(_characterTakeDamageAC);

        if (!_isKnockedBack && normalizedAttackDirection != Vector2.zero && knockBackPower > 0)
            StartCoroutine(HandleKnockback(normalizedAttackDirection, knockBackPower));

        _animations.DoDamageFlash(_data.DamageColor);

        /*        if (_damageVFX)
        */
        ScreenDamageUpdate();

        _hud.UpdateHealthSlots(_currentHealth);
        //StartCoroutine(UpdateDamageUI());

        _currentVirtualCamera.GetComponent<CinemachineShake>().StartCameraShake(0.5f, 1.0f);

        if (_currentHealth <= 0)
        {
            if (!_isInBreach)
            {
                Die();
            }
            else if (_restoreFromBreachCounter == 0)
            {
                Restore();
                _restoreFromBreachCounter++;
            }
        }
    }
    public void ReplanishHealth()
    {
        _currentHealth = _data.MaxHealth;
    }
    #endregion

    #region Stamina Methods
/*    private IEnumerator RegenerateStamina()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.1f); // Check more frequently

            // Check if enough time has passed since last stamina use
            if (_timeSinceUsedStamina >= _data.StaminaRegenDelay && _currentStamina < _data.MaxStamina)
            {
                // Start regenerating stamina
                float targetStamina = Mathf.Min(_currentStamina + _data.StaminaRegenRate * Time.unscaledDeltaTime * _data.StaminaRegenTime, _data.MaxStamina);
                _currentStamina = Mathf.Lerp(_currentStamina, targetStamina, Time.unscaledDeltaTime * _data.StaminaRegenTime);
            }
            else
            {
                // Increment time since last stamina use
                _timeSinceUsedStamina += Time.unscaledDeltaTime;
            }
        }
    }*/

    public void UseStamina(float amount)
    {
        _currentStamina -= amount;
    }
    #endregion

    #region Gun Methods
    public void HostlerGun()
    {
        switch (_currentlyEquippedGun)
        {
            case GunPrimary:
                _primaryGun.SR.enabled = false;
                _primaryGun.IsEquipped = false;
                _primaryGun.CanFire = false;
                _primaryGun.transform.SetParent(transform);
                _primaryGun.transform.SetLocalPositionAndRotation(_primaryOffHand.localPosition, Quaternion.identity);

                break;
            case GunSideArm:
                _sideArm.SR.enabled = false;
                _sideArm.IsEquipped = false;
                _sideArm.CanFire = false;
                _sideArm.transform.SetParent(transform);
                _sideArm.transform.SetLocalPositionAndRotation(_sideArmOffHand.localPosition, Quaternion.identity);

                break;
            default:
                break;
        }
    }
    private void ReturnGun(GunBase gun, Transform gunPlacementTr)
    {
        if (!gun)
            return;

        gun.IsEquipped = false;
        gun.CanFire = false;
        gun.IsFiring = false;
        gun.transform.SetParent(null);
        gun.transform.position = gunPlacementTr.position;

        Vector2 scale = gun.transform.localScale;
        gun.transform.localScale = new Vector2(Math.Abs(scale.x), Math.Abs(scale.y));
        gun.SR.enabled = true;
        gun.HighlightVFX.SetActive(true);
        gun.IsOnFloor = true;
        gun.Pickup.gameObject.SetActive(true);
        gun.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (gun == _currentlyEquippedGun)
            _currentlyEquippedGun = null;
    }
    public void LoseGuns()
    {
        if (!_currentlyEquippedGun && !_primaryGun && !_sideArm)
            return;

        if (_primaryGun)
        {
            Destroy(_primaryGun.gameObject);
            _primaryGun = null;
        }
        if (_sideArm)
        {
            Destroy(_sideArm.gameObject);
            _sideArm = null;
        }

        _currentlyEquippedGun = null;
    }

    public void ReplacePrimaryGun(GunPrimary newPrimary, Transform gunPlacementTr)
    {
        if (_primaryGun)
        {
            _primaryGun.Pickup.WorldTr = gunPlacementTr;
            ReturnGun(_primaryGun, _primaryGun.Pickup.WorldTr);
            _primaryGun = null;
        }

        _primaryGun = newPrimary;
        _primaryGun.HighlightVFX.SetActive(false);
        _primaryGun.Pickup.gameObject.SetActive(false);
        _primaryGun.IsOnFloor = false;

        int tempTier = 0;
        if (!_data.UnlockableGunIds.ContainsKey(_primaryGun.Uid))
        {
            int _randomVoiceLine = UnityEngine.Random.Range(0, _newWeaponPickupJakeAC.Length);
            EventManager.InvokeUpdateStatusEffect(true, "NEW WEAPON AQUIRED", _newWeaponPickupJakeAC[_randomVoiceLine]);
/*            _data.UnlockableGunIds.Remove(_primaryGun.Uid);
*/            _data.UnlockableGunIds.Add(_primaryGun.Uid, _primaryGun.Tier);
        }
        else if (_data.UnlockableGunIds.TryGetValue(_primaryGun.Uid, out tempTier))
        {
            if (tempTier > _primaryGun.Tier)
            {
                _data.UnlockableGunIds.Remove(_primaryGun.Uid);
                _data.UnlockableGunIds.Add(_primaryGun.Uid, _primaryGun.Tier);
            }
        }

        _data.EquippedPrimaryId = _primaryGun.Uid;

        EquipPrimaryGun();
    }
    private void EquipPrimaryGun()
    {
        if (_currentlyEquippedGun)
            HostlerGun();

        // Initialize Gun
        _primaryGun.transform.SetParent(_armPivot);
        _primaryGun.transform.SetLocalPositionAndRotation(_primaryOnHand.localPosition, Quaternion.identity);
        _primaryGun.SR.enabled = true;
        _primaryGun.IsEquipped = true;
        _primaryGun.CanFire = true;

        if (!_isArmFacingRight)
        {
            _primaryGun.transform.localScale = Vector3.one;
            _primaryGun.transform.localRotation = Quaternion.identity;
            _primaryGun.SR.sortingOrder = (int)FaceDirection.Left;
        }
        else if (_isArmFacingRight)
        {
            _primaryGun.transform.localScale = Vector3.one;
            _primaryGun.transform.localRotation = Quaternion.identity;
            _primaryGun.SR.sortingOrder = (int)FaceDirection.Right;
        }

        _currentlyEquippedGun = _primaryGun;
    }

    public void ReplaceSideArm(GunSideArm sideArm, Transform gunPlacementTr)
    {
        if (_sideArm)
        {
            _sideArm.Pickup.WorldTr = gunPlacementTr;
            ReturnGun(_sideArm, gunPlacementTr);
            _sideArm = null;
        }

        _sideArm = sideArm;
        _sideArm.HighlightVFX.SetActive(false);
        _sideArm.Pickup.gameObject.SetActive(false);
        _sideArm.IsOnFloor = false;

        int tempTier = 0;

        if (!_data.UnlockableGunIds.ContainsKey(_sideArm.Uid))
        {
            _data.UnlockableGunIds.Add(_sideArm.Uid, _sideArm.Tier);
            int _randomVoiceLine = UnityEngine.Random.Range(0, _newWeaponPickupJakeAC.Length);
            EventManager.InvokeUpdateStatusEffect(true, "NEW WEAPON AQUIRED", _newWeaponPickupJakeAC[_randomVoiceLine]);
        }
        else if (_data.UnlockableGunIds.TryGetValue(_sideArm.Uid, out tempTier))
        {
            if (tempTier > _sideArm.Tier)
            {
                _data.UnlockableGunIds.Remove(_sideArm.Uid);
                _data.UnlockableGunIds.Add(_sideArm.Uid, _sideArm.Tier);
            }
        }

        _data.EquippedSideArmId = _sideArm.Uid;

        EquipSideArm();
    }
    private void EquipSideArm()
    {
        if (_currentlyEquippedGun)
            HostlerGun();

        // Initialize Gun
        _sideArm.transform.SetParent(_armPivot);
        _sideArm.transform.SetLocalPositionAndRotation(_sideArmOnHand.localPosition, Quaternion.identity);
        _sideArm.SR.enabled = true;
        _sideArm.IsEquipped = true;
        _sideArm.CanFire = true;

        if (!_isArmFacingRight)
        {
            _sideArm.transform.localScale = Vector3.one;
            _sideArm.transform.localRotation = Quaternion.identity;
            _sideArm.SR.sortingOrder = (int)FaceDirection.Left;
        }
        else if (_isArmFacingRight)
        {
            _sideArm.transform.localScale = Vector3.one;
            _sideArm.transform.localRotation = Quaternion.identity;
            _sideArm.SR.sortingOrder = (int)FaceDirection.Right;
        }

        _currentlyEquippedGun = _sideArm;
    }

    public void SwapGuns()
    {
        if (!_currentlyEquippedGun || !_primaryGun || !_sideArm)
            return;

        OnRelease();
        HostlerGun();

        if (_currentlyEquippedGun == _sideArm)
            EquipPrimaryGun();
        else if (_currentlyEquippedGun == _primaryGun)
            EquipSideArm();
    }

    private void Shoot()
    {
        if (_currentlyEquippedGun.CanFire)
            _currentlyEquippedGun.Shoot();
    }
    #endregion

    #region Cheats
    private void HealthCheat()
    {
        if (_isHealthCheatOn || _isTrainingCheatsOn)
            _currentHealth = _data.MaxHealth;
    }
    private void StaminaCheat()
    {
        if (_isStaminaCheatOn || _isTrainingCheatsOn)
            _currentStamina = _data.MaxStamina;
    }
    private void UltCheat()
    {
        if (_isUltCheatOn || _isTrainingCheatsOn)
            _ultimate.CurrentUltCharge = _ultimate.MaxUltCharge;
    }
    private void FocusCheat()
    {
        if (_isFocusCheatOn || _isTrainingCheatsOn)
            _focus.CurrentFocus = _focus.MaxFocus;
    }
    private void StaticHealthCheat()
    {
        if (!_isStaticHealthCheatOn)
            _tempHealthForCheats = _currentHealth;
        else
            _currentHealth = _tempHealthForCheats;
    }
    public void SetStaticHealthCheat(bool isTrue)
    {
        _isStaticHealthCheatOn = isTrue;
    }
    private void CheckUpdateCheats()
    {
        HealthCheat();
        StaminaCheat();
        UltCheat();
        FocusCheat();
    }
    #endregion

    /*private IEnumerator DamageScreenEffect()
    {
                _damageVFX.SetActive(true);
                _damageVolume.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(_damageVFXTime);
                _damageVFX.SetActive(false);
                _damageVolume.gameObject.SetActive(false);

    }*/

    private void ScreenDamageUpdate()
    {
        Color color = _damageImage.color;

        color.a += _damageIncreaseAmount;
        color.a = Mathf.Clamp01(color.a); // Keep alpha within 0-1 range
        _damageImage.color = color;
    }

    #region UI
    /*private IEnumerator UpdateDamageUI()
    {
        if (_isTakingDamage)
            yield break;

        _isTakingDamage = true;

        //_hud.UpdateDamage(_damageVFXTime, 1.25f);
        yield return new WaitForSecondsRealtime(_damageVFXTime);
        _isTakingDamage = false;
    }*/
    #endregion

    #region VFXs
    public void FadeOut()
    {
        _focus.ResumeTime();
        StartCoroutine(_animations.FadeOutPlayer());

        if (_currentlyEquippedGun) // might need fixing
            StartCoroutine(_currentlyEquippedGun.FadeOutWeapon());
    }
    public void FadeIn()
    {
        StartCoroutine(_animations.FadeInPlayer());

        if (_currentlyEquippedGun) // might need fixing
            StartCoroutine(_currentlyEquippedGun.FadeInWeapon());
    }
    #endregion

    #region StatusDialogues
    [Header("Status Voice Lines")]

    [SerializeField] private AudioClip[] _newWeaponPickupJakeAC;
    [SerializeField] private AudioClip[] _reloadWeaponJakeAC;

    #endregion

    #region Events
    private void OnUpdateCurrency()
    {
        if (_hud) _hud.CurrencyTxt.text = _data.Currency.ToString();
        //UpdateCurrency();
    }
    private void OnGainCurrency(int amount)
    {
        _data.Currency += amount;
        EventManager.InvokeUpdateCurrency();
        //GainCurrency(amount);
    }
    private void OnPayCurrency(int amount)
    {
        _data.Currency -= amount;
        EventManager.InvokeUpdateCurrency();
        //PayCurrency(amount);
    }

    private void OnPlayerHit(Vector2 normalizedAttackDirection, float damage, float knockBackPower)
    {
        TakeDamage(normalizedAttackDirection, damage, knockBackPower);
    }
    private void OnPickupConsumable(float amount)
    {
        if (_currentHealth < _data.MaxHealth)
            _currentHealth += amount;
        else if (_currentHealth >= _data.MaxHealth)
            _currentHealth = _data.MaxHealth;
    }
    private void OnCutscene(bool isStarting)
    {
        // change UI and crosshair visibility
        _animations.StopAnimations(isStarting);
        _rb.velocity = Vector3.zero;
        _isInCutscene = isStarting;
        _isInputDisabled = isStarting;
        _shouldStopUpdates = isStarting;
    }

    private void OnEndGame()
    {
        _hud.gameObject.SetActive(false);
    }
    private void OnTimeFreeze(float timeToFreeze)
    {

    }

    // IPersistable
    private void OnSceneChanged(UnityEngine.SceneManagement.Scene currentScene, UnityEngine.SceneManagement.Scene nextScene)
    {
        if ((SceneType)currentScene.buildIndex == SceneType.Tutorial || (SceneType)nextScene.buildIndex == SceneType.Training)
        {
            LoseGuns();
        }
    }


    /*private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode loadMode)
    {
        if (!_isAlive)
            Revive(scene);

        ResetColors();
    }*/

    private void OnSkillAquired(StatType statType, float factor)
    {
        switch (statType)
        {
            case StatType.MaxHealth:
                int tempMaxHealth = _data.MaxHealth;
                _data.MaxHealth = Mathf.RoundToInt(_data.MaxHealth * factor);
                _data.MaxHealthFactor = factor;
                _currentHealth += _data.MaxHealth - tempMaxHealth;
                _hud.UpdateHealthSlots(_currentHealth);
                break;
            case StatType.MeleeDamage:
                _data.MeleeDamage *= factor;
                _data.MeleeDamageFactor = factor;
                break;
            case StatType.FocusCost:
                _focus.FocusCost *= factor;
                _data.FocusCostFactor = factor;
                break;
        }
    }
    private void OnGunUpgrade(GunBase gun)
    {
        // replace current weapon with one tier higher
        GunBase newGun = _currentlyEquippedGun;
        newGun = Instantiate(newGun.GunTiers[newGun.Tier], newGun.transform.position, Quaternion.identity);
        newGun.gameObject.SetActive(false);
        LoseGuns();
        newGun.gameObject.SetActive(true);
    }

    public void SaveData(ref Profile data)
    {
        data.UnlockableGunIds = _data.UnlockableGunIds;
        data.IsFirstTimeInTraining = IsFirstTimeInTraining;
    }
    public void LoadData(Profile data)
    {
        transform.position = data.PlayerPos;
        _currentHealth = data.MaxHealth;
        _focus.CurrentFocus = 0.0f;
        _ultimate.CurrentUltCharge = 0.0f;
        _ultimate.RefilUlt();

        _data.UnlockableGunIds = data.UnlockableGunIds;
        
        if (_hud)
        {
            _hud.UpdateHealthSlots(_currentHealth);
            _hud.UpdateStaminaBar(_data.MaxStamina, _currentStamina);
            _hud.UpdateUltBar(_ultimate.MaxUltCharge, _ultimate.CurrentUltCharge);
            _hud.UpdateFocusBar(_focus.MaxFocus, _focus.CurrentFocus);
            _hud.UpdatePrimaryGun(_primaryGun);
            _hud.UpdateSideArm(_sideArm);
        }

        IsFirstTimeInTraining = data.IsFirstTimeInTraining;
    }
    #endregion
}
