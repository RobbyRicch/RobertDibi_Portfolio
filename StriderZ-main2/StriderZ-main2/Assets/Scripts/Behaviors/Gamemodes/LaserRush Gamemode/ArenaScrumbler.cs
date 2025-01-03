using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BreakingRings { FirstRing, SecondRing, ThirdRing, FourthRing, OuterRing }
public enum ArenaEvents { MeteorShower, DeadZone, GroundFall }

public class ArenaScrumbler : MonoBehaviour
{
    public static ArenaScrumbler Instance { get; private set; }
    private delegate void BreakRing();
    private BreakRing _currentBreakRingLogic;

    //[SerializeField] private bool _isUsingFallingPieces = false, _isUsingMeteorShower = false;
    private ForkManager _forkManager;
    public ForkManager ForkManager { get => _forkManager; set => _forkManager = value; }

    public LaserRushLateGameType ArenaEvent;
    [SerializeField] private Transform _endPointTr, _newCameraTr;
    [SerializeField] private GameObject _arenaWall;
    [SerializeField] private float _arenaWallTime = 3.0f;

    #region Falling Pieces Private Fields
    [SerializeField] private List<ArenaPiece> _allBreakingArenaPieces, _firstRing, _secondRing, _thirdRing, _fourthRing, _OuterstRing;
    [SerializeField] private BreakingRings _currentBreakingRing;
    [SerializeField] private float _timeBetweenPieces = 0.0f;
    [SerializeField] private float _timeBetweenRings = 0.0f;
    [SerializeField] private bool _isBreakingArena = false;
    private Vector3 _gravityForce = Physics.gravity;
    private int _playerCounter = 0;
    private float _randSecs = 0.0f;
    private string _playerTag = "Player";
    private string _deathlineTag = "Deathline";


    private bool _isForkOff = false;

    [Header("New Arena Ground Fall")]
    [SerializeField] private List<ArenaPiece> _AllNewArenaPeices;
    [SerializeField] private float _maxRandomTimeToFall;
    [SerializeField] private GameObject _middlePeice;
    #endregion
    [Space]
    [SerializeField] private Transform[] _allPlayerArenaSpawns;
    [SerializeField] private List<PlayerInputHandler> _playersInArena = new List<PlayerInputHandler>();
    public Transform PlayerNPC_Destenation;
    public NavMeshLink NavLink;
    private bool _respawned = false;
    private bool _eventStarted = false;

    [Space]
    [SerializeField] private float _forkFallTime = 5.0f;

    #region Meteor Shower Private Fields
    [Space][SerializeField] private GameObject _meteorShowerPrefab;
    #endregion

    #region Floor is Lava Private Fields
    [SerializeField] private GameObject _floorIsLavaPrefab;
    private bool _activateGroundFall = false;
    private float _timer = 0;
    private int currentRandomPiece = 0;
    private float num = 0;
    private bool turnOn = false;
    #endregion

