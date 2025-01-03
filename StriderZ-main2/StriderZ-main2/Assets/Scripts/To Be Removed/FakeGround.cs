using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FakeGround : MonoBehaviour
{
    [SerializeField] private float _minVelocityToPass = 22.5f;
    [SerializeField] private Collider _collider;

    private float _playerHeight = 1.5f, _playerHeightOffset = 0.1f;

    private List<Rigidbody> _playersRb = new();

    private const string _playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            if (other.TryGetComponent(out Rigidbody playerRb))
                DisablePlayerGravityOnTrigger(playerRb);
        }
        /* Old Behavior
         * DisableFallThroughOnTrigger(other);
         */
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            foreach (Rigidbody playerRb in _playersRb)
            {
                if (other.CompareTag(playerRb.tag))
                {
                    ChangePlayerGravityUseByVelocity(playerRb);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            foreach (Rigidbody playerRb in _playersRb)
            {
                if (other.CompareTag(playerRb.tag))
                {
                    DisableGravityUseOnExitTrigger(playerRb);
                }
            }
        }
    }

    /* Collisions
    private void OnCollisionStay(Collision collision)
    {
        ChangeFallThroughStateOnCollisionStay(collision.collider);
    }
    private void OnCollisionExit(Collision collision)
    {
        EnableFallThroughOnCollisionExit(collision.collider);   
    }
    */

    #region Old Behavior
    private void DisableFallThroughOnTrigger(Collider other)
    {
        if (other.CompareTag(_playerTag))
            if (other.GetComponent<Rigidbody>().velocity.x > _minVelocityToPass)
                _collider.isTrigger = false;
    }
    private void ChangeFallThroughStateOnCollisionStay(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            if (other.GetComponent<Rigidbody>().velocity.x > _minVelocityToPass)
                _collider.isTrigger = false;
            else
                _collider.isTrigger = true;
        }
    }
    private void EnableFallThroughOnCollisionExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _collider.isTrigger = true;
        }
    }
    #endregion

    #region New Behavior
    private void DisablePlayerGravityOnTrigger(Rigidbody playerRb)
    {
        _playersRb.Add(playerRb);

        if (playerRb.velocity.x < _minVelocityToPass)
            playerRb.useGravity = true;
    }
    private void ChangePlayerGravityUseByVelocity(Rigidbody playerRb)
    {
        if (playerRb.velocity.x > _minVelocityToPass)
        {
            playerRb.useGravity = false;
            playerRb.transform.position = new(playerRb.transform.position.x, _playerHeight + _playerHeightOffset, playerRb.transform.position.z);
        }
        else
        {
            playerRb.useGravity = true;
        }
    }
    private void DisableGravityUseOnExitTrigger(Rigidbody playerRb)
    {
        playerRb.useGravity = false;
        _playersRb.Remove(playerRb);
    }
    #endregion
}
