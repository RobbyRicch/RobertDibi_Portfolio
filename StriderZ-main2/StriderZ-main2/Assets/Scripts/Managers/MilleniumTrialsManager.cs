using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilleniumTrialsManager : MonoBehaviour
{
    private static MilleniumTrialsManager _instance;
    public static MilleniumTrialsManager Instance => _instance;

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField] private Animator[] _frontSpawnWallAnimators;
    public Animator[] FrontSpawnWalls => _frontSpawnWallAnimators;

    [SerializeField] private Transform[] _linupTransforms;
    public Transform[] LinupTransforms => _linupTransforms;

    #region Trigger Switches
    private TriggerSwitch[] _allTrackShifters;
    public TriggerSwitch[] AllTrackShifters => _allTrackShifters;
    #endregion

    #region Retract Bridges
    private TriggerSwitch[] _allRetractingBridges;
    public TriggerSwitch[] AllRetractingBridges => _allRetractingBridges;
    #endregion
}
