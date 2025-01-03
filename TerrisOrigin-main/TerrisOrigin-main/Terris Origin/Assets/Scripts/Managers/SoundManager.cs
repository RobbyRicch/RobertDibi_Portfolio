using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Singlton
    public static SoundManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    


    public enum SoundType
    {
        //Sounds for Character
        Walk,
        Jump,
        Sprint,
        takeHit,
        lowHP,

        //Sounds for Lightning
        LightingBasic,
        LightningCharged,
        LightningDash,
        LightningWrath,

        //Sounds For Fire
        FireBasic,
        FireCharged,
        FireJump,
        FireAura,
    };
    #region Audio Sources
    [Header("Audio Sources")]
    [SerializeField] AudioSource stepAudioSource;
    [SerializeField] AudioSource characterAudioSource;
    [SerializeField] AudioSource abilitiesAudioSource;

    #endregion
    #region Character Sounds
    [Header("Character Sounds")]
    [SerializeField] AudioClip Walk;
    [SerializeField] AudioClip Jump;
    [SerializeField] AudioClip Sprint;
    [SerializeField] AudioClip takeHit;
    [SerializeField] AudioClip lowHP;

    #endregion
    #region Element Sounds
    #region Lightning
    [Header("Lightning Audio")]

    [SerializeField] AudioClip LightningBasic;
    [SerializeField] AudioClip LightningCharged;
    [SerializeField] AudioClip LightningDash;
    [SerializeField] AudioClip LightningWrath;
    #endregion
    #region Fire
    [Header("Fire Audio")]

    [SerializeField] AudioClip FireBasic;
    [SerializeField] AudioClip FireCharged;
    [SerializeField] AudioClip FireJump;
    [SerializeField] AudioClip FireAura;
    #endregion
    #endregion

    public void PlaySound(SoundType ST)
    {
        if (abilitiesAudioSource != null)
        {
            switch (ST)
            {
                #region Character
                case SoundType.Walk:
                    if (!isPlaying) // Check if AudioSource is already playing a sound
                    {
                        stepAudioSource.clip = Walk;
                        RandomPitch();
                        stepAudioSource.Play();
                        isPlaying = true;
                        

                        StartCoroutine(ResetIsPlayingCoroutine(stepAudioSource.clip.length));
                    }
                    break;

                case SoundType.Jump:
                    if (!isPlaying)
                    {
                        characterAudioSource.clip = Jump;
                        RandomPitch();
                        characterAudioSource.Play();
                        
                        isPlaying = true;
                        

                        StartCoroutine(ResetIsPlayingCoroutine(1f));
                        
                    }
                    break;


                case SoundType.Sprint:
                    break;

                case SoundType.takeHit:
                    if (!isPlaying)
                    {
                        characterAudioSource.clip = takeHit;
                        RandomPitch();
                        characterAudioSource.PlayOneShot(takeHit);

                        isPlaying = true;
                        

                        StartCoroutine(ResetIsPlayingCoroutine(1f));

                    }
                    break;

                case SoundType.lowHP:
                    RandomPitch();
                    break;

                    #endregion
                    #region Lightning Sounds                  
                case SoundType.LightingBasic:
                    RandomPitch();
                    if (AbilitiesSelection.Instance.RightHandElement == AbilitiesSelection.ElementType.Lightning)
                    {
                    abilitiesAudioSource.PlayOneShot(LightningBasic, 0.1f);

                    }
                    break;
                case SoundType.LightningCharged:
                    RandomPitch();
                    abilitiesAudioSource.PlayOneShot(LightningBasic, 0.1f);
                    break;
                case SoundType.LightningDash:
                    RandomPitch();
                    abilitiesAudioSource.PlayOneShot(LightningDash, 0.2f);
                    break;
                case SoundType.LightningWrath:
                    RandomPitch();
                    abilitiesAudioSource.PlayOneShot(LightningWrath, 0.3f);
                    break;

                    
                #endregion

                #region Fire Sounds
                case SoundType.FireBasic:
                    RandomPitch();
                    if (AbilitiesSelection.Instance.RightHandElement == AbilitiesSelection.ElementType.Fire)
                    { 
                        abilitiesAudioSource.PlayOneShot(FireBasic, 0.25f);
                    
                    }
                    break;
                case SoundType.FireCharged:
                    RandomPitch();
                    abilitiesAudioSource.PlayOneShot(FireCharged, 0.1f);
                    break;
                case SoundType.FireJump:
                    RandomPitch();
                    abilitiesAudioSource.PlayOneShot(FireJump, 0.4f);
                    break;
                case SoundType.FireAura:
                    RandomPitch();
                    abilitiesAudioSource.PlayOneShot(FireAura, 0.6f);
                    break;
                    #endregion

            }
        }
    }

    private void RandomPitch()
    {
        float rand = Random.Range(0.8f, 1.2f);
        if (abilitiesAudioSource != null) abilitiesAudioSource.pitch = rand;
    }
    bool isPlaying;
    // Call this method to reset the "isPlaying" flag when the sound has finished playing
    IEnumerator ResetIsPlayingCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        isPlaying = false;
    }
}