    #region Monobehaviour Callbacks
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        switch (ArenaEvent)
        {
            case LaserRushLateGameType.MeteorShower:
                _meteorShowerPrefab.SetActive(false);
                break;
            case LaserRushLateGameType.FloorIsLava:
                _floorIsLavaPrefab.SetActive(false);
                break;
            case LaserRushLateGameType.GroundFall:
                //_currentBreakRingLogic = BreakOuterRing;
                break;
        }

    }
    private void Start()
    {
        LaserRushGameMode.Instance.EndPoint = _endPointTr;

        //PlayerManager.Instance.AllPlayersArenaSpawns = _allPlayerArenaSpawns;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();
            if (!_playersInArena.Contains(player))
            {
                _playersInArena.Add(player);
                // respawn players when all other players are in the arena
                /*if (_playersInArena.Count == PlayerManager.Instance.AllPlayersAlive.Count)
                {
                    
                }*/

                if (!_respawned)
                    RespawnPlayers();
                if (!_eventStarted)
                {
                    ArenaEvent = _forkManager.ActiveArenaEventType;
                    switch (ArenaEvent)
                    {
                        case LaserRushLateGameType.MeteorShower:
                            _meteorShowerPrefab.SetActive(true);
                            break;
                        case LaserRushLateGameType.FloorIsLava:
                            _floorIsLavaPrefab.SetActive(true);
                            break;
                        case LaserRushLateGameType.GroundFall:
                            _activateGroundFall = true;
                            break;
                    }
                    _eventStarted = true;
                }

                _eventStarted = true;
                //StartCoroutine(DropFork(_forkFallTime));
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();
            if (_playersInArena.Contains(player))
            {
                _playersInArena.Remove(player);
            }
        }
    }
    #endregion

    #region Arena
    private void RespawnPlayers()
    {
        List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
        for (int i = 0; i < allPlayers.Count; i++)
        {
            PlayerInputHandler currentPlayer = allPlayers[i];

            /*if (currentPlayer == player)
                continue;*/
            if (currentPlayer.Controller.IsAlive)
                continue;

            currentPlayer.transform.position = _allPlayerArenaSpawns[i].position;
            currentPlayer.Controller.Rb.angularVelocity = Vector3.zero;
            currentPlayer.Controller.Rb.velocity = Vector3.zero;
            currentPlayer.Attractor.ReturnLeftAttractor();
            currentPlayer.Attractor.ReturnRightAttractor();
            currentPlayer.Controller.Revive();
            currentPlayer.Controller.Rb.useGravity = true;
            currentPlayer.Controller.Rb.isKinematic = false;
            currentPlayer.IsPlayerInputsDisable = false;

           LaserRushGameMode.Instance.LaserRushUIHandler.ApplyColorToPlayerUI(currentPlayer);
        }
        _respawned = true;
    }
    private IEnumerator DropFork(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            _forkManager.transform.Translate(Vector3.down * Physics.gravity.magnitude / 100);
            yield return null;
        }
    }
    #endregion

    #region New Ground Fall
    private void Update()
    {
        if (_playersInArena.Count >= PlayerManager.Instance.AllPlayersAlive.Count & !_isForkOff)
        {
            _isForkOff = true;
            Invoke("TurnOffFork", 3);
        }
        if (_activateGroundFall)
        {
            _middlePeice.SetActive(false);
            if (_AllNewArenaPeices.Count > 0)
            {
                if (_timer >= 1)
                {
                    if (!turnOn)
                    {
                        int currentPieces = _AllNewArenaPeices.Count;
                        currentRandomPiece = Random.Range(0, currentPieces);
                        _AllNewArenaPeices[currentRandomPiece].VFX.SetActive(true);
                        turnOn = true;
                    }

                    foreach (var renderer in _AllNewArenaPeices[currentRandomPiece].Renderers)
                    {
                        //num = Mathf.Lerp(0, 10, 10);

                        num += (10 / 1) * Time.deltaTime;
                        Debug.Log(num);
                        renderer.material.SetFloat("_Alpha", num);

                    }
                    if (num >= 10)
                    {
                        _timer = 0;
                        num = 0;
                        _AllNewArenaPeices[currentRandomPiece].ShouldFall = true;
                        turnOn = false;
                        _AllNewArenaPeices.RemoveAt(currentRandomPiece);
                    }
                }
                else
                {
                    _timer += Time.deltaTime;
                }
            }
            else
            {
                _activateGroundFall = false;
            }
        }
    }
    private void TurnOffFork()
    {
        // error: NullReferenceException: Object reference not set to an instance of an object
        // ArenaScrumbler.TurnOffFork()(at Assets / Scripts / Behaviors / ArenaScrumbler.cs:182)
        RandomTracksTemplate.Instance.Tracks[RandomTracksTemplate.Instance.Tracks.Count - 1]
                .GetComponentInParent<Transform>().gameObject.SetActive(false);
    }
    private IEnumerator PlayGroundFall()
    {
        while (_AllNewArenaPeices.Count > 0)
        {
            //_randSecs = Random.Range(1, _maxRandomTimeToFall);
            int currentPieces = _AllNewArenaPeices.Count;
            int currentRandomPiece = Random.Range(0, currentPieces);
            StartCoroutine(FallPiece(_AllNewArenaPeices, currentRandomPiece));
            _AllNewArenaPeices.RemoveAt(currentRandomPiece);
            yield return new WaitForSeconds(10);
        }
    }
    private IEnumerator FallPiece(List<ArenaPiece> targetRing, int breakingPieceIndex)
    {
        targetRing[breakingPieceIndex].VFX.SetActive(true);
        foreach (var renderer in targetRing[breakingPieceIndex].Renderers)
        {
            float num = 0;
            while (num < 10)
            {
                num = Mathf.Lerp(0, 10, 3);
                Debug.Log(num);
                renderer.material.SetFloat("_Alpha", num);
            }
        }
        yield return new WaitForSeconds(3);
        targetRing[breakingPieceIndex].ShouldFall = true;
    }
    #endregion

    #region ArenaScrumbler Logic
    private void BreakPiece(List<ArenaPiece> targetRing, int breakingPieceIndex)
    {
        targetRing[breakingPieceIndex].ShouldFall = true;
    }
    private IEnumerator BreakTargetRing(List<ArenaPiece> targetRing, float maxRandSecs)
    {
        while (targetRing.Count > 0)
        {
            _randSecs = Random.Range(0, maxRandSecs);
            int currentPieces = targetRing.Count;
            int currentRandomPiece = Random.Range(0, currentPieces);

            BreakPiece(targetRing, currentRandomPiece);
            targetRing.RemoveAt(currentRandomPiece);
            yield return new WaitForSeconds(_randSecs);
        }

        _isBreakingArena = false;
        int nextRingNum = (int)_currentBreakingRing - 1;
        _currentBreakingRing = (BreakingRings)nextRingNum;

        switch (_currentBreakingRing)
        {
            case BreakingRings.FirstRing:
                _currentBreakRingLogic = BreakFirstRing;
                break;
            case BreakingRings.SecondRing:
                _currentBreakRingLogic = BreakSecondRing;
                break;
            case BreakingRings.ThirdRing:
                _currentBreakRingLogic = BreakThirdRing;
                break;
            case BreakingRings.FourthRing:
                _currentBreakRingLogic = BreakFourthRing;
                break;
            case BreakingRings.OuterRing:
                _currentBreakRingLogic = BreakOuterRing;
                break;
            default:
                break;
        }

        BreakCurrentRing();
    }
    private IEnumerator BreakAllInRandomDelay()
    {
        while (_allBreakingArenaPieces.Count > 0)
        {
            _randSecs = Random.Range(0, 1.5f);
            int currentPieces = _allBreakingArenaPieces.Count;
            int currentRandomPiece = Random.Range(0, currentPieces);

            BreakPiece(_allBreakingArenaPieces, currentRandomPiece);
            _allBreakingArenaPieces.RemoveAt(currentRandomPiece);
            yield return new WaitForSeconds(_randSecs);
        }

        _isBreakingArena = false;
    }
    private void BreakOuterRing()
    {
        _isBreakingArena = true;
        StartCoroutine(BreakTargetRing(_OuterstRing, _timeBetweenPieces));
    }
    private void BreakFourthRing()
    {
        _isBreakingArena = true;
        StartCoroutine(BreakTargetRing(_fourthRing, _timeBetweenPieces));
    }
    private void BreakSecondRing()
    {
        _isBreakingArena = true;
        StartCoroutine(BreakTargetRing(_secondRing, _timeBetweenPieces));
    }
    private void BreakThirdRing()
    {
        _isBreakingArena = true;
        StartCoroutine(BreakTargetRing(_thirdRing, _timeBetweenPieces));
    }
    private void BreakFirstRing()
    {
        _isBreakingArena = true;
        StartCoroutine(BreakTargetRing(_firstRing, _timeBetweenPieces));
    }
    #endregion

    #region ArenaScrumbler Behavior
    public void BreakCurrentRing()
    {
        if (_currentBreakRingLogic != null)
        {
            Invoke(_currentBreakRingLogic.Method.Name, _timeBetweenRings);
        }
    }
    public void BreakAllArena()
    {
        _isBreakingArena = true;
        StartCoroutine(BreakAllInRandomDelay());
    }
    #endregion

    #region Pieces Behavior
    public void ApplyGravity(Transform transform)
    {
        transform.position += (_gravityForce / 6);
    }
    #endregion

    #region Meteor Shower
    private void ActivateMeteorShower()
    {

    }
    #endregion
}