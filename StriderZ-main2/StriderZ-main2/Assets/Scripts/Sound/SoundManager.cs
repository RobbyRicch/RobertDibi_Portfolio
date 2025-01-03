using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //An Instance of the Sound Manager
    public static SoundManager Instance;

    #region Sources
    [Header("Sources")] //Seperate Sources for Music & Effects
    [SerializeField] private AudioSource _anouncerSource;
    [SerializeField] private AudioSource _uiSource;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _hookSource, _stunSource, _phaseSource, _spikesSource, _collideSource, _playerSource,_pickUpSource,_mechJawsSource, _batsource;
    [SerializeField] private AudioSource _meteorSource;

    [Range(0, 0)][SerializeField] private float _maxVolume = 1.0f;
    private bool _isPlaying;
    public AudioSource AnouncerSource => _anouncerSource;
    public AudioSource UISource => _uiSource;
    public AudioSource MusicSource => _musicSource;
    public AudioSource HookSource => _hookSource;
    public AudioSource StunSource => _stunSource;
    public AudioSource PhaseSource => _phaseSource;
    public AudioSource SpikesSource => _spikesSource;
    public AudioSource PlayerSource => _playerSource;
    public AudioSource CollideSound => _collideSource;
    public AudioSource PickUpSource => _pickUpSource;
    public AudioSource BatSource => _batsource;
    public AudioSource MeteorSource => _meteorSource;
    public AudioSource MechaJawsSource => _mechJawsSource;
    #endregion

    #region Game Sounds
    [Header("Game Sounds")]
    [SerializeField] private AudioClip _victorySound;
    [SerializeField] private AudioClip _mainMenuMusic, _gameMusic;
    
    public AudioClip VictorySound => _victorySound;
    public AudioClip MainMenuMusic => _mainMenuMusic;
    public AudioClip GameMusic => _gameMusic;
    #endregion

    #region UI Sounds
    [Header("UI Sounds")]
    [SerializeField] private AudioClip _uiBtnPress;
    [SerializeField] private AudioClip _uiBtnBack;
    public AudioClip UIBtnPress => _uiBtnPress;
    public AudioClip UIBtnBack => _uiBtnBack;
    #endregion

    #region Player Sounds
    [Header("Player Sounds")]
    [SerializeField] private AudioClip _playerDeath;
    [SerializeField] private AudioClip _deathlineDeath;
    [SerializeField] private AudioClip _collideSound;
    [SerializeField] private AudioClip _hookUseSound, _hookHitSound, _hookReleaseSound;
    [SerializeField] private AudioClip _stunUseSound, _stunHitSounds;
    [SerializeField] private AudioClip _phaseUseSound, _phaseEndSound;
    [SerializeField] private AudioClip _batHitSound, _batMissSound;
    [SerializeField] private AudioClip _spikesUseSound, _spikesLandingSound, _spikeshitSound;
    [SerializeField] private AudioClip _pickUpSound, _mechaJawsSound;
    public AudioClip PlayerDeath => _playerDeath;
    public AudioClip DeathlineDeath => _deathlineDeath;
    public AudioClip HookUseSound => _hookUseSound;
    public AudioClip HookHitSound => _hookHitSound;
    public AudioClip HookReleaseSound => _hookReleaseSound;
    public AudioClip StunUseSound => _stunUseSound;
    public AudioClip StunHitSounds => _stunHitSounds;
    public AudioClip PhaseUseSound => _phaseUseSound;
    public AudioClip PhaseEndSound => _phaseEndSound;
    public AudioClip SpikesUseSound => _spikesUseSound;
    public AudioClip SpikesLandingSound => _spikesLandingSound;
    public AudioClip SpikeshitSound => _spikeshitSound;
    public AudioClip CollideHitSound => _collideSound;
    public AudioClip PickUpSound => _pickUpSound;
    public AudioClip MechaJawsSound => _mechaJawsSound;
    public AudioClip BatHitSound => _batHitSound;
    public AudioClip BatMissSound => _batMissSound;

    #endregion

    #region Announcer Sounds
    [Header("Announcer Sounds")]
    [SerializeField] private AudioClip _roundStartCountdown;
    [SerializeField] private AudioClip _go;
    [SerializeField] private float fadeTime = 1.5f;
    public AudioClip RoundStartCountdown => _roundStartCountdown;
    public AudioClip Go => _go;

    [SerializeField] private AudioClip[] _playerReady, _playerVictory, _playerEliminated, _playerSpawned;
    public AudioClip[] PlayerReady => _playerReady;
    public AudioClip[] PlayerVictory => _playerVictory;
    public AudioClip[] PlayerEliminated => _playerEliminated;
    public AudioClip[] PlayerSpawned => _playerSpawned;
    /* for each group of sound clips that varies depending on the player make an array like 'private AudioClip[] _playerReady;'.
     * in order to expose those arrays create a property like 'public AudioClip[] PlayerReady => _playerReady;'.
     */

    /* if a certain sound can or will be cut by another sound you should create a behavior for that, example:
     * 
     * private IEnumerator PlayAnnouncerWhenFree(AudioClip announcerClip)
     * {
     *     while (_anouncerSource.isPlaying)
     *         yield return null;
     * 
     *     PlayAnnouncerSound(announcerClip);
     * }
     * 
     * (there could be other cases, try to anticipate as much "end scenarios" as you can)
     */

    /* we should be able to save a float of _minVolume, _currentVolume, _tempVolume and _maxVolume to be able to keep tabs on the sounds
     * where '_tempVolume' is a temporary amount of sound used for a brief period of time
     * (like lowering a certain soundSource in order for another one to be heard better)
     */
    #endregion

    //To never destroy the manager!
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        EventManager.OnGameLaunched += OnGameLaunched;
    }
    private void Start()
    {
        _musicSource.volume = 0.5f;
    }
    private void OnDisable()
    {
        EventManager.OnGameLaunched -= OnGameLaunched;
    }

    //Plays the Sound
    public void PlayAnnouncerSound(AudioClip clip)
    {
        StartCoroutine(PlaySoundSynced(_anouncerSource, clip, true));
    }
    public void PlayUISound(AudioClip clip)
    {
        _uiSource.PlayOneShot(clip);
    }
    public void PlayMusicSound(AudioClip clip)
    {
        _musicSource.volume = 0.5f;

        _musicSource.PlayOneShot(clip);
    }
    public void PlayHookSound(AudioClip clip)
    {
        _hookSource.PlayOneShot(clip);
    }   
    public void PlayPickUpSound(AudioClip clip)
    {
        _pickUpSource.PlayOneShot(clip);
    }
    public void PlayMechaJaws(AudioClip clip)
    {
        _mechJawsSource.volume = 0.7f;
        _mechJawsSource.PlayOneShot(clip);
    }
    public void PlayStunSound(AudioClip clip)
    {
        _stunSource.PlayOneShot(clip);
    }
    public void PlayPhaseSound(AudioClip clip)
    {
        _phaseSource.volume = 0.6f;
        _phaseSource.PlayOneShot(clip);
    }
    public void PlaySpikesSound(AudioClip clip)
    {
        _spikesSource.PlayOneShot(clip);
    }  
    public void PlayCollideSound(AudioClip clip)
    {
        _collideSource.PlayOneShot(clip);
    }
    public void PlayBatSound(AudioClip clip)
    {
        _batsource.volume = 0.7f;
        _batsource.PlayOneShot(clip);
    }
    public void PlayPlayerSound(AudioClip clip, bool isOverriding, float duration)
    {
        StartCoroutine(PlaySoundSynced(_playerSource, clip, isOverriding));
    }

    //Controls Master Volume -> VolumeSlider Script
    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;

    }
    private IEnumerator PlaySoundSynced(AudioSource audioSource, AudioClip clip, bool isOverriding)
    {
        if (audioSource.isPlaying && !isOverriding)
            yield break;

        audioSource.Stop();
        _musicSource.volume = 0.5f;
        _musicSource.volume = _musicSource.volume / 2;
        audioSource.PlayOneShot(clip);

        yield return new WaitForSeconds(clip.length);

        _musicSource.volume = 0.5f;
    }
    private IEnumerator PlaySoundSynced(AudioClip clip, bool isOverriding, float duration)
    {
        if (_isPlaying && !isOverriding)
            yield break;

        _musicSource.volume = 0.5f;
        _musicSource.volume = _musicSource.volume / 2;
        _playerSource.PlayOneShot(clip);

        yield return new WaitForSeconds(duration);

        _musicSource.volume = 0.5f;
    }
    //Methods for toggling music/effects
    public void ToggleEffects()
    {
        _anouncerSource.mute = !_anouncerSource.mute;
        _uiSource.mute = !_uiSource.mute;
        _hookSource.mute = !_hookSource.mute;
        _stunSource.mute = !_stunSource.mute;
        _phaseSource.mute = !_phaseSource.mute;
        _spikesSource.mute = !_spikesSource.mute;
    } 
    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }

    private void OnGameLaunched()
    {
        //PlayMusicSound();
    }
}
