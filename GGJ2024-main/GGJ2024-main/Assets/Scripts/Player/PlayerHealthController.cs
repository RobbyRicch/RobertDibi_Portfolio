using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] float _maxHealth;
    [SerializeField] float _currentHealth;
    [SerializeField] Collider _collider;
    [SerializeField] private AnimationManager _animationManager;
    [SerializeField] private Rigidbody _playerRigidbody;
    public bool GotHit = false;

    // Start is called before the first frame update
    void Awake()
    {
        _currentHealth = _maxHealth;
        _collider = GetComponent<Collider>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            GameManager.Instance.OnLoseHeart();
            _currentHealth--;
            Destroy(collision.gameObject);
            GotHit = true;

            if (_currentHealth <= 0)
            {
                Debug.Log("Im Very Very Dead!");
                _animationManager.PlayDeath();
            }
            else
            {
                _animationManager.PlayHit();
            }
        }
    }

    public void OnGotHit()
    {
        if (_collider != null)
        {
            _playerRigidbody.useGravity = false;
            _collider.enabled = false;
        }
    }

    public void OnRecovered()
    {
        if (_collider != null)
        {
            _collider.enabled = true;
            _playerRigidbody.useGravity = true;
            GotHit = false;
        }
    }
}
