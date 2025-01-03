using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeceleratorMediator : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material[] _materials;
    [SerializeField] private LayerMask _groundLayer;
    [Range(0.0f, 1.0f)][SerializeField] private float _slowFactor = 0.001f;
    [SerializeField] private float _throwForce, _timeToDestroy;


    private const string _playerTag = "Player";
    private const string _groundTag = "Ground";
    private Transform _groundTransform;
    public PlayerInputHandler ThrowingPlayer { get; set; }
    private List<PlayerInputHandler> _players;
    //private Color _color;
    private float _timeSinceActivation = 0;
    private bool _isActive, _isPlaced;

    private void Start()
    {
        _players = new();
        //_color = _material.color;
        
    }
    private void Update()
    {
        if (!_isPlaced && _groundTransform && _groundTransform.position.y <= transform.position.y + 1)
        {
            _isPlaced = true;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
        }
    }
    private void FixedUpdate()
    {
        if (!_isActive)
            return;

        _timeSinceActivation += Time.deltaTime;

        if (_timeSinceActivation >= _timeToDestroy)
        {
            //_material.color = _color;
            _meshRenderer.material = _materials[0];
            _isPlaced = false;
            Destroy(gameObject);
        }

        for (int i = 0; i < _players.Count; i++)
        {
            _players[i].Controller.Rb.velocity = Vector3.ClampMagnitude(_players[i].Controller.Rb.velocity, _players[i].Controller.Speed * _slowFactor);
        }
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            if (_players.Count == 0)
            {
                _isActive = true;
                //_material.color = Color.green;
                _meshRenderer.material = _materials[1];
            }

            _players.Add(other.GetComponent<PlayerInputHandler>());
        }

        if (other.CompareTag(_groundTag))
            _groundTransform = other.transform;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            _players.Remove(other.GetComponent<PlayerInputHandler>());
        }
    }
}
