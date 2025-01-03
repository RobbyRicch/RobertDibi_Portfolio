using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill_TZ : MonoBehaviour
{

    [Header("Base Stuff")]
    [SerializeField] private float _spinSpeed;
    [SerializeField] private GameObject _bigCollider;
    [SerializeField] private GameObject _smallerCollider;

    private void OnEnable()
    {
        EventManager.OnFocus += OnFocus;
    }
    private void Start()
    {
        _spinSpeed = -800;
        _bigCollider.SetActive(true);
        _smallerCollider.SetActive(false);
    }
    void Update()
    {
        SpinQuick();
    }
    private void OnDisable()
    {
        EventManager.OnFocus -= OnFocus;
    }

    private void SpinQuick()
    {
        transform.Rotate(0, 0, _spinSpeed * Time.deltaTime);
    }
    private void OnFocus(bool isStarting)
    {
        if (isStarting)
        {
            _spinSpeed = -200;
            _bigCollider.SetActive(false);
            _smallerCollider.SetActive(true);
        }
        else
        {
            _spinSpeed = -800;
            _bigCollider.SetActive(true);
            _smallerCollider.SetActive(false);
        }
    }
}
