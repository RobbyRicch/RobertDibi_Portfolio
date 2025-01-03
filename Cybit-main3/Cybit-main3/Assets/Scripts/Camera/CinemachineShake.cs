using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CinemachineShake : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _vCam;
    [SerializeField] private float _shakeIntensity = 1.0f;
    [SerializeField] private float _shakeTime = 0.2f;

    private CinemachineBasicMultiChannelPerlin _perlinNoise;
    private IEnumerator _shakeCamera;

    private void OnEnable()
    {
        EventManager.OnCameraShake += OnCameraShake;
        _perlinNoise = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void Start()
    {
        StopShaking();
    }
    private void OnDisable()
    {
        EventManager.OnCameraShake -= OnCameraShake;
    }
    private void StopShaking()
    {
        _perlinNoise.m_AmplitudeGain = 0.0f;

        if (_shakeCamera != null)
            StopCoroutine(_shakeCamera);
    }
    private IEnumerator ShakeCamera()
    {
        yield return new WaitForSecondsRealtime(_shakeTime);
        StopShaking();
        _shakeCamera = null;
    }
    private IEnumerator ShakeCamera(float shakeTime)
    {
        yield return new WaitForSecondsRealtime(shakeTime);
        StopShaking();
        _shakeCamera = null;
    }
    public void StartCameraShake()
    {
        _perlinNoise.m_AmplitudeGain = _shakeIntensity;
        _shakeCamera = ShakeCamera();
        StartCoroutine(_shakeCamera);
    }
    public void StartCameraShake(float shakeTime, float shakeIntensity)
    {
        _perlinNoise.m_AmplitudeGain = shakeIntensity;
        _shakeCamera = ShakeCamera(shakeTime);
        StartCoroutine(_shakeCamera);
    }

    private void OnCameraShake()
    {
        StartCameraShake();
    }
}
