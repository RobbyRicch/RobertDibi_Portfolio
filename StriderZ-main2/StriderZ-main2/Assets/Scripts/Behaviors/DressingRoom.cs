using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Cinemachine;

public class DressingRoom : MonoBehaviour
{
    [SerializeField] private MultiplayerEventSystem _multiplayerEventSystem;
    [SerializeField] private Image _modelIconImage, _colorPalletImage;
    [SerializeField] private SpriteRenderer _hangerSprite;
    public Image ModeIconImage => _modelIconImage;
    public Image ColorPalletImage => _colorPalletImage;

    [SerializeField] private float _originalCamDistance = 66.0f, _newCamDistance = 40.0f;
    [SerializeField] private float _playerRotationsSpeed = 100.0f;
    [SerializeField] private Transform _padTr;
    [SerializeField] private Transform _offTr, _onTr;
    [SerializeField] private bool _isBusy;

    private InputSystemUIInputModule _lobbyUiInputModule;
    private InputSystemUIInputModule _tempUiInputModule;
    private PlayerInputHandler _usingPlayer;
    private PlayerInput _usingPlayerInput;
    private Vector3 _targetPlayerPos = Vector3.zero;
    private Vector3 _targetPlayerRot = Vector3.zero;
    private const string _playerTag = "Player";

    private void Start()
    {
        _lobbyUiInputModule = LobbyManager.Instance.gameObject.GetComponent<InputSystemUIInputModule>();

        Vector3 newPlayerPos = transform.position;
        newPlayerPos.y += 1.5f;
        _targetPlayerPos = newPlayerPos;

        Vector3 newPlayerRot = Quaternion.identity.eulerAngles;
        newPlayerRot.y = 180.0f;
        _targetPlayerRot = newPlayerRot;
    }
    private void Update()
    {
        if (_usingPlayer && _usingPlayer.transform.position != _targetPlayerPos)
        {
            _usingPlayer.Controller.Rb.angularVelocity = Vector3.zero;
            _usingPlayer.Controller.Rb.velocity = Vector3.zero;
            _usingPlayer.transform.position = _targetPlayerPos;

            Vector3 newRotation = _usingPlayer.transform.rotation.eulerAngles;
            newRotation.y += _playerRotationsSpeed * Time.deltaTime;
            _usingPlayer.Controller.Rb.rotation = Quaternion.Euler(newRotation);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag) && !_isBusy)
        {
            /* set members */
            _usingPlayer = other.gameObject.GetComponent<PlayerInputHandler>();
            _usingPlayerInput = PlayerSetupManager.Instance.transform.GetChild(_usingPlayer.Data.ID).GetComponent<PlayerInput>();
            LobbyManager.Instance.AllChangingPlayers.Add(_usingPlayer);

            /* set input module */
            _tempUiInputModule = _multiplayerEventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            _tempUiInputModule.move = _lobbyUiInputModule.move;
            _tempUiInputModule.submit = _lobbyUiInputModule.submit;
            _tempUiInputModule.cancel = _lobbyUiInputModule.cancel;
            _multiplayerEventSystem.firstSelectedGameObject = _modelIconImage.gameObject;

            /* disable movement */
            _usingPlayer.IsPlayerInputsDisable = true;
            _usingPlayer.Attractor.CancelAttractorLeft(true);
            _usingPlayer.Attractor.CancelAttractorRight(true);
            _usingPlayer.Attractor.DisableAttractor(true);
            _usingPlayer.Controller.Rb.angularVelocity = Vector3.zero;
            _usingPlayer.Controller.Rb.velocity = Vector3.zero;
            _usingPlayer.transform.position = _targetPlayerPos;

            _usingPlayerInput.uiInputModule = _tempUiInputModule; // assign ui module to player

            _padTr.position = _onTr.position; // visual represenation

            /* set dressing room ui */
            _modelIconImage.sprite = _usingPlayer.Data.ModelData.IconImage.sprite;
            _colorPalletImage.sprite = _usingPlayer.SetupData.ColorData.PalletSprite;
            _hangerSprite.enabled = false;

            _multiplayerEventSystem.gameObject.SetActive(true); // enable current multiplayer event system

            /* camera zoom if not zoomed */
            if (CinemachineManager.Instance.VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance != _newCamDistance)
                CinemachineManager.Instance.VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = _newCamDistance;

            _isBusy = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag) && _isBusy)
        {
            StopChanging();

        }
    }

    public void ChangeModel()
    {
        if (!_usingPlayer)
            return;

        LobbyManager.Instance.CycleModelsOnPlayer(_usingPlayer.SetupData, true);
        _modelIconImage.sprite = _usingPlayer.Data.ModelData.IconImage.sprite;
    }
    public void ChangeColorPallet()
    {
        if (!_usingPlayer)
            return;

        Sprite colorPallet = PlayerSetupManager.Instance.AllColors[0].PalletSprite;

        LobbyManager.Instance.CycleColorsOnPlayer(_usingPlayer.SetupData);
        _colorPalletImage.sprite = colorPallet;
    }
    public void ChangeTrail()
    {
        _usingPlayer.Controller.IsChangingModel = false;
        _usingPlayer.Controller.IsChangingPallet = false;
        _usingPlayer.Controller.IsChangingTrail = true;
    }

    public void StopChanging()
    {
        if (LobbyManager.Instance.AllChangingPlayers.Count == 1)
        {
            CinemachineManager.Instance.VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_CameraDistance = _originalCamDistance;
        }

            _hangerSprite.enabled = true;
        LobbyManager.Instance.AllChangingPlayers.Remove(_usingPlayer);

        _usingPlayerInput.uiInputModule = null;
        _usingPlayerInput = null;

        _usingPlayer.IsPlayerInputsDisable = false;
        _usingPlayer.Attractor.DisableAttractor(false);
        _usingPlayer = null;
        Destroy(_tempUiInputModule);

        _padTr.position = _offTr.position;

        _multiplayerEventSystem.gameObject.SetActive(false);
        _isBusy = false;
    }
}
