using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GrappleController _grappleController;
    [SerializeField] private float _stunTime = 2f;

    private int _useCount = 3;

    private void OnEnable()
    {
        //EventManager.OnStun += OnPlayerStunned;
    }
    private void OnDisable()
    {
        //EventManager.OnStun -= OnPlayerStunned;
    }

    public void OnPlayerStunned(Rigidbody playerRigidbody, PlayerController playerController)
    {
        if (playerRigidbody.gameObject != _playerController.gameObject || _useCount == 0)
            return;

        PlayerInputHandler player = playerController.GetComponent<PlayerInputHandler>();

        _useCount--;
        //player.Data.StunCharges = _useCount;
        //UIManager.Instance.RemoveStunChargeFromPlayer(player.SetupData.ID);
        //UIManager.Instance.AllPlayerChargesUI[_playerController.PlayerData.PlayerID][_useCount].enabled = false;
        //UIManager.Instance.RemoveStunChargeFromPlayer(player.Config.ID);
        SoundManager.Instance.PlayStunSound(SoundManager.Instance.StunHitSounds);
        _playerController.IsStunned = true;
        _grappleController.enabled = false;
        _playerController.Rb.velocity = Vector3.zero;
        _playerController.InputHandler.Data.StunVFX.Play();
        StartCoroutine(DoStun(_stunTime));
    }

    private IEnumerator DoStun(float sec)
    {
        _playerController.Rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(sec);

        _playerController.IsStunned = false;
        _grappleController.enabled = true;
    }
}
