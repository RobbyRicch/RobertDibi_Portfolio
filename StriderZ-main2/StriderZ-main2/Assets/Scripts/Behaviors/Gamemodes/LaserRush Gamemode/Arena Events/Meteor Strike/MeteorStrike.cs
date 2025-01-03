using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorStrike : MonoBehaviour
{
    [SerializeField] private float _startDelay = 3f;
    [SerializeField] private float _endDelay = 3f;
    private float _timer = 0;
    [SerializeField] private SphereCollider _sphereCollider;
    private bool enabled = false;
    // Start is called before the first frame update
    void Start()
    {
        _sphereCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer > _startDelay && !enabled)
        {
            _sphereCollider.enabled = true;
            enabled = true;
        }
        if (_timer < _endDelay)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _sphereCollider.enabled = false;
        }
    }
}
