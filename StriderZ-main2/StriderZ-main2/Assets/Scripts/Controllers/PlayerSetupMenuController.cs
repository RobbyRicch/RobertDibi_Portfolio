using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.UI;
using TMPro;

public class PlayerSetupMenuController : MonoBehaviour
{
    #region Data
    [Header("UI Interactions")]
    [SerializeField] private MultiplayerEventSystem _currentMultiplayerEventSystem;

    [SerializeField] private InputSystemUIInputModule _playerInputModule;
    public InputSystemUIInputModule PlayerInputModule => _playerInputModule;

    [Header("UI Components")]
    [SerializeField] private Image _helmetIcon;

    [Header("Bot objects")]
    [SerializeField] private GameObject[] _setupModels;
    public GameObject[] SetupModels => _setupModels;

    [Header("Setup bot mesh")]
    [SerializeField] private ModelData _modelData;
    public ModelData ModelData => _modelData;

    [Header("Ready up components & data")]
    [SerializeField] private Button _playerReadyBtn/*, _colorBtn*/;
    [SerializeField] private TextMeshProUGUI _playerTitleTMPro, _playerReadyTMPro;
    [SerializeField] private string _readyText = "Ready!", _unreadyText = "Ready Up";
    [SerializeField] private string _nickName = "P 0";

    [SerializeField] private int _playerIndex;
    public int PlayerIndex { get => _playerIndex; set => _playerIndex = value; }

    private PlayerSetupData _playerSetupData;
    private float _ignoreInputTime = 0.5f;
    private bool _isInputEnabled = false;
    #endregion

    #region Monobehaviour Callbacks
    private void Start()
    {
        _playerSetupData = PlayerSetupManager.Instance.AllPlayersSetupData[_playerIndex];
        _playerTitleTMPro.text = _playerSetupData.Nickname;
        _playerReadyTMPro.text = _unreadyText;
        _playerReadyTMPro.color = Color.red;
    }
    private void Update()
    {
        if (!_playerSetupData.IsSetupDone && _playerReadyTMPro.text == _readyText)
            _playerReadyTMPro.text = _unreadyText;

        if (Time.time > _ignoreInputTime)
            _isInputEnabled = true;
    }
    #endregion

