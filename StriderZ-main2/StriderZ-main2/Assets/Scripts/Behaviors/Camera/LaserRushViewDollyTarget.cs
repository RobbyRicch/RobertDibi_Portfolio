using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LaserRushViewDollyTarget : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _viewCam, _trackCam;
    [SerializeField] private GameObject _ViewDollyTrack;
    [Range(0.0f, 1.0f)][SerializeField] private float _speed = 0.5f;
    [SerializeField] private float _nextWaypointDistanceThreshold = 2.0f;
    [SerializeField] private bool _isDollyFinished = false;
    [SerializeField] private bool _isDebugMassagesOn = false;

    private CinemachineTrackedDolly _trackedDolly;

    private void Start()
    {
        _trackedDolly = _viewCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        _trackedDolly.m_PathPosition = _trackedDolly.m_Path.MinUnit(_trackedDolly.m_PositionUnits);
    }
    private void Update()
    {
        if (_isDollyFinished)
            return;

        HandleWaypoint();
    }

    private void MoveToWaypoint()
    {
        _trackedDolly.m_PathPosition = Mathf.Lerp(_trackedDolly.m_PathPosition, _trackedDolly.m_Path.MaxUnit(_trackedDolly.m_PositionUnits), _speed * Time.deltaTime);
    }
    private void HandleWaypoint()
    {
        MoveToWaypoint();

        if (_trackedDolly.m_PathPosition + _nextWaypointDistanceThreshold >= _trackedDolly.m_Path.MaxUnit(_trackedDolly.m_PositionUnits))
        {
            _isDollyFinished = true;
            SwitchCameraWhenFinishedDolly();
        }
    }
    private void SwitchCameraWhenFinishedDolly()
    {
        if (CinemachineManager.Instance)
        {
            CinemachineManager.Instance.SwitchVirtualCameras((int)LaserRushVCType.Spawn);
            LaserRushGameMode.Instance.StartCountdown();
        }
    }
}
