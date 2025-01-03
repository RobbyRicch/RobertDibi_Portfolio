using Cinemachine;
using UnityEngine;

public class TaskBase : MonoBehaviour
{
    [SerializeField] protected bool _isTaskComplete = false;
    public bool IsTaskComplete => _isTaskComplete;

    protected Player_Controller _playerController;
    public Player_Controller PlayerController { get => _playerController; set => _playerController = value; }

    protected CinemachineVirtualCamera _vCam;
    public CinemachineVirtualCamera VCam { get => _vCam; set => _vCam = value; }
}
