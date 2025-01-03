using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class InfoCanvasManager : MonoBehaviour
{
    [SerializeField] private int _id = 0;
    public int Id => _id;

    [Header("Refrences")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private GameObject _canBeClosedTextGO;
    [SerializeField] private Animator _canvasAnimator;
    [SerializeField] private GameObject _canvasGO;
    [SerializeField] private AudioSource _bgm;
    [SerializeField] private AudioSource _canvasAudioSource;
    [SerializeField] private AudioClip _canvasOpenSFX;
    [SerializeField] private GameObject _objectToActivate;
    public GameObject ObjectToActivate { get => _objectToActivate; set => _objectToActivate = value; }

    [Header("Targets")]
    [SerializeField] private VideoClip _targetClip;

    [SerializeField] private string _targetTitle;

    [TextArea(3, 10)]
    [SerializeField] private string _targetDesc;

    [SerializeField] private string _targetObjective;

    [Header("State")]
    [SerializeField] private bool _isActive;
    [SerializeField] private bool _canBeClosed;
    [SerializeField] private bool _shouldActivateSomething;

    [Header("Player")]
    [SerializeField] private Player_Controller _player;

    [Header("Audio")]
    [SerializeField] private AudioClip[] _annDialogues;

    [Header("Canvas Time")]
    [SerializeField] private float _timeForAllowClosing;
    public Player_Controller Player { get => _player; set => _player = value; }

    [SerializeField] private bool _isActiveOnStart = false;

    private void Start()
    {
        if (_isActiveOnStart)
            ApplyCanvas();
    }
    private void Update()
    {
        if (_isActive && Input.anyKeyDown)
        {
            _canBeClosed = true;
        }

        if (_isActive && Input.anyKeyDown && _canBeClosed)
        {
            CloseCanvas();
        }
    }
    public void ApplyCanvas()
    {
        int _randomVoiceLine = Random.Range(0, 2);
        _canvasAudioSource.PlayOneShot(_canvasOpenSFX);
        _player.IsMovementOnlyDisabled = true;
        _canvasAudioSource.clip = _annDialogues[_randomVoiceLine];
        _canvasAudioSource.Play();
        _player.IsInputDisabled = true;
        _canvasAnimator.SetTrigger("Open");
        _canBeClosedTextGO.SetActive(false);
        _videoPlayer.clip = _targetClip;
        _titleText.text = _targetTitle;
        _descText.text = _targetDesc;
        _isActive = true;
        _canvasGO.SetActive(true);
        _canvasAnimator.ResetTrigger("Close");
        StartCoroutine(AllowClosing());
    }

    public void CloseCanvas()
    {
        _canvasAnimator.SetTrigger("Close");
        _videoPlayer.Stop();
        _titleText.text = string.Empty;
        _descText.text = string.Empty;
        _isActive = false;
        _canvasGO.SetActive(false);
        _canvasAnimator.ResetTrigger("Open");
        _player.IsMovementOnlyDisabled = false;
        _player.IsInputDisabled = false;
        if (_shouldActivateSomething && _objectToActivate != null)
        {
            _objectToActivate.SetActive(true);
        }
    }

    private IEnumerator AllowClosing()
    {
        _canBeClosed = false;
        yield return new WaitForSeconds(_timeForAllowClosing);
        _canBeClosedTextGO.SetActive(true);
        _canBeClosed = true;

    }


}
