using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingGate : MonoBehaviour
{
    [SerializeField] private float _gateTimer = 0.5f;
    [SerializeField] private MeshRenderer _gateMesh;

    private Color _color;
    private Vector3 _startingPos = Vector3.zero, _targetPos = Vector3.zero;

    private void Awake()
    {
        _startingPos = transform.position;
        _targetPos = new(_startingPos.x, _startingPos.y - transform.localScale.y * 2.0f, _startingPos.z);
        //_color = _gateMesh.material.color;
    }
    private void OnEnable()
    {
        /*EventManager.OnRoundStart += OnRoundStart;
        EventManager.OnRoundEnd += OnRoundEnd;
        EventManager.OnRoundEndWithDraw += OnRoundEndWithDraw;*/
    }
    private void OnDisable()
    {
        /*EventManager.OnRoundStart -= OnRoundStart;
        EventManager.OnRoundEnd -= OnRoundEnd;
        EventManager.OnRoundEndWithDraw -= OnRoundEndWithDraw;*/
    }

    #region Events
    private void OnRoundStart()
    {
        //_gateMesh.enabled = false;
        transform.position = _targetPos;
        //_gateMesh.material.color = new Color(_color.r, _color.g, _color.b, _color.a / 2);
    }
    private void OnRoundEnd(PlayerInputHandler player)
    {
        //_gateMesh.enabled = true;
        transform.position = _startingPos;
        //_gateMesh.material.color = _color;
    }
    private void OnRoundEndWithDraw()
    {
        //_gateMesh.enabled = true;
        transform.position = _startingPos;
        //_gateMesh.material.color = _color;
    }
    #endregion
}
