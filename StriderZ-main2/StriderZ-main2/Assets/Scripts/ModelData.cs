using System;
using System.Collections.Generic;
using UnityEngine;

public enum ModelType { NBUS, Rasha, Chase, Catherine, Jinn }

public class ModelData : MonoBehaviour
{
    #region General
    [Header("General")]
    [SerializeField] private Animator _animator;
    public Animator Animator => _animator;
    #endregion

    #region UI Related
    [Header("Icon")]
    [SerializeField] private GameObject _icon;
    public GameObject Icon { get => _icon; set => _icon = value; }

    [SerializeField] private UnityEngine.UI.Image _iconImage;
    public UnityEngine.UI.Image IconImage => _iconImage;

    [Header("Score Text")]
    [SerializeField] private GameObject _scoreTMPro;
    public GameObject ScoreTMPro { get => _scoreTMPro; set => _scoreTMPro = value; }

    [SerializeField] private TMPro.TextMeshProUGUI _scoreText;
    public TMPro.TextMeshProUGUI ScoreText => _scoreText;

    [Header("Gained Score Text")]
    [SerializeField] private GameObject _gainedScoreTMPro;
    public GameObject GainedScoreTMPro { get => _gainedScoreTMPro; set => _gainedScoreTMPro = value; }

    [SerializeField] private TMPro.TextMeshProUGUI _gainedScoreText;
    public TMPro.TextMeshProUGUI GainedScoreText => _gainedScoreText;
    #endregion

    #region Main Visuals
    [Header("Main Visuals")]
    [SerializeField] private SkinnedMeshRenderer _helmetMesh;
    [SerializeField] private SkinnedMeshRenderer _bodyMesh;
    public SkinnedMeshRenderer HelmetMesh { get => _helmetMesh; set => _helmetMesh = value; }
    public SkinnedMeshRenderer BodyMesh { get => _bodyMesh; set => _bodyMesh = value; }
    #endregion

    #region Hands Visual: ~ 0(base hand), 1(1st finger), 2(2nd finger)... 5(thumb) ~
    [Header("Hands Visual")]
    [SerializeField] private SkinnedMeshRenderer[] _leftHandMeshes;
    [SerializeField] private SkinnedMeshRenderer[] _rightHandMeshes;
    public SkinnedMeshRenderer[] LeftHandMeshes { get => _leftHandMeshes; set => _leftHandMeshes = value; }
    public SkinnedMeshRenderer[] RightHandMeshes { get => _rightHandMeshes; set => _rightHandMeshes = value; }

    [SerializeField] private SkinnedMeshRenderer[] _leftAttractorHandMeshes, _rightAttractorHandMeshes;
    public SkinnedMeshRenderer[] LeftAttractorHandMeshes { get => _leftAttractorHandMeshes; set => _leftAttractorHandMeshes = value; }
    public SkinnedMeshRenderer[] RightAttractorHandMeshes { get => _rightAttractorHandMeshes; set => _rightAttractorHandMeshes = value; }
    #endregion

    #region Hands Rig: ~ 0(left hand), 1(right hand) ~
    [Header("Hands Rig")]
    [SerializeField] private Transform[] _upperArmsOrigin;
    public Transform[] UpperArmsOrigin { get => _upperArmsOrigin; set => _upperArmsOrigin = value; }
    
    [SerializeField] private Transform[] _attractorHands;
    public Transform[] AttractorHands { get => _attractorHands; set => _attractorHands = value; }
    #endregion

    #region Pickables: ~ 0(back), 1(left hand), 2(right hand) ~
    [Header("Pickables")]
    [SerializeField] private Transform[] _itemsOrigin;
    public Transform[] ItemsOrigin { get => _itemsOrigin; set => _itemsOrigin = value; }
    #endregion
}