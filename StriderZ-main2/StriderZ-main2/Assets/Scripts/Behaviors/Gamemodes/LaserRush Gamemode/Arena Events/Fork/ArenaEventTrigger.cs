using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaEventTrigger : MonoBehaviour
{
    [SerializeField] private ForkManager _forkManager;
    public ForkManager ForkManager => _forkManager;

    [SerializeField] private LaserRushLateGameType _arenaEventType;
    [SerializeField] private GameObject particleEffectObject;
    [SerializeField] private GameObject _endPoint;
    [SerializeField] private bool _isEndless = false;

    private bool _isPlayerPassed = false;
    public bool IsPlayerPassed => _isPlayerPassed;

    private string _playerTag = "Player";

    private void Start()
    {
        _playerTag = PlayerManager.Instance.PlayerTag;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_forkManager.FirstPlayerToPass != null)
            return;

        if (_isEndless && other.CompareTag(_playerTag))
        {
            RandomTracksTemplate.Instance.Tracks.Clear();
            RandomTracksTemplate.Instance.fork = false;
            _endPoint.SetActive(true);
            return;
        }
        else if (!_isPlayerPassed && other.CompareTag(_playerTag))
        {
            PlayerInputHandler player = other.GetComponent<PlayerInputHandler>();
            particleEffectObject.SetActive(true);
            _isPlayerPassed = true;
            _forkManager.TriggerGameModeLate(player, _arenaEventType, _forkManager);
            return;
        }
    }
}
