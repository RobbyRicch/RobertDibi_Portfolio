using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

/*-Player max speed
- Player boosted max speed

- Deathline speed = lower than Player max speed but close.
- Deathline push impulse = impulse force is set by distance of Deathline from camera center
  and on the impulse frame the target should be the lower middle edge of the cameras bounding box (aiming at the last player)*/

public class Deathline : MonoBehaviour
{
    private static Deathline _instance;
    public static Deathline Instance => _instance;

    [SerializeField] private float _timeToActivateFaterCountdown = 1.0f;
    public float TimeToActivateFaterCountdown => _timeToActivateFaterCountdown;

    [Header("Movement")]
    [SerializeField] private float _height = 6.0f;
    [SerializeField] private float _chasingThreshold = 1f, _maxDistanceFromPlayers = 100.0f;
    [SerializeField] private float _speed = 20.0f, _speedFactor = 5.0f, _rotationFactor = 0.3f, _turnPower = 25.0f;
    [SerializeField] private bool _isMoving = false, _isChasing = false;

    [Header("Player Impact")]
    [SerializeField] private int _maxDangerCount = 1;
    [SerializeField] private float _cameraShakeDuration = 2.0f, _cameraShakeStrenght = 0.5f;
    [SerializeField] private float _bounceForce = 60.0f, _dangerTime = 6.0f;
    [SerializeField] private float _distanceToKillBehind = 10.0f;

    private string _playerTag = "Player";
    private float _playerYPos;
    private Camera _camera;
    private List<Transform> _targetPositions;
    private Transform _targetPos, _previousTarget;
    private float _previousPositionX;
    private NavMeshPath _path;
    private int _currentCornerIndex;
    private bool _isInitialized = false;

