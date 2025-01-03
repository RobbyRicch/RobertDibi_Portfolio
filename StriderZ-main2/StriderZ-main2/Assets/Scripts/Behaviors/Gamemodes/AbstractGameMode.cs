using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public abstract class AbstractGameMode : MonoBehaviour
{
    #region Scoring
    [Header("Scoring")]
    [SerializeField] protected int _maxScore;
    public int MaxScore => _maxScore;
    #endregion

    #region Cinemachine Data
    [Header("Cinemachine Data")]
    [SerializeField] protected Camera _mainCam;
    public Camera MainCam => _mainCam;

    [SerializeField] protected CinemachineVirtualCamera _virtualCamera;
    public CinemachineVirtualCamera VirtualCamera => _virtualCamera;

    [SerializeField] protected CinemachineVirtualCamera[] _allVirtualCameras;
    public CinemachineVirtualCamera[] AllVirtualCameras => _allVirtualCameras;
    #endregion

    #region Locations
    [Header("Locations")]
    [SerializeField] protected Transform[] _playerSpawns;
    public Transform[] PlayerSpawns => _playerSpawns;

    [SerializeField] protected Transform _deathArea;
    public Transform DeathArea => _deathArea;
    #endregion

    #region Victory & Conclusions Data
    [Header("Victory & Conclusions Data")]
    [SerializeField] protected float _victoryDelay = 1.0f;
    [SerializeField] protected float _resultsDelay = 1.0f, _winDelay = 1.0f;
    public float VictoryDelay => _victoryDelay;
    public float ResultsDelay => _resultsDelay;
    public float WinDelay => _winDelay;

    protected IEnumerator _victorySequence, _conclusionSequence;
    #endregion

    #region Podiums Data
    [Header("The Podium Data")]
    [SerializeField] protected Transform[] _podiumTransforms;
    public Transform[] PodiumTransforms => _podiumTransforms;

    [SerializeField] protected float _podiumTime = 5.0f;
    #endregion

    #region Position Handling
    protected void SetPlayerSpawnPositions(PlayerInputHandler[] players)
    {
        foreach (PlayerInputHandler player in players)
        {
            player.Controller.SetNewSpawnPoint(_playerSpawns[player.SetupData.ID]);
            StartCoroutine(player.Controller.Recreate);
        }
    }
    protected Transform[] AssignPlayersToPodiums()
    {
        List<PlayerInputHandler> players = PlayerManager.Instance.AllPlayers.OrderByDescending(p => p.Data.Score).ToList();
        Transform[] sortedTransforms = players.Select(player => player.transform).ToArray();
        Quaternion newRotation = Quaternion.identity * Quaternion.Euler(1.0f, 180.0f, 1.0f);

        for (int i = 0; i < sortedTransforms.Length; i++)
        {
            sortedTransforms[i].SetParent(_podiumTransforms[i].transform);
            sortedTransforms[i].localPosition = Vector3.zero;
            sortedTransforms[i].localRotation = Quaternion.identity;
            players[i].Controller.Rb.useGravity = false;
            players[i].Controller.Rb.isKinematic = true;
            sortedTransforms[i].SetParent(null);
            sortedTransforms[i].localRotation = newRotation;
            DontDestroyOnLoad(sortedTransforms[i]);
        }

        return sortedTransforms;
    }
    #endregion

    #region Alert Handling
    public void StartCountdown()
    {
        SoundManager.Instance.PlayAnnouncerSound(SoundManager.Instance.RoundStartCountdown);
        AlertManager.Instance.PlayCountdownPopUp();
    }
    #endregion

    #region Coroutines
    private IEnumerator ConclusionSequence()
    {
        CinemachineManager.Instance.ClearTargetGroup();
        CinemachineManager.Instance.ActivatePodiumVirtualCamera();
        AssignPlayersToPodiums();
        yield return new WaitForSeconds(_podiumTime);

        UIManager.Instance.ScoreboardPanel.SetActive(false);
        yield return new WaitForSeconds(_podiumTime);

        UIManager.Instance.FadeIn(0.5f);
        yield return new WaitForSeconds(0.5f);

        CustomSceneManager.ReturnToLobby();
    }
    #endregion

    #region Events
    protected void OnGameModeLaunched()
    {
        if (CinemachineManager.Instance == null)
            return;

        if (PlayerManager.Instance == null)
            return;

        CinemachineManager.Instance.TargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];

        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerInputHandler player = allPlayers[i];

            if (!player.Controller.IsAlive)
                player.Controller.Revive();
                
            CinemachineManager.Instance.TargetGroup.AddMember(player.Data.TrackCamTr, CinemachineManager.Instance.TargetWeight, CinemachineManager.Instance.TargetRadius);
        }
        CinemachineManager.Instance.SetupNewScene(_mainCam, _virtualCamera, _allVirtualCameras);

        _conclusionSequence = ConclusionSequence();
        UIManager.Instance.FadeOut(0.5f); // need to set off and on the Transition Canvas

        //SoundManager.Instance.PlayMusicSound();

        GameManager.Instance.RoundNumber++;
        EventManager.InvokeSpawnPlayers(PlayerManager.Instance.AllPlayers.ToArray());
    }
    protected void OnSpawnPlayers(PlayerInputHandler[] players)
    {
        SetPlayerSpawnPositions(players);
    }
    protected void OnGameModeBegin()
    {
        GameManager.Instance.PodiumTransforms = _podiumTransforms;
        CinemachineManager.Instance.ActivateTrackVirtualCamera();
    }
    protected void OnGameModeConcluded()
    {
        GameManager.Instance.RoundNumber = 1;
        StartCoroutine(_conclusionSequence);
    }
    #endregion
}
