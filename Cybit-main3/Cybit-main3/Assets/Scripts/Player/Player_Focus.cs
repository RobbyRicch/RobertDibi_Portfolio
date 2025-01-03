using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Focus : MonoBehaviour, IProfileSaveable
{
    [Header("Components")]
    [SerializeField] private AudioSource _focusAudioSource;
    [SerializeField] private AudioClip _focusSFX;

    [Header("Data")]
    [SerializeField] private float _focusCd = 0.35f;
    [SerializeField] private float _focusTimeScaleTarget = 0.2f;
    [SerializeField] private bool _isFocusAvailable = true;

    [SerializeField] private int _maxFocus = 100;
    public int MaxFocus { get => _maxFocus; set => _maxFocus = value; }

    [SerializeField] private float _currentFocus;
    public float CurrentFocus { get => _currentFocus; set => _currentFocus = value; }

    [SerializeField] private float _focusCost = 10.0f;
    public float FocusCost { get => _focusCost; set => _focusCost = value; }

    [SerializeField] private float _originalFocusCost = 10.0f;
    public float OriginalFocusCost => _originalFocusCost;

    [SerializeField] private bool _isFocusing = false;
    public bool IsFocusing => _isFocusing;

    private Coroutine _focusRoutine = null;
    private Coroutine _resetCDRoutine = null;
    private float _originalTimeScale = 1.0f;

    [Header("VFX")]
    [SerializeField] private Animator _cameraAnimator;
    public Animator CameraAnimator { get => _cameraAnimator; set => _cameraAnimator = value; }

    [SerializeField] private GameObject _focusVolume;
    public GameObject FocusVolume { get => _focusVolume; set => _focusVolume = value; }

    [SerializeField] private GameObject _focusDummyVFX;
    public GameObject FocusDummyVFX { get => _focusDummyVFX; set => _focusDummyVFX = value; }

    #region MonoBehaviour Callbacks
    private void OnEnable()
    {
        EventManager.OnGainFocus += OnGainFocus;
    }
    private void Start()
    {
        InitializeFocuse();
    }
    private void OnDisable()
    {
        EventManager.OnGainFocus -= OnGainFocus;
    }
    #endregion

    #region Methods
    private void InitializeFocuse()
    {
        _focusDummyVFX.SetActive(false);
    }
    private void GainFocus(float amount)
    {
        _currentFocus += amount;
    }
    private void UseFocus()
    {
        _currentFocus -= _focusCost * (Time.unscaledDeltaTime * 2);
        _currentFocus = Mathf.Clamp(_currentFocus, 0, _maxFocus);
    }
    private IEnumerator ResetCD()
    {
        yield return new WaitForSecondsRealtime(_focusCd);
        _isFocusAvailable = true;
    }
    private IEnumerator FocusRoutine()
    {
        _isFocusing = true;
        _isFocusAvailable = false;
        _originalTimeScale = Time.timeScale;

        Time.timeScale = Mathf.Lerp(Time.timeScale, _focusTimeScaleTarget, 1.25f); // enter bullet time
        _cameraAnimator.SetBool("FocusON", true); // adjust player speed according to bullet time
        _focusVolume.SetActive(true);
        _focusDummyVFX.transform.SetParent(null);
        _focusDummyVFX.SetActive(true);

        if (_focusAudioSource != null)
            _focusAudioSource.PlayOneShot(_focusSFX);

        EventManager.InvokeFocus(true);
        while (_isFocusing)
        {
            if (_currentFocus <= 0)
            {
                ResumeTime();
                _focusRoutine = null;
                yield break;
            }
            else
            {
                UseFocus();
            }
            yield return null;
        }
    }

    public void ResumeTime()
    {
        if (_isFocusing)
        {
            _resetCDRoutine = StartCoroutine(ResetCD());

            _isFocusing = false;
            Time.timeScale = _originalTimeScale;
            _cameraAnimator.SetBool("FocusON", false);
            _focusVolume.SetActive(false);
            _focusDummyVFX.SetActive(false);
            _focusDummyVFX.transform.SetParent(transform);
            EventManager.InvokeFocus(false);
        }
    }
    public void HandleFocus()
    {
        if (!_isFocusing && _currentFocus > _focusCost)
            _focusRoutine = StartCoroutine(FocusRoutine());
        else if (_isFocusing && !_isFocusAvailable)
            ResumeTime();
    }
    #endregion

    #region Events
    private void OnGainFocus(float focusToGain)
    {
        GainFocus(focusToGain);
    }

    public void SaveData(ref Profile data)
    {
        data.FocusCost = _focusCost;
    }
    public void LoadData(Profile data)
    {
        _focusCost = data.FocusCost;
    }
    #endregion
}