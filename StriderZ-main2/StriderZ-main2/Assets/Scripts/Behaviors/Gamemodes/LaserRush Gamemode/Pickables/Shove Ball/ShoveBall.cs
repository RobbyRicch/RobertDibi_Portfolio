using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveBall : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private GameObject _bombObj;
    [SerializeField] private GameObject _bombEffect;
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _throwForce, _timeToDestroy, _throwUpwardFactor;
    [SerializeField] private float _radius = 5f, _force = 1500f;

    private const string _playerTag = "Player";
    private const string _groundTag = "Ground";
    public PlayerInputHandler ThrowingPlayer { get; set; }
    private float _timeSinceActivation = 0;
    private bool _isActive = false;

    private void Start()
    {
        Throw();
    }
    private void FixedUpdate()
    {
        if (!_isActive)
            return;

        _timeSinceActivation += Time.deltaTime;

        if (_timeSinceActivation >= _timeToDestroy)
        {
            _isActive = false;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag) || other.CompareTag(_groundTag) || ((1 << other.gameObject.layer) & _wallLayer) != 0)
        {
            if (!_isActive && other.gameObject != ThrowingPlayer.gameObject)
            {
                _isActive = true;
                _bombObj.SetActive(false);
                _sphereCollider.center = Vector3.zero;
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
                _rb.isKinematic = true;
                Explode();
            }
        }
    }
    private void Explode()
    {
        _bombEffect.SetActive(true);

        Collider[] colliders = Physics.OverlapSphere(transform.position, _radius);
        foreach (Collider nearByObject in colliders)
        {
            Rigidbody _rb = nearByObject.GetComponent<Rigidbody>();
            if (_rb != null)
            {
                //_rb.AddExplosionForce(_force, transform.position, _radius);

                Vector3 dir = transform.position - nearByObject.transform.position;
                Vector3 dirNoY = new Vector3(dir.x, 0, dir.z);
                _rb.AddForce(-dirNoY.normalized * _force, ForceMode.Impulse);
            }
        }
    }
    private void Throw()
    {
        _rb.AddForce((ThrowingPlayer.Controller.CrosshairParent.transform.forward - (Vector3.up / _throwUpwardFactor)) * _throwForce, ForceMode.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}
