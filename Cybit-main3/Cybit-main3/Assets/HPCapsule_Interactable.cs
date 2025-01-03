using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HPCapsule_Interactable : MonoBehaviour
{


    [Header("Canister Base")]
    [SerializeField] public bool _isBroken;
    [SerializeField] public bool _hasBeenShot;
    [SerializeField] public GameObject _dropItem;

    [Header("Component Refs")]
    [SerializeField] private Animator _canisterAnimator;
    [SerializeField] private AudioSource _canisterAudioSource;
    [SerializeField] private AudioClip _canisterExplodeAC;
    [SerializeField] private Light2D _canisterLight;



    private void Update()
    {
        if (!_isBroken && _hasBeenShot)
        {
            ExplodeCanister();
        }
    }


    private void ExplodeCanister()
    {
        _isBroken = true;
        _canisterAnimator.SetTrigger("Explode");
        _canisterAudioSource.pitch = Random.Range(1, 1.4f);
        _canisterAudioSource.PlayOneShot(_canisterExplodeAC);
        _canisterLight.enabled = false;
        _dropItem.transform.SetParent(null);
        _dropItem.SetActive(true);
    }
}
