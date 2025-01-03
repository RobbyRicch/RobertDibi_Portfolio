using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFader : MonoBehaviour
{
    [SerializeField] private Image _faderImg;

    [SerializeField] private float _defaultFadeTime = 1.0f;
    public float DefaultFadeTime => _defaultFadeTime;

    public IEnumerator FadeInRoutine()
    {
        float time = 0;
        Color startColor = _faderImg.color;
        Color targetColor = _faderImg.color;
        targetColor.a = 1.0f;

        while (time < _defaultFadeTime)
        {
            _faderImg.color = Color.Lerp(startColor, targetColor, time / _defaultFadeTime);
            time += Time.deltaTime;
            Debug.Log($"FadeInRoutine - time: {time}, Time.deltaTime: {Time.deltaTime}, {_faderImg.color.a}");
            yield return null;
        }
        _faderImg.color = targetColor;

    }
    public IEnumerator FadeOutRoutine()
    {
        float time = 0;
        Color startColor = _faderImg.color;
        Color targetColor = _faderImg.color;
        targetColor.a = 0f;

        while (time < _defaultFadeTime)
        {
            _faderImg.color = Color.Lerp(startColor, targetColor, time / _defaultFadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        _faderImg.color = targetColor;
        gameObject.SetActive(false);
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeInRoutine());
    }
    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public IEnumerator FadeInRoutine(float fadeTime)
    {
        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        float time = 0;
        Color startColor = _faderImg.color;
        Color targetColor = _faderImg.color;
        targetColor.a = 1.0f;

        while (time < fadeTime)
        {
            _faderImg.color = Color.Lerp(startColor, targetColor, time / fadeTime);
            time += Time.deltaTime;
            //Debug.Log($"FadeInRoutine - time: {time}, Time.deltaTime: {Time.deltaTime}, {_faderImg.color.a}");
            yield return null;
        }
        _faderImg.color = targetColor;
    }
    public IEnumerator FadeOutRoutine(float fadeTime)
    {
        float time = 0;
        Color startColor = _faderImg.color;
        Color targetColor = _faderImg.color;
        targetColor.a = 0f;

        while (time < fadeTime)
        {
            _faderImg.color = Color.Lerp(startColor, targetColor, time / fadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        _faderImg.color = targetColor;
        gameObject.SetActive(false);
    }

    public void FadeIn(float fadeTime)
    {
        StartCoroutine(FadeInRoutine(fadeTime));
    }
    public void FadeOut(float fadeTime)
    {
        StartCoroutine(FadeOutRoutine(fadeTime));
    }

    public IEnumerator StartRunRoutine()
    {
        yield return StartCoroutine(FadeOutRoutine());
        yield return StartCoroutine(FadeInRoutine());
    }

    // IPersistable
    public void OnStartRun()
    {
        StartCoroutine(StartRunRoutine());
    }
}
