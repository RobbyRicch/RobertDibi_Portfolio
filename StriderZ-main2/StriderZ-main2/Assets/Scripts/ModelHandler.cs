using System;
using System.Collections.Generic;
using UnityEngine;

public class ModelHandler : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private int _numOfModels = 4;
    public int NumOfModels => _numOfModels;

    [SerializeField] private GameObject _tempPhaseModel;
    public GameObject TempPhaseModel => _tempPhaseModel;

    [SerializeField] private GameObject[] _allModels;
    public GameObject[] AllModels => _allModels;

    [SerializeField] private Animator[] _allAnimators;

    [SerializeField] private GameObject[] _allWinHelmets;
    public GameObject[] AllWinHelmets => _allWinHelmets;

    [SerializeField] private SkinnedMeshRenderer[] _allHelmets, _allBodies;
    public SkinnedMeshRenderer[] AllHelmets => _allHelmets;
    public SkinnedMeshRenderer[] AllBodys => _allBodies;

    [SerializeField] private List<SkinnedMeshRenderer[]> _allLeftHands, _allRightHands;
    public List<SkinnedMeshRenderer[]> AllLeftHands => _allLeftHands;
    public List<SkinnedMeshRenderer[]> AllRightHands => _allRightHands;

    [SerializeField] private Transform[][] _allHandsRig;
    public Transform[][] AllHandsRig => _allHandsRig;

    [SerializeField] private Transform[][] _allItemsOrigin;
    public Transform[][] AllItemsOrigin => _allItemsOrigin;

    [SerializeField] private Transform[][] _allUpperArmsOrigin;
    public Transform[][] AllUpperArmsOrigin => _allUpperArmsOrigin;

    #region Hands Meshes
    [SerializeField] private SkinnedMeshRenderer[] _nbusLeftHand, _nbusRightHand, _testRightHand;
    [SerializeField] private SkinnedMeshRenderer[] _rashaLeftHand, _rashaRightHand;
    [SerializeField] private SkinnedMeshRenderer[] _chaseLeftHand, _chaseRightHand;
    [SerializeField] private SkinnedMeshRenderer[] _catherineLeftHand, _catherineRightHand;
    #endregion

    #region Hands Rig: 0 = left, 1 = right
    [SerializeField] private Transform[] _nbusHandsRig, _testHandsRig;
    [SerializeField] private Transform[] _rashaHandsRig;
    [SerializeField] private Transform[] _chaseHandsRig;
    [SerializeField] private Transform[] _catherineHandsRig;
    #endregion

    #region Items Origin - 0 = back, 1 = left hand, 2 = right hand.
    [SerializeField] private Transform[] _nbusItemsOrigin, _rashaItemsOrigin, _chaseItemsOrigin, _catherineItemsOrigin;
    public Transform[] NbusItemsOrigin => _nbusItemsOrigin;
    public Transform[] RashaItemsOrigin => _rashaItemsOrigin;
    public Transform[] ChaseItemsOrigin => _chaseItemsOrigin;
    public Transform[] CatherineItemsOrigin => _catherineItemsOrigin;
    #endregion

    #region Shoulders Origin - 0 = left shoulder, 1 = right shoulder.
    [SerializeField] private Transform[] _nbusUpperArmsOrigin, _rashaUpperArmsOrigin, _chaseUpperArmsOrigin, _catherineUpperArmsOrigin;
    public Transform[] NbusUpperArmsOrigin => _nbusUpperArmsOrigin;
    public Transform[] RashaUpperArmsOrigin => _rashaUpperArmsOrigin;
    public Transform[] ChaseUpperArmsOrigin => _chaseUpperArmsOrigin;
    public Transform[] CatherineUpperArmsOrigin => _catherineUpperArmsOrigin;
    #endregion

    /*[SerializeField] private TrailRenderer[] _allTrails;
    public TrailRenderer[] AllTrails => _allTrails;

    [SerializeField] private TrailRenderer[] _allNitroTrails;
    public TrailRenderer[] AllNitroTrails => _allNitroTrails;

    [SerializeField] private ParticleSystem[] _allStunVfx, _allSpeedVfx;
    public ParticleSystem[] AllStunVfx => _allStunVfx;
    public ParticleSystem[] AllSpeedVfx => _allSpeedVfx;

    [SerializeField] private ParticleSystem[] _allDRSStruck, _allDRSStrike, _allEMP, _allParalyze;
    public ParticleSystem[] AllDRSStruck => _allDRSStruck;
    public ParticleSystem[] AllDRSStrike => _allDRSStrike;
    public ParticleSystem[] AllEMP => _allEMP;
    public ParticleSystem[] AllParalyze => _allParalyze;

    [SerializeField] private ParticleSystem[] _allDeathlineHitVFX, _allLavaHitVFX;
    public ParticleSystem[] AllDeathlineHitVFX => _allDeathlineHitVFX;
    public ParticleSystem[] AllLavaHitVFX => _allLavaHitVFX;*/

    private void Awake()
    {
        _allLeftHands = new List<SkinnedMeshRenderer[]>();
        _allRightHands = new List<SkinnedMeshRenderer[]>();
        _allHandsRig = new Transform[][] { _nbusHandsRig, _rashaHandsRig, _chaseHandsRig, _catherineHandsRig };
        _allItemsOrigin = new Transform[][] { _nbusItemsOrigin, _rashaItemsOrigin, _chaseItemsOrigin, _catherineItemsOrigin };
        _allUpperArmsOrigin = new Transform[][] { _nbusUpperArmsOrigin, _rashaUpperArmsOrigin , _chaseUpperArmsOrigin , _catherineUpperArmsOrigin };

        _allLeftHands.Add(_nbusLeftHand);
        _allLeftHands.Add(_rashaLeftHand);
        _allLeftHands.Add(_chaseLeftHand);
        _allLeftHands.Add(_catherineRightHand);

        _allRightHands.Add(_nbusRightHand);
        _allRightHands.Add(_rashaRightHand);
        _allRightHands.Add(_chaseRightHand);
        _allRightHands.Add(_catherineRightHand);
    }

    private void ClearModels()
    {
        for (int i = 0; i < _numOfModels; i++)
            _allModels[i].SetActive(false);
    }
    public void ChangeModel(PlayerInputHandler player, int modelNum)
    {
        ClearModels();

        if (modelNum < _numOfModels -1)
            modelNum++;
        else
            modelNum = 0;

        player.Controller.Animator = _allAnimators[modelNum];
        player.Data.ModelData.Icon = _allWinHelmets[modelNum];
        //player.Data.ModelData.IconImage = _allWinHelmets[modelNum];
        player.Data.ModelData.HelmetMesh = _allHelmets[modelNum];
        player.Data.ModelData.BodyMesh = _allBodies[modelNum];
        player.Data.ModelData.LeftHandMeshes = _allLeftHands[modelNum];
        player.Data.ModelData.RightHandMeshes = _allRightHands[modelNum];
        player.Data.ModelData.AttractorHands = _allHandsRig[modelNum];
        player.Attractor.LeftHandTr = _allHandsRig[modelNum][0];
        player.Attractor.RightHandTr = _allHandsRig[modelNum][1];
        player.Attractor.LeftHandParent = _allHandsRig[modelNum][0].parent;
        player.Attractor.RightHandParent = _allHandsRig[modelNum][1].parent;
        player.Data.ModelData.ItemsOrigin = _allItemsOrigin[modelNum];
        player.Data.ModelData.UpperArmsOrigin = _allUpperArmsOrigin[modelNum];
        player.Data.ModelData.ItemsOrigin = _allItemsOrigin[modelNum];

        _allModels[modelNum].SetActive(true);
    }
    public void ChangeSetupModel(int modelNum)
    {
        ClearModels();

        if (modelNum < _numOfModels - 1)
            modelNum++;
        else
            modelNum = 0;

        _allModels[modelNum].SetActive(true);
    }
}