using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public enum LaserRushVCType // virtual camera type
{
    View,
    Spawn,
    Track,
    WinRound,
    Arena,
    Podiums
}

public class CinemachineManager : MonoBehaviour
{
    private static CinemachineManager _instance;
    public static CinemachineManager Instance => _instance;

    #region Data
    [Header("Data")]
    [SerializeField] private Camera _mainCam;
    public Camera MainCam => _mainCam;

    [SerializeField] private CinemachineTargetGroup _targetGroup;
    public CinemachineTargetGroup TargetGroup => _targetGroup;

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    public CinemachineVirtualCamera VirtualCamera => _virtualCamera;

    [SerializeField] private CinemachineVirtualCamera[] _allVirtualCameras;
    public CinemachineVirtualCamera[] AllVirtualCameras => _allVirtualCameras;

    private Transform _winCamTr;
    public Transform WinCamTr { get => _winCamTr; set => _winCamTr = value; }

    [SerializeField] private float _targetsWeight = 1.0f, _targetsRadius = 3.0f;
    public float TargetWeight => _targetsWeight;
    public float TargetRadius => _targetsRadius;
    #endregion

    #region UI Elements
    [Header("UI Elements")]
    [SerializeField] private RectTransform[] _cutEffectImages;
    [SerializeField] float _startCutEffectHeight, _endCutEffectHeight;
    #endregion

    private IEnumerator _activeCutEffectRoutine;

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        _mainCam = Camera.main;
    }
    #endregion

    public void SetupNewScene(Camera newMainCam, CinemachineVirtualCamera newFirstVirtualCam)
    {
        _mainCam = newMainCam;
        _virtualCamera = newFirstVirtualCam;
    }
    public void SetupNewScene(Camera newMainCam, CinemachineVirtualCamera newFirstVirtualCam, CinemachineVirtualCamera[] newAllVirtualCameras)
    {
        _mainCam = newMainCam;
        _virtualCamera = newFirstVirtualCam;
        _allVirtualCameras = newAllVirtualCameras;
    }
    public void SwitchVirtualCameras(int virtualCameraTypeIndex)
    {
        _allVirtualCameras[virtualCameraTypeIndex].gameObject.SetActive(true);
        _virtualCamera.gameObject.SetActive(false);
        _virtualCamera = _allVirtualCameras[virtualCameraTypeIndex];
    }

    #region LaserRush Camera Handling
    public void ActivateTrackVirtualCamera()
    {
        int trackCamIndex = (int)LaserRushVCType.Track;

        _allVirtualCameras[trackCamIndex].Follow = _targetGroup.transform;
        _allVirtualCameras[trackCamIndex].LookAt = _targetGroup.transform;
        _allVirtualCameras[trackCamIndex].gameObject.SetActive(true);

        _virtualCamera.gameObject.SetActive(false);
        _virtualCamera = _allVirtualCameras[trackCamIndex];
    }
    public void ActivateArenaVirtualCamera()
    {
        int arenaCamIndex = (int)LaserRushVCType.Arena;

        _allVirtualCameras[arenaCamIndex].Follow = _targetGroup.transform;
        _allVirtualCameras[arenaCamIndex].LookAt = _targetGroup.transform;
        _allVirtualCameras[arenaCamIndex].gameObject.SetActive(true);

        _virtualCamera.gameObject.SetActive(false);
        _virtualCamera = _allVirtualCameras[arenaCamIndex];
    }
    public void ActivatePodiumVirtualCamera()
    {
        int podiumCamIndex = (int)LaserRushVCType.Podiums;
        _allVirtualCameras[podiumCamIndex].gameObject.SetActive(true);
        _virtualCamera.gameObject.SetActive(false);
        _virtualCamera = _allVirtualCameras[podiumCamIndex];
    }
    public void ClearTargetGroup()
    {
        _targetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
    }
    #endregion

    public void ActivateWinRoundCamera(PlayerInputHandler winningPlayer, Transform winningPlayerWinCamTr)
    {
        _winCamTr = winningPlayerWinCamTr;

        int winRoundCamIndex = (int)LaserRushVCType.WinRound;
        _allVirtualCameras[winRoundCamIndex].Follow = winningPlayer.Data.WinCamTr;
        _allVirtualCameras[winRoundCamIndex].LookAt = winningPlayer.transform;
        _allVirtualCameras[winRoundCamIndex].gameObject.SetActive(true);
        _virtualCamera.gameObject.SetActive(false);
        _virtualCamera = _allVirtualCameras[winRoundCamIndex];
        _virtualCamera.transform.position = _winCamTr.position;
    }

    #region Coroutines
    public IEnumerator OnCutWithEffectRoutine(float effectTime, int cameraTypeIndex)
    {
        /*-- effect logic here --*/

        yield return new WaitForSeconds(effectTime);

        SwitchVirtualCameras(cameraTypeIndex);
        //_mainCam.GetComponent<CinemachineBrain>().ActiveBlend.BlendWeight
    }
    #endregion

    #region Unity Events
    public void OnCutWithEffect(int cameraTypeIndex) // if theres a problem check for inspector index to be same as enum
    {
        _activeCutEffectRoutine = null;
        _activeCutEffectRoutine = OnCutWithEffectRoutine(0.5f, cameraTypeIndex);
        StartCoroutine(_activeCutEffectRoutine);
    }
    #endregion
}
