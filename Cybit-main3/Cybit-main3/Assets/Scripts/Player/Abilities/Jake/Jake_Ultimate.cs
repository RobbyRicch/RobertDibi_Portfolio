using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jake_Ultimate : UltimateBase
{
    [Header("Components")]
    [SerializeField] private Player_Animations _animations;
    [SerializeField] private Transform _ultOriginTr;

    [SerializeField] private ProjectileUlt _ultProjectile;
    public ProjectileUlt UltProjectile { get => _ultProjectile; set => _ultProjectile = value; }

    [Header("Data")]
    [SerializeField] private float _ultFireDelay = 0.25f;

    [Header("VFXs")]
    [SerializeField] private GameObject _ultKeyRef;
    [SerializeField] private GameObject _ultActiveVFX;

    private void Start()
    {
        _isUltStateActive = false;
        StartCoroutine(RefillUltRoutine());
    }

    private IEnumerator ActivateUltimate()
    {
        if (_isUltStateActive) yield break; // make sure this coroutine cannot run if player shouldn't attack

        _isUltStateActive = true;
        _ultActiveVFX.SetActive(true);
        _ultKeyRef.SetActive(true);
        yield return new WaitForSeconds(_activationTime);

        _canUlt = true;
        StartCoroutine(HandleUltState());
        yield break;
    }
    private IEnumerator FireUltimate()
    {
        if (!_isUltStateActive || !_canUlt ||  _currentUltCharge < _ultChargeCost) yield break; // make sure this coroutine cannot run if player shouldn't attack

        _animator.SetTrigger("DoUltimate");
        _currentUltCharge -= _ultChargeCost;

        _ultPitch = Random.Range(_ultPitchRange.x, _ultPitchRange.y);
        _ultimateAudioSource.clip = _ultSlashAC;
        _ultimateAudioSource.pitch = _ultPitch;
        _ultimateAudioSource.PlayOneShot(_ultimateAudioSource.clip);
        _animations.DoBloom(_ultBloomIntensity, _ultBloomDuration, _ultBloomCurve);
        yield return new WaitForSecondsRealtime(_ultFireDelay);

        Vector2 direction = _ultOriginTr.right;
        ProjectileUlt ultProjectile = Instantiate(_ultProjectile, _ultOriginTr.position, Quaternion.identity);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        ultProjectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        ultProjectile.SetStats(_ultDamage, _ultSpeed, direction);
        yield break;
    }
    private IEnumerator UltimateRecovery()
    {
        if (!_isUltStateActive) yield break; // make sure this coroutine cannot run if player shouldn't attack

        _canUlt = false;
        _ultActiveVFX.SetActive(false);
        _ultKeyRef.SetActive(false);
        yield return new WaitForSeconds(_recoveryTime);

        _isUltStateActive = false;
        StartCoroutine(RefillUltRoutine());
        yield break;
    }

    protected override void ResetUltimateState()
    {
        StartCoroutine(UltimateRecovery());
    }

    public override void UseUltimate()
    {
        StartCoroutine(FireUltimate());
    }
    public override void ActivateUltimateState()
    {
        base.ActivateUltimateState();
        StartCoroutine(ActivateUltimate());
    }
}
