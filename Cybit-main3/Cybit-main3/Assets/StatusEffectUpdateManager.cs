using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusEffectUpdateManager : MonoBehaviour
{
    [Header("REQUIRED Refrences")]
    [SerializeField] private TextMeshProUGUI _statusEffectText;
    [SerializeField] private Animator _statusEffectTextAnimator;
    [SerializeField] private AudioSource _statusEffectAS;

    [Header("Settings")]
    [SerializeField] private Color _positiveColor;
    [SerializeField] private Color _negativeColor;

    private void OnEnable()
    {
        EventManager.OnUpdateStatusEffect += OnUpdateStatusEffect;
    }

    private void OnDisable()
    {
        EventManager.OnUpdateStatusEffect -= OnUpdateStatusEffect;

    }

    private void OnUpdateStatusEffect(bool isPositiveEffect, string statusText , AudioClip statusRelatedClip)
    {

        if (isPositiveEffect)
        {
            _statusEffectText.color = _positiveColor; 
        }
        else
        {
            _statusEffectText.color = _negativeColor; 
        }

        _statusEffectText.text = statusText;
        _statusEffectTextAnimator.SetTrigger("DisplayStatusUpdate");
        _statusEffectAS.clip = statusRelatedClip;
        _statusEffectAS.PlayOneShot(_statusEffectAS.clip);
        StartCoroutine(ResetTrigger());

    }

    private IEnumerator ResetTrigger()
    {
        yield return new WaitForSeconds(1);
        _statusEffectTextAnimator.ResetTrigger("DisplayStatusUpdate");

    }
}