    [SerializeField] private bool _isDebugMessagesOn;

    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        _playerTag = PlayerSetupManager.Instance.PlayerPrefab.tag;
        //_camera = CameraManager.Instance.MainCam; replace camera
        StartCoroutine(DelayedInitialization());
    }
    private void FixedUpdate()
    {
        if (!_isChasing)
            MoveDeathline();
            //MoveDeathlineAgent();
        else
            ChaseAfterPlayers();

        KillPlayers();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            //StartCoroutine(CameraManager.Instance.CameraShake(1f, 0f));
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();
            player.Data.DangerCounter++;
            player.Data.DangerTime = _dangerTime;
            player.PlayerWorldUI.ChangeEnergyColor(Color.red);
            player.Data.DeathLineHitVFX.transform.rotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
            player.Data.DeathLineHitVFX.Play();

            if (player.Data.DangerCounter > _maxDangerCount)
                player.Controller.IsAlive = false;
            else
                player.Controller.Rb.AddForce(transform.transform.forward * _bounceForce, ForceMode.Impulse);
        }
    }
    private IEnumerator DelayedInitialization()
    {
        yield return new WaitForSeconds(1);

        //List<GameObject> tempTracks = RandomTracksTemplate.Instance.Tracks;
        //_targetPositions = RandomTracksTemplate.Instance.NavPoints;
        List<Transform> tempTracks = RandomTracksTemplate.Instance.NavPoints;
        _targetPositions = new List<Transform>(tempTracks.Count);

        for (int i = 0; i < tempTracks.Count; i++)
            _targetPositions.Add(tempTracks[i].transform);

        _targetPos = _targetPositions[0];
        _path = CalculatePath();
        _isInitialized = true;
    }
    private NavMeshPath CalculatePath()
    {
        if (!_targetPos)
            return null;

        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(transform.position, _targetPos.position + Vector3.up, NavMesh.AllAreas, path))
        {
            return path;
        }
        else
        {
            Debug.Log("Path calculation failed.");
            return null;
        }
    }
    private void SetPath(NavMeshPath path)
    {
        _path = path;
        _currentCornerIndex = 0;
    }
    private void RotateTowardsDirection(Vector3 direction)
    {
        // Calculate the rotation angle based on the movement direction
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // Create the target rotation quaternion
        Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

        // Smoothly rotate the object towards the target rotation
        float rotationSpeed = _speed * _rotationFactor;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
    /*private void MoveDeathline2()
    {
        if (!_isMoving || !_isInitialized || _targetPos is null || _targetPositions.Count < 2) return;

        Vector3 targetPos = _targetPos.position;
        targetPos.y = _height;

        Vector3 lastPlayerPlace = PlayerManager.Instance.PlayerPlacement[PlayerManager.Instance.PlayerPlacement.Count - 1].transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, lastPlayerPlace);

        if (distanceToTarget >= _maxDistanceFromPlayers)
        {
            _isChasing = true;
            return;
        }

        if (_path == null || _currentCornerIndex >= _path.corners.Length)
            return;

        Vector3 targetPosition = _path.corners[_currentCornerIndex];
        targetPosition.y = _height; // Set the specific height

        // Move towards the next corner with a constant speed
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.fixedDeltaTime);

        if (transform.position == targetPosition)
            _currentCornerIndex++;

        Vector3 direction = (_targetPos.position - transform.position).normalized;
        RotateTowardsDirection(direction);

        bool someonePassedCheckpoint = false;
        for (int i = 0; i < PlayerManager.Instance.AllPlayersAlive.Count; i++)
        {
            if (PlayerManager.Instance.AllPlayersAlive[i].transform.position.z > _targetPositions[0].position.z)
            {
                someonePassedCheckpoint = true;
                break;
            }
        }

        if (!someonePassedCheckpoint) return;

        _targetPositions.RemoveAt(0);

        if (_targetPositions[0] is not null)
        {
            _targetPos = _targetPositions[0];
            CalculatePath();

            if (_isDebugMessagesOn) Debug.Log(_targetPositions[0] + "targets:" + _targetPositions.Count);
        }
    }*/
    private void MoveDeathline()
    {
        if (!_isMoving || !_isInitialized || _targetPos is null || _targetPositions.Count < 2) return;

        Vector3 targetPos = _targetPos.position;
        targetPos.y = _height;

        if (_targetPositions[0] is not null)
        {
            _previousTarget = _targetPos;
            _targetPos = _targetPositions[0];

            if (_isDebugMessagesOn) Debug.Log(_targetPositions[0] + "targets:" + _targetPositions.Count);
        }

        Vector3 lastPlayerPlace = LaserRushGameMode.Instance.PlayerPlacement[LaserRushGameMode.Instance.PlayerPlacement.Count - 1].transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, lastPlayerPlace);

        if (distanceToTarget >= _maxDistanceFromPlayers)
        {
            _isChasing = true;
            return;
        }

        float scaledSpeed = _speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, scaledSpeed);

        if (_targetPos.position.x != transform.position.x) 
        {
            // make deathline rotate
            Vector3 direction = (_targetPos.position - transform.position).normalized;
            RotateTowardsDirection(direction);
        }

        bool someonePassedCheckpoint = false;
        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        for (int i = 0; i < allPlayersAlive.Count; i++)
        {
            if (allPlayersAlive[i].transform.position.z > _targetPositions[0].position.z)
            {
                someonePassedCheckpoint = true;
                break;
            }
        }

        if (!someonePassedCheckpoint) return;

        _targetPositions.RemoveAt(0);
    }
    private void ChaseAfterPlayers()
    {
        if (!_isMoving || !_isInitialized || _targetPos is null || _targetPositions.Count < 2) return;

        int lastPlayerPlace = LaserRushGameMode.Instance.PlayerPlacement.Count - 1;
        Vector3 targetPos = LaserRushGameMode.Instance.PlayerPlacement[lastPlayerPlace].transform.position;
        targetPos.y = _height;

        if (_targetPositions[0] is not null)
        {
            _previousTarget = _targetPos;
            _targetPos = _targetPositions[0];
            _path = CalculatePath();

            if (_isDebugMessagesOn) Debug.Log(_targetPositions[0] + "targets:" + _targetPositions.Count);
        }

        float distanceToTarget = Vector3.Distance(transform.position, targetPos);

        if (distanceToTarget <= _chasingThreshold)
        {
            _isChasing = false;
            return;
        }

        // following player
        transform.position = Vector3.MoveTowards(transform.position, _targetPos.position, _speed * _speedFactor * Time.fixedDeltaTime);

        if (_targetPos.rotation.y != transform.rotation.y)
        {
            // make deathline rotate
            Vector3 direction = (_targetPos.position - transform.position).normalized;
            RotateTowardsDirection(direction);
        }

        /*if (_previousTarget is not null && _targetPos.rotation.y != _previousTarget.rotation.y)
        {
            Vector3 addedForce = Vector3.zero;
            addedForce.z += _turnPower;
            transform.position += addedForce;
        }*/

        if (transform.position.z > _targetPositions[0].position.z)
            _targetPositions.RemoveAt(0);
    }
    private void KillPlayers()
    {
        List<PlayerInputHandler> allPlayersAlive = PlayerManager.Instance.AllPlayersAlive;
        for (int i = 0; i < allPlayersAlive.Count; i++)
        {
            if (allPlayersAlive[i].transform.position.z < transform.position.z - _distanceToKillBehind)
                allPlayersAlive[i].Controller.IsAlive = false;
        }
    }

    private void MoveDeathlineAgent()
    {
        if (!_isMoving || !_isInitialized || _targetPos is null || _targetPositions.Count < 2) return;

        Vector3 targetPos = _targetPos.position;
        targetPos.y = _height;


        if (_targetPositions[0] is not null)
        {
            _previousTarget = _targetPos;
            _targetPos = _targetPositions[0];
            _path = CalculatePath();

            if (_isDebugMessagesOn) Debug.Log(_targetPositions[0] + "targets:" + _targetPositions.Count);
        }

        Vector3 lastPlayerPlace = LaserRushGameMode.Instance.PlayerPlacement[LaserRushGameMode.Instance.PlayerPlacement.Count - 1].transform.position;
        float distanceToTarget = Vector3.Distance(transform.position, lastPlayerPlace);

        if (distanceToTarget >= _maxDistanceFromPlayers)
        {
            _isChasing = true;
            return;
        }

        float scaledSpeed = _speed * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, scaledSpeed);

        if (_targetPos.position.x != transform.position.x)
        {
            // make deathline rotate

            /*Vector3 direction = (_targetPos.position - transform.position).normalized;
            RotateTowardsDirection(direction);*/
        }

        bool someonePassedCheckpoint = false;
        for (int i = 0; i < PlayerManager.Instance.AllPlayersAlive.Count; i++)
        {
            if (PlayerManager.Instance.AllPlayersAlive[i].transform.position.z > _targetPositions[0].position.z)
            {
                someonePassedCheckpoint = true;
                break;
            }
        }

        if (!someonePassedCheckpoint) return;

        _targetPositions.RemoveAt(0);
    }

    public void StartDeathline()
    {
        _isMoving = true;
    }
    public void StopDeathline()
    {
        _isMoving = false;
    }
}
