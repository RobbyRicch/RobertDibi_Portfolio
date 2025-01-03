using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;

public class BusterRain_Attack : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float _timeToOpen;
    [SerializeField] private float _timeToClose;
    [SerializeField] private float _sizeAnimationDuration; // Duration to expand the collider

    [Header("Stats")]
    [SerializeField] private int _dmgPerTic;
    [SerializeField] private float _damageInterval; // Time between damage ticks
    [SerializeField] private float _cooldownTime;
    [SerializeField] private bool _onCooldown;
    [SerializeField] private bool _colliderIsOpen;

    [Header("References")]
    [SerializeField] private BoxCollider2D _attackCollider;
    [SerializeField] private bool _playerIsInRange;

    private Vector2 _originalSize;

    private void Awake()
    {
        // Save the original size of the collider
        _originalSize = _attackCollider.size;
        _attackCollider.size = Vector2.zero; // Start with size zero
        
    }

    void Start()
    {
        StartCoroutine(ExpandColliderRoutine());
        StartCoroutine(RainAttackRoutine());
    }

    private IEnumerator ExpandColliderRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < _sizeAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _sizeAnimationDuration;

            // Interpolate the size of the collider
            _attackCollider.size = Vector2.Lerp(Vector2.zero, _originalSize, t);
            yield return null;
        }

        // Ensure the final size is set
        _attackCollider.size = _originalSize;
        _colliderIsOpen = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerIsInRange = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_playerIsInRange && _attackCollider.enabled && !_onCooldown && _colliderIsOpen)
        {
            // Start dealing damage if the player is in range and not on cooldown
            StartCoroutine(DamageRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerIsInRange = false;
        }
    }

    private IEnumerator RainAttackRoutine()
    {
        yield return new WaitForSeconds(_timeToOpen);
        _attackCollider.enabled = true;
        yield return new WaitForSeconds(_timeToClose);
        _attackCollider.enabled = false;
    }

    private IEnumerator DamageRoutine()
    {
        // Immediately put the attack on cooldown
        _onCooldown = true;

        // Deal damage to the player
        DamagePlayer();

        // Wait for the damage interval before allowing another hit
        yield return new WaitForSeconds(_damageInterval);

        // Now, allow the player to be hit again after the cooldown time
        _onCooldown = false;
    }

    private void DamagePlayer()
    {
        // This is where you deal damage to the player
        Debug.Log("PlayerCaughtInRain");
        EventManager.InvokePlayerHit(Vector2.zero, _dmgPerTic, 0);
    }
}