    #region Tools
    /*private void SetUIBtnColor(Color color)
    {
        _colorBtn.image.color = color;
    }*/
    private void ApplyColorsToSetupBot(ColorData colorData)
    {
        #region Helmet
        // ui helmet icon
        _helmetIcon.color = colorData.BaseEmissionColor;

        // base material
        _modelData.HelmetMesh.materials[0].color = colorData.BaseBaseColor;
        _modelData.HelmetMesh.materials[0].SetColor("_EmissionColor", colorData.BaseBaseColor);

        // emission material
        _modelData.HelmetMesh.materials[1].color = colorData.BaseEmissionColor;
        _modelData.HelmetMesh.materials[1].SetColor("_EmissionColor", colorData.BaseEmissionColor);

        // detail material
        //_modelData.HelmetMesh.materials[2].color = colorData.BaseDetailColor;
        //_modelData.HelmetMesh.materials[2].SetColor("_EmissionColor", colorData.BaseDetailColor);

        // Screen material
        _modelData.HelmetMesh.materials[3].SetColor("_FaceLineColor", colorData.FaceColor);
        #endregion

        #region Body
        // base material
        _modelData.BodyMesh.materials[0].color = colorData.BaseBaseColor;
        _modelData.BodyMesh.materials[0].SetColor("_EmissionColor", colorData.BaseBaseColor);

        // emission material
        _modelData.BodyMesh.materials[1].color = colorData.BaseEmissionColor;
        _modelData.BodyMesh.materials[1].SetColor("_EmissionColor", colorData.BaseEmissionColor);

        // detail material
        //_modelData.BodyMesh.materials[2].color = colorData.BaseDetailColor;
        //_modelData.BodyMesh.materials[2].SetColor("_EmissionColor", colorData.BaseDetailColor);
        #endregion

        #region Hands
        for (int i = 0; i < _modelData.LeftHandMeshes.Length; i++)
        {
            switch (i)
            {
                case 0:
                    // left
                    _modelData.LeftHandMeshes[i].materials[0].color = colorData.BaseBaseColor;
                    _modelData.LeftHandMeshes[i].materials[1].color = colorData.BaseEmissionColor;
                    _modelData.LeftHandMeshes[i].materials[1].SetColor("_EmissionColor", colorData.BaseEmissionColor);
                    //_modelData.LeftHandMeshes[i].materials[2].color = colorData.BaseDetailColor;
                    //_modelData.LeftHandMeshes[i].materials[2].SetColor("_EmissionColor", colorData.BaseDetailColor);

                    // right
                    _modelData.RightHandMeshes[i].materials[0].color = colorData.BaseBaseColor;
                    _modelData.RightHandMeshes[i].materials[1].color = colorData.BaseEmissionColor;
                    _modelData.RightHandMeshes[i].materials[1].SetColor("_EmissionColor", colorData.BaseEmissionColor);
                    //_modelData.RightHandMeshes[i].materials[2].color = colorData.BaseDetailColor;
                    //_modelData.RightHandMeshes[i].materials[2].SetColor("_EmissionColor", colorData.BaseDetailColor);
                    break;

                default:
                    _modelData.LeftHandMeshes[i].material.color = colorData.BaseBaseColor; // left
                    _modelData.RightHandMeshes[i].material.color = colorData.BaseBaseColor; // right
                    break;
            }
        }
        #endregion
    }
    private void ApplyModelToSetupBot(PlayerSetupData playerSetupData)
    {
        for (int i = 0; i < _setupModels.Length; i++)
        {
            _setupModels[i].SetActive(false);

            if (i == (int)playerSetupData.ChosenModelType)
            {
                GameObject activeSetupModel = _setupModels[i].gameObject;
                activeSetupModel.SetActive(true);
                _modelData = activeSetupModel.GetComponent<ModelData>();
                _helmetIcon.sprite = playerSetupData.HelmetSprite;
                playerSetupData.UserPic = playerSetupData.HelmetSprite; // temp until setting up accounts
            }
        }

        ApplyColorsToSetupBot(playerSetupData.ColorData);
    }
    private void ApplyColorsToUIElements(PlayerSetupData playerSetupData, ColorData colorData)
    {
        //Transform playerIcon = UIManager.Instance.PlayerLogInLayOut.GetChild(playerSetupData.ID);
        //playerIcon.GetComponent<Image>().color = colorData.BaseEmissionColor;
        //SetUIBtnColor(colorData.EmissionColor);
    }
    private void CycleColorsOnPlayer(PlayerSetupData playerSetupData)
    {
        /* apply to player setup data */
        PlayerSetupManager.Instance.CycleNextColor(_playerSetupData);
        ColorData colorData = playerSetupData.ColorData;

        ApplyColorsToSetupBot(colorData); 
        ApplyColorsToUIElements(playerSetupData, colorData);
    }
    private void CycleModelsOnPlayer(PlayerSetupData playerSetupData, bool isInitialized)
    {
        /* apply to player setup data */
        PlayerSetupManager.Instance.CycleNextModel(playerSetupData, isInitialized);
        ApplyModelToSetupBot(playerSetupData);
    }
    private void InitializePlayerSetupData(PlayerSetupData playerSetupData, bool isInitialized)
    {
        CycleColorsOnPlayer(playerSetupData);
        CycleModelsOnPlayer(playerSetupData, isInitialized);
    }
    #endregion

    #region Initialization
    public void InitializeNewPlayer()
    {
        _playerSetupData = PlayerSetupManager.Instance.AllPlayersSetupData[_playerIndex];
        _playerSetupData.ColorData ??= PlayerSetupManager.Instance.AllColors[0]; // "??=" equals to "is null" equals to "ReferenceEquals(x, null)"
        InitializePlayerSetupData(_playerSetupData, false);
    }
    public void InitializeExistingPlayer()
    {
        ApplyModelToSetupBot(_playerSetupData);
    }
    #endregion

    #region Unity Events
    public void SetPlayerColorsFromUI()
    {
        if (!_isInputEnabled)
            return;

        CycleColorsOnPlayer(_playerSetupData);
    }
    public void SetPlayerModelFromUI()
    {
        if (!_isInputEnabled)
            return;

        CycleModelsOnPlayer(_playerSetupData, false);
    }
    #endregion

    #region Ready
    public void ReadyUp()
    {
        if (!_isInputEnabled)
            return;

        if (!_playerSetupData.IsSetupDone)
        {
            PlayerSetupManager.Instance.ReadyUp(_playerIndex);
            _playerReadyTMPro.text = _readyText;
            _playerReadyTMPro.color = Color.green;
        }
        else
        {
            PlayerSetupManager.Instance.UnReady(_playerIndex);
            _playerReadyTMPro.text = _unreadyText;
            _playerReadyTMPro.color = Color.red;
        }
        
        //_playerReadyBtn.onClick.RemoveAllListeners();
        //_playerReadyBtn.onClick.AddListener(UnReadyPlayer);
    }
    public void UnReadyPlayer()
    {
        if (!_isInputEnabled)
            return;

        _playerReadyBtn.onClick.RemoveAllListeners();
        _playerReadyBtn.onClick.AddListener(ReadyUp);
    }
    #endregion
}
