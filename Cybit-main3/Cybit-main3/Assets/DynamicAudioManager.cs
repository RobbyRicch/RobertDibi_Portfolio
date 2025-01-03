using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicAudioManager : MonoBehaviour
{
    [Header("Audio Refrences")]
    [SerializeField] public AudioSource _backgroundMusicAudioSource;

    [SerializeField] public float _originalVolume;
    [SerializeField] public float _targetVolume;
    [SerializeField] public float _fadeOutSpeed;
    [SerializeField] public float _fadeInSpeed;
    [SerializeField] public float _fadeSpeed;

    [Header("Level Specific")]
    [SerializeField] private string _levelName;
    [SerializeField] public AudioClip _calmPhase;
    [SerializeField] public AudioClip _fightPhase;
    [SerializeField] public AudioClip _heavyPhase;

    private void Start()
    {
        // Initialize audio source volume
        if (_backgroundMusicAudioSource != null)
        {
            _backgroundMusicAudioSource.volume = _originalVolume;
        }
    }

    public IEnumerator FadeInMusicNormal(float timeToFadeIn)
    {
        if (_backgroundMusicAudioSource == null) yield break;

        float startVolume = 0f;
        _backgroundMusicAudioSource.volume = startVolume;
        _backgroundMusicAudioSource.Play();

        while (_backgroundMusicAudioSource.volume < _originalVolume)
        {
            _backgroundMusicAudioSource.volume += timeToFadeIn * Time.unscaledDeltaTime;
            yield return null;
        }
        _backgroundMusicAudioSource.volume = _originalVolume;
    }
    public IEnumerator LowerVolume(float targetVolume)
    {
        if (_backgroundMusicAudioSource == null) yield break;

        float startVolume = _backgroundMusicAudioSource.volume;

        while (_backgroundMusicAudioSource.volume > targetVolume)
        {
            _backgroundMusicAudioSource.volume -= _fadeSpeed * Time.deltaTime;
            if (_backgroundMusicAudioSource.volume < targetVolume)
            {
                _backgroundMusicAudioSource.volume = targetVolume;
            }
            yield return null;
        }
    }

    public IEnumerator ResetVolume(float targetVolume)
    {
        if (_backgroundMusicAudioSource == null) yield break;

        float startVolume = _backgroundMusicAudioSource.volume;

        while (_backgroundMusicAudioSource.volume < targetVolume)
        {
            _backgroundMusicAudioSource.volume += _fadeSpeed * Time.deltaTime;
            if (_backgroundMusicAudioSource.volume > targetVolume)
            {
                _backgroundMusicAudioSource.volume = targetVolume;
            }
            yield return null;
        }
    }

    public IEnumerator FadeOutMusicNormal(float timeToFadeOut)
    {
        if (_backgroundMusicAudioSource == null) yield break;

        float startVolume = _backgroundMusicAudioSource.volume;

        while (_backgroundMusicAudioSource.volume > 0)
        {
            _backgroundMusicAudioSource.volume -= timeToFadeOut * Time.unscaledDeltaTime;
            yield return null;
        }
        _backgroundMusicAudioSource.volume = 0;
        _backgroundMusicAudioSource.Stop(); 
    }

    public void SwitchMusicPhaseInstantly(AudioClip phaseClip)
    {
        if (_backgroundMusicAudioSource == null || phaseClip == null)
        {
            Debug.LogWarning("AudioSource or AudioClip is null!");
            return;
        }

        _backgroundMusicAudioSource.Stop();
        _backgroundMusicAudioSource.clip = phaseClip;
        _backgroundMusicAudioSource.Play();
    }

    public IEnumerator StopMusicInstantly()
    {
        if (_backgroundMusicAudioSource == null) yield break;

        _backgroundMusicAudioSource.Stop();

    }
}
