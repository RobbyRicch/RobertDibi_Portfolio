using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LinkIntegritySystem : MonoBehaviour, IProfileSaveable, IPauseable
{
    [Header("Config")]
    [SerializeField] public int _maxIntegrity = 250;
    [SerializeField] public float _currentIntegrity = 0.0f;
    [SerializeField] private Color _startColor = Color.green;
    [SerializeField] private Color _endColor = Color.red;

    [SerializeField] public float _integrityFailureFactor = 1;
    private int lastPercentage = 100; // should start at full value which is currently 100%

    [SerializeField] private float _integrityThresholdDivider = 3.0f;
    private float _integrityThreshold = 0.0f;

    [Header("Breach")]
    [SerializeField] private List<GameObject> _breachDoors;
    [SerializeField] private float _breachLinkThreshold = 220.0f;
    [SerializeField] public float _lowLinkOverlayThreshold;

    [Header("HUD")]
    [SerializeField] private GameObject _linkGroup;
    [SerializeField] private Image _linkBg;
    [SerializeField] private Image _linkPulse;
    [SerializeField] private Image _linkFill;
    [SerializeField] private TextMeshProUGUI _linkPercentageText; // Add reference to TextMeshProUGUI
    [SerializeField] private Animator _linkHudAnimator;
    public Animator LinkHudAnimator => _linkHudAnimator;

    [SerializeField] public bool _hudInMiddleOfScreen; // maybe convert to enum with hud position type to make this value private
    [SerializeField] private GameObject _lowLinkOverlayGO;
    [SerializeField] private RawImage _lowLinkOverlayImage;
    private Vector3 _hudOriginalLocalPosition;
    private Vector3 _hudOriginalScale;
    [SerializeField] private float _transitionSpeed = 5f;
    [SerializeField] private float fadeDuration;
    private float _originalLinkValue;

    [Header("Hud Animations")]
    [SerializeField] private Animator _camAnimator;

    [Header("Hud Audio")]
    [SerializeField] private AudioSource _linkHudAudioSource;
    [SerializeField] private AudioClip _linkPulseAC;


    [Header("Read Only Flags")]
    [SerializeField] private bool _isLinkConnected = true;

    [SerializeField] private bool _isLinkCompromised = false;
    public bool IsLinkCompromised { get => _isLinkCompromised; set => _isLinkCompromised = value; }

    [SerializeField] private bool _isPaused = false;
    public bool IsPaused { get => _isPaused; set => _isPaused = value; }

    [SerializeField] private bool _isBreachRoomsAvailable = false;
    //[SerializeField] private DeathMenuManager _deathManagerRef;

    private void OnEnable()
    {
        EventManager.OnInitializeLink += OnInitializeLink;
    }
    private void Start()
    {
        // Store the original local position and scale
        _hudOriginalLocalPosition = _linkGroup.transform.localPosition;
        _hudOriginalScale = _linkGroup.transform.localScale;
        //InitializeLinkIntegrity();
    }
    private void Update()
    {
        if (_isLinkConnected && _isLinkCompromised)
            LowerConnection();

        if (_currentIntegrity <= _lowLinkOverlayThreshold)
        {
            _lowLinkOverlayGO.SetActive(true);
            StartCoroutine(FadeInLowLinkOverlay());
        }
        else
        {
            _lowLinkOverlayGO.SetActive(false);
            ResetLowLinkOverlayAlpha();
        }
    }
    private void OnDisable()
    {
        EventManager.OnInitializeLink -= OnInitializeLink;
    }

    private void InitializeLinkIntegrity()
    {
        _currentIntegrity = _maxIntegrity;
        _isLinkConnected = true;

        _integrityThreshold = _maxIntegrity / _integrityThresholdDivider;
    }

    /*public void HoveringCheckpointButton() // what was this used for?
    {
        _originalLinkValue = _currentIntegrity;
        _currentIntegrity = _currentIntegrity - 76.5f; // ask robby what is this value?
        UpdateLinkHUD(_maxIntegrity, _currentIntegrity);
    }
    public void HoveringEndButton()
    {
        _originalLinkValue = _currentIntegrity;
        _currentIntegrity = _maxIntegrity;
        UpdateLinkHUD(_maxIntegrity, _currentIntegrity);
    }
    public void ReturnLinkDisplay()
    {
        _isLinkCompromised = true;
        _currentIntegrity = _originalLinkValue;
        _isLinkCompromised = false;
        UpdateLinkHUD(_maxIntegrity, _currentIntegrity);
    }*/

    private void MoveHudToOGPosition()
    {
        // Move and scale smoothly back to the original position
        _linkGroup.transform.localPosition = Vector3.Lerp(
            _linkGroup.transform.localPosition,
            _hudOriginalLocalPosition,
            Time.deltaTime * _transitionSpeed
            );

        _camAnimator.SetBool("FocusOnLink", false);
        _linkGroup.transform.localScale = Vector3.Lerp(
            _linkGroup.transform.localScale,
            _hudOriginalScale,
            Time.deltaTime * _transitionSpeed
            );
    }
    private void MoveHudToMiddle()
    {
        // Move and scale smoothly to the center
        _linkGroup.transform.localPosition = Vector3.Lerp(
            _linkGroup.transform.localPosition,
            Vector3.zero,
            Time.deltaTime * _transitionSpeed
            );
        _camAnimator.SetBool("FocusOnLink", true);
        _linkGroup.transform.localScale = Vector3.Lerp(
            _linkGroup.transform.localScale,
            new Vector3(2.5f, 2.5f, 2.5f),
            Time.deltaTime * _transitionSpeed
            );
    }
    private void LowerConnection()
    {
        if (_isPaused) return;

        _currentIntegrity -= _integrityFailureFactor * Time.deltaTime;
        _currentIntegrity = Mathf.Clamp(_currentIntegrity, 0, _maxIntegrity);
        UpdateLinkHUD(_maxIntegrity, _currentIntegrity);

        int currentPercentage = Mathf.FloorToInt((_currentIntegrity / _maxIntegrity) * 100);
        if (currentPercentage % 10 == 0 && currentPercentage != lastPercentage)
        {
            lastPercentage = currentPercentage;
            StartCoroutine(LinkHudPulse());

            Debug.Log($"Link pulse triggered at {currentPercentage}%");
        }

        if (_currentIntegrity <= 0)
        {
            _isLinkConnected = false;
            _isLinkCompromised = false;
            return;
            //Debug.Log("Link Disconnected");
        }

        if (_hudInMiddleOfScreen) MoveHudToMiddle();
        else MoveHudToOGPosition();

        // should be event - breach handling
        if (_currentIntegrity <= _breachLinkThreshold && !_isBreachRoomsAvailable)
        {
            foreach (GameObject go in _breachDoors)
            {
                if (go) go.SetActive(true);
                else break;
            }
            _isBreachRoomsAvailable = true;
        }
        else if (_currentIntegrity > _breachLinkThreshold && _isBreachRoomsAvailable)
        {
            foreach (GameObject go in _breachDoors)
            {
                if (go)
                {
                    go.SetActive(false);
                    foreach (GameObject breachspawners in go.GetComponentInChildren<BreachRoomManager>().breachSpawners)
                    {
                        breachspawners.SetActive(false);
                    }
                }
                else
                {
                    break;
                }
            }

            _isBreachRoomsAvailable = false;
        }
    }

    public IEnumerator LinkHudPulse() // could change to happen on event
    {
        _linkHudAnimator.SetTrigger("Pulse");
        _linkHudAudioSource.clip = _linkPulseAC;
        _linkHudAudioSource.Play();
        yield return new WaitForSeconds(1);
        _linkHudAnimator.ResetTrigger("Pulse");
    }

    private void UpdateLinkHUD(int maxIntegrity, float currentIntegrity)
    {
        float fillAmount = currentIntegrity / maxIntegrity;
        _linkFill.fillAmount = fillAmount;

        // Update the TextMeshProUGUI text with the percentage
        _linkPercentageText.text = $"{fillAmount * 100f:F1}%";

        // Change the color of the fill based on the integrity percentage
        _linkFill.color = Color.Lerp(_endColor, _startColor, fillAmount);
        _linkBg.color = Color.Lerp(_endColor, _startColor, fillAmount);
        _linkPulse.color = Color.Lerp(_endColor, _startColor, fillAmount);
        _linkPercentageText.color = Color.Lerp(_endColor, _startColor, fillAmount);
    }

    public void MendLinkIntegrity(float amount) // could turn to event?
    {
        if (_currentIntegrity < _maxIntegrity)
            _currentIntegrity += amount;
        else
            _currentIntegrity = _maxIntegrity;
    }

    public bool IsLinkStable()
    {
        return _currentIntegrity > _integrityThreshold;
    }

    private IEnumerator FadeInLowLinkOverlay()
    {
        float elapsedTime = 0f;
        Color imageColor = _lowLinkOverlayImage.color;

        // Ensure the overlay is active before starting the fade
        while (_lowLinkOverlayGO.activeInHierarchy)
        {
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Calculate the new alpha, but cap it at 0.2 (20% opacity)
            float newAlpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            imageColor.a = Mathf.Min(newAlpha, 0.2f); // Limit to 20% opacity

            // Set the new color to the image
            _lowLinkOverlayImage.color = imageColor;

            // Optionally wait for the next frame
            yield return null;
        }
    }
    private void ResetLowLinkOverlayAlpha()
    {
        Color imageColor = _lowLinkOverlayImage.color;
        imageColor.a = 0; // Reset the alpha to 0
        _lowLinkOverlayImage.color = imageColor; // Update the image color
    }

    private void OnInitializeLink()
    {
        InitializeLinkIntegrity();
    }

    public void OnPauseGame(bool shouldPause)
    {
        if (shouldPause)
            _isPaused = false;
        else
            _isPaused = true;
    }
    public void SaveData(ref Profile data)
    {
        data.MaxIntegrity = _maxIntegrity;
        data.CurrentIntegrity = _currentIntegrity;
        UpdateLinkHUD(_maxIntegrity, _currentIntegrity);
    }
    public void LoadData(Profile data)
    {
        _maxIntegrity = data.MaxIntegrity;
        _currentIntegrity = data.CurrentIntegrity;
        UpdateLinkHUD(_maxIntegrity, _currentIntegrity);
    }
}
