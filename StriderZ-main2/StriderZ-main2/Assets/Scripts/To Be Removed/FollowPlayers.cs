using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayers : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _smoothTime = 0.5f;

    [Header("Zoom")]
    [SerializeField] private float _zoomLimiter = 50.0f;
    [SerializeField] private float _minZoom = 40.0f, _maxZoom = 10.9f;

    [Header("General")]
    [SerializeField] private float _greatestPlayersDistance;
    [SerializeField] private bool _isStopped = false;

    private List<Transform> _targets = new List<Transform>();
    private Vector3 _velocity;

    private void Start()
    {
        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        for (int i = 0; i < allPlayersAlive.Count; i++)
        {
            if (!_targets.Contains(allPlayersAlive[i].transform))
                _targets.Add(allPlayersAlive[i].transform);
        }
    }

    private void Update()
    {
        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        if (!_isStopped && _targets.Count != allPlayersAlive.Count)
        {
            _targets.Clear();
            for (int i = 0; i < allPlayersAlive.Count; i++)
            {
                if (!_targets.Contains(allPlayersAlive[i].transform))
                    _targets.Add(allPlayersAlive[i].transform);
            }
        }
    }
    private void LateUpdate()
    {
        if (!_isStopped && _targets.Count == 0)
            return;

        Move();
    }

    private Vector3 GetCenterPoint()
    {
        if (_targets.Count == 1)
            return _targets[0].position;

        Bounds bounds = new Bounds(_targets[0].position, Vector3.zero);

        for (int i = 0; i < _targets.Count; i++)
            bounds.Encapsulate(_targets[i].position);

        _greatestPlayersDistance = bounds.size.z;
        return bounds.center;
    }
    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPos = centerPoint + _offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref _velocity, _smoothTime);
    }
    private void Zoom()
    {
        float newZoom = Mathf.Lerp(_maxZoom, _minZoom, _greatestPlayersDistance / _zoomLimiter);
        // apply zoom ;
    }
}
