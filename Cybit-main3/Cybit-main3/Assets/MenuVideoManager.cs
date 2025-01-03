using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MenuVideoManager : MonoBehaviour
{
    [Header("Intro - Logo Switch")]
    [SerializeField] private VideoPlayer _vpRef;            
    [SerializeField] private bool _introShouldSwitch;       
    [SerializeField] private bool _introPlaying;            

    [Header("Loop Background")]
    [SerializeField] private VideoClip _loopedBackgroundVid;
    [SerializeField] private bool _loopIsPlaying;           
    
    [Header("Extras")]
    [SerializeField] private GameObject _thingsToActivate;
    [SerializeField] private float _timeToActivateButtons;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    private void OnEnable()
    {
        // Initialize states
        _introPlaying = true;
        _loopIsPlaying = false;
        _thingsToActivate.SetActive(false);

        StartCoroutine(ActivateButtons());
        _vpRef.loopPointReached += HandleVideoFinished;
    }

    private IEnumerator ActivateButtons()
    {
        yield return new WaitForSeconds(_timeToActivateButtons);
        _thingsToActivate.SetActive(true);
    }

    private void OnDisable()
    {
        // Unsubscribe from the event
        _vpRef.loopPointReached -= HandleVideoFinished;
    }

    // This method will be called when the video finishes
    private void HandleVideoFinished(VideoPlayer vp)
    {
        if (_introPlaying)
        {
            // Logic when the intro video finishes
            _introPlaying = false;
            _loopIsPlaying = true;

            // Switch to the looped background video
            _vpRef.clip = _loopedBackgroundVid;
            _vpRef.isLooping = true;
            _vpRef.Play();

        }
    }
}
