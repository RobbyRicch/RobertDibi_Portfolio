using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveToGameMode : MonoBehaviour
{
    [SerializeField] private Transform _padTr;
    [SerializeField] private Transform _offTr, _onTr;
    [SerializeField] private GameObject _timerTextCanvas;
    [SerializeField] private TextMeshProUGUI _timerTextItem;
    [SerializeField] private MeshRenderer _topMesh;
    [SerializeField] private string _ready = "Ready!", _notReady = "Not Ready";
    [SerializeField] private bool _isBusy = false;

    [ColorUsage(true, true)]private Color _offColor;
    [ColorUsage(true, true)][SerializeField] private Color _onColor;

    private const string _playerTag = "Player";

    private string _timerText;
    private List<PlayerInputHandler> _readyPlayers;

    #region Monobehaviour Callbacks
    // Start is called before the first frame update
    private void Awake()
    {
        _readyPlayers = new List<PlayerInputHandler>();
        _offColor = _topMesh.material.GetColor("_Base_Color");
    }
    private void Start()
    {
        _padTr.position = _offTr.position;
    }
    private void Update()
    {
        _timerTextItem.text = SceneManagerMono.Instance.CurrentTimerTime.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isBusy)
            return;

        if (other.CompareTag(_playerTag) && other.GetType().ToString().Equals("UnityEngine.CapsuleCollider")) // need to change - detect only the right collider
        {
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();
            int playerID = player.Data.ID;
            _readyPlayers.Add(player);
            TextMeshProUGUI readyText = LobbyManager.Instance.ReadyTexts[playerID];
            readyText.text = _ready;
            UIManager.Instance.PopUIObject(readyText.transform, UIManager.Instance.PopTargetSize, UIManager.Instance.PopPeakSize);

            if (_readyPlayers.Count == PlayerSetupManager.Instance.AllPlayersSetupData.Count)
            {
                _timerTextCanvas.SetActive(true);
                _padTr.position = _onTr.position;
                _isBusy = true;
                _topMesh.material.SetColor("_Base_Color", _onColor);
                EventManager.InvokePlayersReady(PlayerManager.Instance.AllPlayers.ToArray());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag) && other.GetType().ToString().Equals("UnityEngine.CapsuleCollider"))
        {
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();
            int playerID = player.Data.ID;
            _readyPlayers.Remove(player);
            TextMeshProUGUI readyText = LobbyManager.Instance.ReadyTexts[playerID];
            readyText.text = _notReady;
            UIManager.Instance.UnPopUIObject(readyText.transform);
        }

        if (_readyPlayers.Count < PlayerSetupManager.Instance.AllPlayersSetupData.Count || _readyPlayers.Count == 0)
        {
            StopTimer();
        }
    }

    public void StopTimer()
    {
        SceneManagerMono.Instance.StopTimer();
        _isBusy = false;
        _padTr.position = _offTr.position;
        _timerTextCanvas.SetActive(false);
        _topMesh.material.SetColor("_Base_Color", _offColor);
    }
    #endregion
}
