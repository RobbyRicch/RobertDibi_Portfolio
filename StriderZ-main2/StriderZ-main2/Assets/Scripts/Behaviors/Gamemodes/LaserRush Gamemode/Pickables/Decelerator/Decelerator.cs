using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decelerator : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Transform _diskObj;
    [SerializeField] private GameObject _bombObj;
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private GameObject _unactivateEffect;
    [SerializeField] private GameObject _activateEffect;
    [SerializeField] private Material[] _materials;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;
    [Range(0.0f, 0.01f)][SerializeField] private float _slowFactor = 0.001f;
    [SerializeField] private float _throwForce, _timeToDestroy , _throwUpwardFactor, _ricochetSpeedFactor;
    [SerializeField] Vector3 _targetSize = new(14.3f, 14.3f, 14.3f);

    private const string _playerTag = "Player";
    private const string _groundTag = "Ground";
    private const string _wallTag = "Wall";
    public PlayerInputHandler ThrowingPlayer { get; set; }
    private List<PlayerInputHandler> _players;
    //private Color _color;
    private float _timeSinceActivation = 0;
    private bool _isActive, _isPlaced;

    private void Start()
    {
        _players = new();
        Throw();
    }
    private void FixedUpdate()
    {
        if (!_isPlaced)
            return;

        if (_players.Count > 0)
        {
            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].Controller.Rb.velocity = Vector3.ClampMagnitude(_players[i].Controller.Rb.velocity, 
                    _players[i].Controller.Speed * _slowFactor);
            }
        }

        _timeSinceActivation += Time.deltaTime;

        if (_timeSinceActivation >= _timeToDestroy)
        {
            _meshRenderer.material = _materials[0];
            _isPlaced = false;
            Destroy(gameObject);
        }
        else
        {
            if (_isActive)
            {
                _timeSinceActivation = 0;
                _isActive = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_playerTag) && !other.CompareTag(_groundTag))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, transform.localScale.z + 8f, _wallLayer))
            {
                Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
                float rot = 90 - Mathf.Atan2(reflectDir.z, reflectDir.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, rot, 0);
                _rb.velocity = _rb.velocity + transform.forward * _rb.velocity.magnitude * _ricochetSpeedFactor;
            }
        }
        if (other.CompareTag(_playerTag) && _isPlaced)
        {
            if (_players.Count == 0)
            {
                _isActive = true;
                //_material.color = Color.green;
                _meshRenderer.material = _materials[1];
                _unactivateEffect.SetActive(false);
                _activateEffect.SetActive(true);
            }

            _players.Add(other.GetComponent<PlayerInputHandler>());
        }

        if (other.CompareTag(_groundTag))
        {
            _bombObj.SetActive(false);
            _sphereCollider.center = Vector3.zero;
            _isPlaced = true;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
            _unactivateEffect.SetActive(true);
            //StartCoroutine(ActivateDecelerator(_targetSize, 0.5f));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _players.Remove(other.GetComponent<PlayerInputHandler>());
        }
    }

    private void Throw()
    {
        _rb.AddForce((ThrowingPlayer.Controller.CrosshairParent.transform.forward - (Vector3.up / _throwUpwardFactor)) * _throwForce, ForceMode.Impulse);
    }

    private IEnumerator ActivateDecelerator(Vector3 targetSize, float duration)
    {
        float time = 0;
        Vector3 startSize = transform.localScale;
        while (time < duration)
        {
            _diskObj.localScale = Vector3.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _diskObj.localScale = targetSize;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Ray(transform.position, transform.forward*8));
    }
}
