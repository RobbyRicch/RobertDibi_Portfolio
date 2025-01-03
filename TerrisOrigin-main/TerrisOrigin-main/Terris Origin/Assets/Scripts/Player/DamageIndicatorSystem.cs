using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicatorSystem : MonoBehaviour
{
    [SerializeField] private DamageIndicator _damageIndicatorPrefab;
    [SerializeField] private Camera _mainCam;
    [SerializeField] private Transform _player;
    [SerializeField] private GameObject DMG_Overlay;
    [SerializeField] private float DMG_OverlaySpeed = 3f;

    private Dictionary<Transform, DamageIndicator> _indicators = new Dictionary<Transform, DamageIndicator>();
    private IEnumerator OverlayStart;

    // Delegates
    public static Action<Transform> CreateIndicator = delegate { };
    public static Func<Transform, bool> CheckIfObjInSight;

    private void OnEnable()
    {
        CreateIndicator += Create;
        CheckIfObjInSight += InSight;
    }

    private void OnDisable()
    {
        CreateIndicator -= Create;
        CheckIfObjInSight -= InSight;
    }

    private void Create(Transform target)
    {
        DMG_Overlay.SetActive(true);
        OverlayStart = DMGOverlayFadeIn();
        StartCoroutine(OverlayStart);
        StartCoroutine(_mainCam.GetComponent<CameraShake>().Shake(0.15f, 0.1f));
        if (_indicators.ContainsKey(target))
        {
            //_indicators[target].RestartIndicator();
            return;
        }
        else
        {
            DamageIndicator newIndicator = Instantiate(_damageIndicatorPrefab, transform);
            newIndicator.Register(target, _player, new Action(() => { _indicators.Remove(target); }));

            _indicators.Add(target, newIndicator);
        }
        //StopCoroutine(OverlayOff);
    }

    private bool InSight(Transform t)
    {
        Vector3 screenPoint = _mainCam.WorldToViewportPoint(t.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
    IEnumerator DMGOverlayFadeIn()
    {
        //float numToLower = DMG_OverlayTime / 255;
        Color color = DMG_Overlay.GetComponent<RawImage>().color;
        while (color.a < 0.95f)
        {
            color.a = Mathf.Lerp(color.a, 1, Time.deltaTime * DMG_OverlaySpeed);
            DMG_Overlay.GetComponent<RawImage>().color = new Color(color.r, color.g, color.b, color.a);
            yield return null;
        }
        DMG_Overlay.GetComponent<RawImage>().color = new Color(color.r, color.g, color.b, 1);
        StartCoroutine(DMGOverlayFadeOut());
    }
    IEnumerator DMGOverlayFadeOut()
    {
        //float numToLower = DMG_OverlayTime / 255;
        Color color = DMG_Overlay.GetComponent<RawImage>().color;
        while (color.a > 0.05f)
        {
            color.a = Mathf.Lerp(color.a, 0, Time.deltaTime * DMG_OverlaySpeed);
            DMG_Overlay.GetComponent<RawImage>().color = new Color(color.r, color.g, color.b, color.a);
            yield return null;
        }
        DMG_Overlay.SetActive(false);
        DMG_Overlay.GetComponent<RawImage>().color = new Color(color.r, color.g, color.b, 0);
    }
}
