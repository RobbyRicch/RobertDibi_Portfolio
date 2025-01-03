using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrailType { Regular, Nitro }

public class PlayerData : MonoBehaviour
{
    [SerializeField] private int _id; // to remove
    public int ID { get => _id; set => _id = value; }

    [SerializeField] private string _nickname; // to remove
    public string Nickname { get => _nickname; set => _nickname = value; }

    [Header("General Data")]
    [SerializeField] private TMPro.TextMeshProUGUI _nickNameTMPro;
    public TMPro.TextMeshProUGUI NickNameTMPro => _nickNameTMPro;

    [SerializeField] private Sprite _userIcon;
    public Sprite UserIcon { get => _userIcon; set => _userIcon = value; }

    [SerializeField] private GameObject[] _models;
    public GameObject[] Models => _models;

    [SerializeField] private ModelData _modelData;
    public ModelData ModelData { get => _modelData; set => _modelData = value; }

    [SerializeField] private Transform _projectileTr;
    public Transform ProjectileTr => _projectileTr;

    [SerializeField] private Transform _trackCamTr, _winCamTr;
    public Transform TrackCamTr => _trackCamTr;
    public Transform WinCamTr => _winCamTr;

    [SerializeField] private GameObject _speedLines;
    public GameObject SpeedLines => _speedLines;

    [SerializeField] private TrailRenderer _mainTrail, _nitroTrail; // change to array and use TrailType
    public TrailRenderer MainTrail { get => _mainTrail; set => _mainTrail = value; }
    public TrailRenderer NitroTrail { get => _nitroTrail; set => _nitroTrail = value; }

    [SerializeField] private float _dangerTime = 0;
    public float DangerTime { get => _dangerTime; set => _dangerTime = value; }

    [SerializeField] private float _globalDangerTime = 6.0f;
    public float GlobalDangerTime => _globalDangerTime;

    [SerializeField] private int _previousScore = 0, _score = 0, _dangerCounter = 0;
    public int Score { get => _score; set => _score = value; }
    public int PreviousScore { get => _previousScore; set => _previousScore = value; }
    public int DangerCounter { get => _dangerCounter; set => _dangerCounter = value; }

    [Header("Components")]
    [SerializeField] private PlayerWorldUI _playerWorldUI;
    public PlayerWorldUI PlayerWorldUI => _playerWorldUI;
    
    [SerializeField] private PickupAnimationManager[] _pickupAnimationManagers;
    public PickupAnimationManager[] PickupAnimationManagers { get => _pickupAnimationManagers; set => _pickupAnimationManagers = value; }

    [SerializeField] private PickableStatusEffect _pickupStatusEffectItem;
    public PickableStatusEffect PickupStatusEffectItem { get => _pickupStatusEffectItem; set => _pickupStatusEffectItem = value; }

    [SerializeField] private PickableAbilty _pickupItem;
    public PickableAbilty PickupItem { get => _pickupItem; set => _pickupItem = value; }

    [SerializeField] private Weapon _currentWeapon;
    public Weapon CurrentWeapon { get => _currentWeapon; set => _currentWeapon = value; }

    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem _speedVFX;
    public ParticleSystem SpeedVFX => _speedVFX;

    [SerializeField] private GameObject _playerElimVFX;
    public GameObject PlayerElimVFX { get => _playerElimVFX; set => _playerElimVFX = value; }

    [SerializeField] private GameObject _fallHoverVfx;
    public GameObject FallHoverVfx => _fallHoverVfx;

    [SerializeField] private GameObject _respawnVfx;
    public GameObject RespawnVfx => _respawnVfx;

    [SerializeField] private GameObject _dangerVfx;
    public GameObject DangerVfx => _dangerVfx;

    [SerializeField] private GameObject _deathlineDeathVFXGO;
    public GameObject DeathlineDeathVFXGO { get => _deathlineDeathVFXGO; set => _deathlineDeathVFXGO = value; }

    [SerializeField] private ParticleSystem _deathLineHitVFX;
    public ParticleSystem DeathLineHitVFX { get => _deathLineHitVFX; set => _deathLineHitVFX = value; }

    /* remove all pickup effects and make sure they work from pickups */

    // pickups effects
    [SerializeField] private ParticleSystem _aura, _stunVFX, _strikeVFX, _EMPVFX, _sharkBiteVFX;
    public ParticleSystem Aura { get => _aura; set => _aura = value; }
    public ParticleSystem StunVFX { get => _stunVFX; set => _stunVFX = value; }
    public ParticleSystem StrikeVFX { get => _strikeVFX; set => _strikeVFX = value; }
    public ParticleSystem EMPVFX { get => _EMPVFX; set => _EMPVFX = value; }
    public ParticleSystem SharkBiteVFX { get => _sharkBiteVFX; set => _sharkBiteVFX = value; }

    // arena effects
    [SerializeField] private ParticleSystem _lavaVFX;
    public ParticleSystem LavaVFX { get => _lavaVFX; set => _lavaVFX = value; }
}
