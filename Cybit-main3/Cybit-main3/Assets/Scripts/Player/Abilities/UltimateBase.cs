using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UltimateBase : MonoBehaviour
{
    [Header("Base Components")]
    [SerializeField] protected Animator _animator;
    [SerializeField] protected Animator _hudUltAnimator;
    [SerializeField] protected AnimationCurve _ultBloomCurve;

    [Header("Base Data")]
    [SerializeField] protected float _ultThreshold = 5.0f;
    [SerializeField] protected float _activationTime = 0.5f;
    [SerializeField] protected float _recoveryTime = 0.1f;
    [SerializeField] protected float _refillFactor = 1.0f;

    [SerializeField] protected float _ultBloomIntensity = 1.1f;
    [SerializeField] protected float _ultBloomDuration = 0.4f;

    [SerializeField] protected int _ultDamage = 200;
    public int UltDamage { get => _ultDamage; set => _ultDamage = value; }

    [SerializeField] protected float _ultSpeed = 1000.0f;
    public float UltSpeed { get => _ultSpeed; set => _ultSpeed = value; }

    [SerializeField] protected int _maxUltCharge = 90;
    public int MaxUltCharge { get => _maxUltCharge; set => _maxUltCharge = value; }

    [SerializeField] protected float _currentUltCharge = 0.0f;
    public float CurrentUltCharge { get => _currentUltCharge; set => _currentUltCharge = value; }

    [SerializeField] protected float _ultChargeCost = 30.0f;
    public float UltChargeCost { get => _ultChargeCost; set => _ultChargeCost = value; }

    protected bool _isUltStateActive;
    public bool IsUltStateActive { get => _isUltStateActive; set => _isUltStateActive = value; }

    [Header("Base Sound")]
    [SerializeField] protected AudioSource _ultimateAudioSource;
    [SerializeField] protected AudioSource _hudUltimateAudioSource;
    [SerializeField] protected AudioClip _ultSlashAC;
    [SerializeField] protected AudioClip _ultActivateAC, _ultPingAC;
    protected Vector2 _ultPitchRange = new(0.75f, 1.25f);
    protected float _ultPitch = 1.0f;

    protected bool _canUlt = true;
    public bool CanUlt { get => _canUlt; set => _canUlt = value; }

    protected IEnumerator RefillUltRoutine()
    {
        while (!_isUltStateActive && _currentUltCharge < _maxUltCharge)
        {
            _currentUltCharge += Time.unscaledDeltaTime * _refillFactor;
            yield return null;
        }

        _currentUltCharge = _maxUltCharge;
        if (_hudUltAnimator)
        {
            _hudUltAnimator.SetTrigger("UltFullTrigger");
            _hudUltimateAudioSource.clip = _ultPingAC;
            _hudUltimateAudioSource.Play();
        }
    }
    protected IEnumerator HandleUltState()
    {
        while (_isUltStateActive)
        {
            if (_currentUltCharge < _ultChargeCost)
            {
                _hudUltAnimator.ResetTrigger("UltFullTrigger");
                _hudUltAnimator.SetBool("IsUsingUlt", false);
                ResetUltimateState();
                yield break;
            }

            yield return null;
        }
    }
    protected virtual void ResetUltimateState() { }

    public void RefilUlt()
    {
        StartCoroutine(RefillUltRoutine());
    }

    public abstract void UseUltimate();
    public virtual void ActivateUltimateState()
    {
        _hudUltAnimator.SetBool("IsUsingUlt", true);
        _hudUltimateAudioSource.clip = _ultActivateAC;
        _hudUltimateAudioSource.Play();
    }
}
