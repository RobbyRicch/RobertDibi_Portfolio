using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachineVirtualCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _vCam;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _originalFollowTarget;
    private Transform _currentFollowTarget = null;

    private void Start()
    {
        _currentFollowTarget = _originalFollowTarget;
    }

    public void DoZoom()
    {
        _animator.SetBool("Zoom", true);
    }
    public void DoZoom(Transform newFollowTarget)
    {
        _vCam.Follow = newFollowTarget;
        _animator.SetBool("Zoom", true);
    }

    public void EndZoom()
    {
        _animator.SetBool("Zoom", false);
    }
    public void EndZoom(Transform newFollowTarget)
    {
        _vCam.Follow = newFollowTarget;
        _animator.SetBool("Zoom", false);
    }
}
