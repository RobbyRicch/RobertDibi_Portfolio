using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private float _maxTimer = 1;
    private float _timer;

    private Image _pointerImage;
    private Color _transparentColor;

    private RectTransform _rectTransform;

    public Transform Target;
    private Transform _player;

    private Action unRegister;

    private Quaternion _tRot = Quaternion.identity;
    private Vector3 _tPos;

    private bool _isRegistered;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _pointerImage = GetComponentInChildren<Image>();
        _transparentColor = _pointerImage.color;
        _transparentColor.a = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isRegistered)
        {
            RotateToTarget();
            Countdown();
        }
    }


    public void Register(Transform target, Transform player, Action unregister)
    {
        Target = target;
        _player = player;
        unRegister = unregister;

        _isRegistered = true;
    }

    //public void RestartIndicator()
    //{
    //    _timer = 0;
    //    Color tempColor = _transparentColor;
    //    tempColor.a = 1;
    //    _pointerImage.color = tempColor;
    //}

    private void RotateToTarget()
    {
        // Caching Pos & Rot
        if (gameObject.activeInHierarchy && Target)
        {
            _tPos = Target.position;
            _tRot = Target.rotation;
        }

        // Rotation Calculations
        Vector3 direction = _player.position - _tPos;
        _tRot = Quaternion.LookRotation(direction);
        _tRot.z = -_tRot.y;
        _tRot.x = 0;
        _tRot.y = 0;
        Vector3 northDirection = new Vector3(0, 0, _player.eulerAngles.y);

        // Setting the Rotation
        _rectTransform.localRotation = _tRot * Quaternion.Euler(northDirection);
    }

    private void Countdown()
    {
        if (gameObject.activeInHierarchy)
        {
            _timer += Time.deltaTime;
            if (_timer >= _maxTimer)
            {
                unRegister();
                Destroy(gameObject);
            }
            else if (_timer >= _maxTimer / 10 * 8)
            {
                _pointerImage.CrossFadeColor(_transparentColor, _maxTimer / 10 * 2, false, true);
            }
        }
    }
}
