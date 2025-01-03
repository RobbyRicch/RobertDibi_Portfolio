using System;
using System.Collections.Generic;
using UnityEngine;

public class DeathlineCollider : MonoBehaviour
{
    [Header("Deathline UI Elements")]
    [SerializeField] private RectTransform _deathLineUI;
    public RectTransform DeathLineUI => _deathLineUI;

    [Header("Player Impact")]
    [SerializeField] private int _maxDangerCount = 1;
    [SerializeField] private float _cameraShakeDuration = 2.0f, _cameraShakeStrenght = 0.5f;
    [SerializeField] private float _bounceForce = 60.0f, _dangerTime = 6.0f;
    [SerializeField] private float _distanceToKillBehind = 10.0f;

    [Header("Components")]
    [SerializeField] private BoxCollider _hitCollider;

    [Header("Raycast")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _rayDistance = 100.0f;
    private Vector3 _deathLineAndGroundContactPoint = Vector3.zero;

    [SerializeField] private float _autonomicSpeed = 8.0f;

    [SerializeField] private bool _isActive;

    [SerializeField] private bool _isMoving;
    public bool IsMoving { get => _isMoving; set => _isMoving = value; }

    private Ray _cameraRay;

    private string _playerTag = "Player";

    private delegate void State();
    private State _state;

    [SerializeField] private bool _isDebugMessagesOn;

    private void Awake()
    {
        if (!_hitCollider.isTrigger)
            _hitCollider.isTrigger = true;
    }
    private void OnEnable()
    {
        EventManager.OnPlayerPause += OnPauseMenu;
    }
    private void Start()
    {
        if (PlayerSetupManager.Instance == null)
            return;

        _playerTag = PlayerSetupManager.Instance.PlayerPrefab.tag;
    }
    private void OnDisable()
    {
        EventManager.OnPlayerPause -= OnPauseMenu;
    }
    private void Update()
    {
        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CurrentGameState == GameStates.MidGame && _isMoving && IsDeathLineAboveGround())
        {
            Vector3 newPos = _deathLineAndGroundContactPoint;
            newPos.y = transform.localScale.y / 2;
            transform.position = newPos;
        }
    }
    private void FixedUpdate()
    {
        KillPlayers();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            //StartCoroutine(CameraManager.Instance.CameraShake(1f, 0f));
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();
            //player.Data.DangerTime = _dangerTime;
            player.Controller.SetDangerTime();
            //player.PlayerWorldUI.ChangeEnergyColor(Color.red);
            player.Data.DeathLineHitVFX.transform.rotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
            player.Data.DeathLineHitVFX.Play();

            if (player.Data.DangerCounter > _maxDangerCount)
            {
                player.Controller.IsAlive = false;
                player.Controller.IsInDanger = false;
            }
            else
                player.Controller.Rb.AddForce(transform.transform.forward * _bounceForce, ForceMode.Impulse);
        }
        /*else if (other.CompareTag("PlayerNPC"))
        {

        }*/
    }

    private void ScreenState()
    {

    }
    private void SpeedingState()
    {

    }

    private bool IsDeathLineAboveGround()
    {
        _cameraRay = CinemachineManager.Instance.MainCam.ScreenPointToRay(_deathLineUI.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(_cameraRay, out hit, _rayDistance, (int)_groundLayer))
        {
            Debug.DrawLine(_cameraRay.origin, hit.point, Color.red);
            _deathLineAndGroundContactPoint = hit.point;
            return true;
        }
        Debug.DrawRay(_cameraRay.origin, _cameraRay.direction * _rayDistance, Color.red);
        return false;
    }
    private void KillPlayers()
    {
        if (PlayerManager.Instance == null)
            return;

        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        for (int i = 0; i < allPlayersAlive.Count; i++)
        {
            if (allPlayersAlive[i].transform.position.z < transform.position.z - _distanceToKillBehind)
                allPlayersAlive[i].Controller.IsAlive = false;
        }
    }

    #region Events
    private void OnPauseMenu(PlayerInputHandler player)
    {
        _isMoving = false;
    }
    private void OnClosePauseMenu()
    {
        _isMoving = true;
    }
    private void OnRoundStart()
    {
        _isMoving = true;
    }
    #endregion
}
