using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonBall : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerMotor _playerMotor;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private AnimationManager animationManager;
    [SerializeField] private ManaSystem _manaSystem;
    [SerializeField] private int _manaCost = 10;

    [SerializeField] private float trajectoryHeight = 5f;
    [SerializeField] private float _coolDown = 3f;
    [SerializeField] private float _canonBallTimeLength = 0.3f;
    [SerializeField] private float _aOERadius = 3;
    private float _cooldownTimer = 0;
    private float _canonBalltimer = 0;
    private Vector3 _spawnPos;
    private Vector3 _hitPos;
    private bool _visualizing = false;
    [SerializeField] private bool isCanonBalling = false;
    [SerializeField] private float _CanonBallMaxHeight = 5;
    [SerializeField] private float _Range;
    [SerializeField] private float maxCanonBallAngle = 30f;
    [SerializeField] private float minCanonBallAngle = 30f;
    [SerializeField] private LayerMask _layerToHit;
    //[SerializeField] GameObject CanonBallUI;
    [SerializeField] GameObject CanonBallIndicator;
    private GameObject _tempVisualizerObj;
    private bool GroundHit = false;

    // Public variables
    public float rocketForce = 100f;
    public float rocketDuration = 0.2f;
    public float jumpForce = 10f;
    [SerializeField] private Transform Player;

    float distance;
    float timeToReachPoint;
    public float countdownSpeed = 1f;
    Vector3 currentPlayerPosition;
    public bool jumped = false;
    private bool rocketJumped = false;
    [SerializeField] private GameObject RocketJumpParticle;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_cooldownTimer >= _coolDown && _inputManager.FireCanonBallHold && _manaSystem.CurrentMana >= _manaCost)
        {
            CanonBallZone();
            //CanonBallUI.SetActive(true);
            //Time.timeScale = 0.5f;
        }
        else if (_cooldownTimer >= _coolDown && _inputManager.FireCanonBallHoldCanceled /*&& _visualizing*//*//**&& animationManager.LightningDashLoaded*/)
        {
            if (!isCanonBalling)
            {
                if (_manaSystem.Decrease(_manaCost))
                {
                    CancelCanonBallZone();
                    
                    OnStartCanonBall();
                    if (GroundHit)
                        distance = Vector3.Distance(Player.position, _spawnPos);
                    else
                        distance = Vector3.Distance(Player.position, _hitPos);

                    timeToReachPoint = distance / countdownSpeed;
                    currentPlayerPosition = Player.position;
                    RocketJump();
                    //SoundManager.Instance.PlaySound(SoundManager.SoundType.LightningDash);
                }
            }
        }
        else
        {
            _cooldownTimer += Time.deltaTime;
            _inputManager.FireCanonBallHoldCanceled = false;
        }

        if (_playerMotor.Landed && jumped)
        {
            if (rocketJumped)
            {
                rocketJumped = false;
                RocketJumpParticle.SetActive(true);
                StartCoroutine(RocketJumpParticleOff());
            }
            _playerMotor.playerVelocity = Vector3.zero;
            jumped = false;
            OnEndCanonBall();
        }
    }

    private void RocketJump()
    {
        jumped = true;
        rocketJumped = true;
        Vector3 newDir;
        if (GroundHit)
            newDir = _spawnPos;
        else
            newDir = _hitPos;

        _playerMotor.playerVelocity = CalculateJumpVelocity(currentPlayerPosition, newDir, trajectoryHeight);
        //_playerMotor.RocketJumpSpeed = 3f;
        SoundManager.Instance.PlaySound(SoundManager.SoundType.FireJump);

    }
    IEnumerator RocketJumpParticleOff()
    {
        yield return new WaitForSeconds(0.2f);
        RocketJumpParticle.SetActive(false);
    }
    void OnStartCanonBall()
    {
        _playerMotor.Jumped = true;
        _playerMotor.Landed = false;
        //Time.timeScale = 1f;
        isCanonBalling = true;
        //DashBoxVolume.SetActive(true);
        //CanonBallUI.SetActive(true);
        //PlayDashParicles();
        //_inputManager.DashPress = false;
        //_manaHandler.UseManaOnce(_manaCost);
    }
    void OnEndCanonBall()
    {
        _cooldownTimer = 0;
        _canonBalltimer = 0;
        isCanonBalling = false;
        //DashBoxVolume.SetActive(false);
        //CanonBallUI.SetActive(false);
        //_playerMotor.EnableGravity(true);
        _playerMotor.RocketJumpSpeed = 1f;
        _playerMotor.playerVelocity = Vector3.zero;
    }
    void CanonBallZone()
    {
        SetSpawnLocation();
        if (!_visualizing)
        {
            _visualizing = true;
            _tempVisualizerObj = Instantiate(CanonBallIndicator, _spawnPos, Quaternion.identity);
            _tempVisualizerObj.transform.SetParent(null);
            _tempVisualizerObj.transform.localScale = new Vector3(_aOERadius, _tempVisualizerObj.transform.localScale.y, _aOERadius);
        }
        if (GroundHit)
        {
            _tempVisualizerObj.transform.position = _spawnPos;
        }
        else
        {
            SetSpawnHeight(_spawnPos);
            _tempVisualizerObj.transform.position = _hitPos;
        }
    }
    private void CancelCanonBallZone()
    {
        _visualizing = false;
        Destroy(_tempVisualizerObj);
    }
    private void SetSpawnLocation()
    {
        RaycastHit hit;
        Ray ray = new Ray(_mainCamera.transform.position, _mainCamera.transform.forward);
        if (Physics.Raycast(ray, out hit, _Range, _layerToHit))
        {
            _spawnPos = hit.point;
            GroundHit = true;
        }
        else
        {
            _spawnPos = ray.GetPoint(_Range);
            GroundHit = false;
        }

        Debug.DrawLine(ray.origin, _spawnPos, Color.cyan, 3);
    }
    private void SetSpawnHeight(Vector3 pos)
    {
        RaycastHit hit;
        Ray ray = new Ray(pos, Vector3.up * -1);
        if (Physics.Raycast(ray, out hit, 100, _layerToHit))
        {
            _spawnPos = hit.point + new Vector3(0, _CanonBallMaxHeight, 0);
            _hitPos = hit.point;
            //Debug.Log(_spawnPos);
        }
        else
        {
            _spawnPos += new Vector3(0, _CanonBallMaxHeight, 0);
            _hitPos = _spawnPos - new Vector3(0, _CanonBallMaxHeight, 0);
            //Debug.Log("2");
        }

        Debug.DrawLine(ray.origin, _spawnPos, Color.yellow, 3);
        Debug.DrawLine(_spawnPos, _hitPos, Color.green, 3);
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = _playerMotor.Gravity;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(4 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private void OnValidate()
    {
        _inputManager = GetComponentInParent<InputManager>();
        _controller = GetComponentInParent<CharacterController>();
        _playerMotor = GetComponentInParent<PlayerMotor>();
    }
}
