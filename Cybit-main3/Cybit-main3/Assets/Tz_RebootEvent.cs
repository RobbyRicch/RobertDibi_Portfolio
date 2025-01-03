using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tz_RebootEvent : MonoBehaviour
{
    [Header("GOAL : Trigger reboot cutscene after dialogue finishes")]
    [Header("Refrences")]
    [SerializeField] public DialogueManager _targetDialogue;
    [SerializeField] public GameObject _targetVideo;
    [SerializeField] public RawImage _rawImageRef;
    [SerializeField] public Sprite _standbyImage;
    [SerializeField] public GameObject[] _thingsToDisable;
    [SerializeField] public Player_Controller _playerRef;
    [SerializeField] public GameObject _cursorRef;
    [SerializeField] public GameObject _PlayerFirstDialogue;
    [SerializeField] public GameObject _annEntryDialogue;
    [SerializeField] public Tz_PhaseManager _phaseOneScript;
    [SerializeField] AudioSource _bgmAudio;
    [SerializeField] AudioSource _wakeUpAudioSource;
    [SerializeField] AudioClip _wakeUpAC;

    [Header("Bool Checks")]
    [SerializeField] private bool _shouldPlayVideo;

    [Header("Settings")]
    [SerializeField] public float _timeToSwitchToImage;
    [SerializeField] public float _timeToAllowPlayerMovement;
    [SerializeField] public bool _playerCanInput;
    [SerializeField] public bool _videoHasPlayed;
    [SerializeField] public Animator _cinemachineAnimatorRef;

    private void Start()
    {
        StartCoroutine(WaitForCanvas(2));
        StartCoroutine(UpdateVideoBool(2));
    }
    void Update()
    {

        if (!_targetDialogue._isDialogueRunning && !_videoHasPlayed && _shouldPlayVideo)
        {
            _targetVideo.SetActive(true);
            _targetDialogue.ManualCanvasExitNormal();
            StartCoroutine(SwitchVideoToImage(_timeToSwitchToImage));
        }

        if (_rawImageRef.texture == _standbyImage.texture && Input.anyKeyDown && _playerCanInput)
        {
            foreach (GameObject objects in _thingsToDisable)
            {
                objects.SetActive(false);
            }
            _wakeUpAudioSource.PlayOneShot(_wakeUpAC);
                StopAllCoroutines();
                _playerCanInput = false;
                _cinemachineAnimatorRef.SetTrigger("WakeUp");
            StartCoroutine(AllowPlayerToMove(_timeToAllowPlayerMovement));
        }
    }

    IEnumerator SwitchVideoToImage(float time)
    {
        yield return new WaitForSeconds(time);
        _videoHasPlayed = true;
        _rawImageRef.texture = _standbyImage.texture;
        _playerCanInput = true;

    }
    IEnumerator UpdateVideoBool(float time)
    {
        yield return new WaitForSeconds(time);
        _shouldPlayVideo = true;

    }
    IEnumerator WaitForCanvas(float time)
    {
        yield return new WaitForSeconds(time);
        _targetDialogue.ManualCanvasEnter();
        _annEntryDialogue.SetActive(true);
    }
    IEnumerator AllowPlayerToMove(float time)
    {
        yield return new WaitForSeconds(3);
        _PlayerFirstDialogue.SetActive(true);
        yield return new WaitForSeconds(time);
        _cursorRef.SetActive(true);
        _cinemachineAnimatorRef.ResetTrigger("WakeUp");
        Debug.Log("Reboot Seuqence Complete");
        _phaseOneScript.enabled = true;
        _bgmAudio.Play();
    }
}
