using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowPlayer : MonoBehaviour
{
    [SerializeField] private float _maxSlowVelocityMagnitude = 10f, _slowTimeInSeconds = 2f;
    [SerializeField] private bool _clampVelocityMagnitude = false;
    
    private const string _playerTag = "Player";
    private Rigidbody _playerRb;

    private void FixedUpdate()
    {
        // used to be in Update
        ClampPlayerVelocityMagnitude();
    }
    private void OnTriggerEnter(Collider other)
    {
        // check if trigger is with player
        if (other.gameObject.CompareTag(_playerTag))
        {
            // get player's rigidbody
            _playerRb = other.gameObject.GetComponent<Rigidbody>();

            // start slow coroutine
            StartCoroutine(SlowPlayerForSeconds(_slowTimeInSeconds));
        }
    }

    private void ClampPlayerVelocityMagnitude()
    {
        // if there is no need for clamp or rigidbody don't exist, return
        if (!_clampVelocityMagnitude || !_playerRb) 
            return;

        // (implied else) if player's velocity is over our maximum value to clamp
        if (_playerRb.velocity.magnitude > _maxSlowVelocityMagnitude) 
            _playerRb.velocity = Vector3.ClampMagnitude(_playerRb.velocity, _maxSlowVelocityMagnitude); // force velocity down to the max value to clamp
    }

    private IEnumerator SlowPlayerForSeconds(float seconds)
    {
        if (_playerRb)
            _clampVelocityMagnitude = true; // clamp velocity
        else
            StopCoroutine(SlowPlayerForSeconds(0)); // stop slow coroutine if rigidbody don't exist

        yield return new WaitForSeconds(seconds);

        // get rid of player's rigidbody
        _playerRb = null;

        // stop clamping velocity
        _clampVelocityMagnitude = false; 

        StopCoroutine(SlowPlayerForSeconds(0)); // stop slow coroutine
    }
}
