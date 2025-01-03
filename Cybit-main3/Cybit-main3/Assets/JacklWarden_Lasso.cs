using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JacklWarden_Lasso : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _snareTime;
    [Header("Refrences")]
    [SerializeField] private JackalWarden_AI _wardenRef;
    [SerializeField] private Player_Controller _playerRef;
    [SerializeField] private GameObject _snare;
    [SerializeField] private GameObject _spawnedSnareVFX;
    [SerializeField] private GameObject _lassoGO;

    [Header("Bools")]
    [SerializeField] private bool _isTouchingPlayer;
    [SerializeField] private bool _shouldSnare;
    [SerializeField] private bool _isSnarring;
    [SerializeField] private bool _shouldDestory;

    private void Start()
    {
        _isTouchingPlayer = false;
        _shouldSnare = false;
        _wardenRef = GameObject.FindObjectOfType<JackalWarden_AI>();
        _lassoGO = _wardenRef._lassoGO;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_playerRef == null)
            {
                Debug.Log("trying to get player component");
                _playerRef = collision.GetComponent<Player_Controller>();
            }

            if (_playerRef != null)
            {
                _isTouchingPlayer = true;
                _shouldSnare = true;
                _isSnarring = true;
                StartCoroutine(SnarePlayer(_snareTime));
            }
        }

        if (collision.gameObject.CompareTag("Walls") && !_isTouchingPlayer && !_isSnarring || collision.gameObject.CompareTag("Roofs") && !_isTouchingPlayer && !_isSnarring)
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator SnarePlayer(float snareTime)
    {
        _playerRef.IsInputDisabled = true;
        _playerRef.IsMovementOnlyDisabled = true;
        _wardenRef._playerHasBeenSnared = true;

        /*if (_playerRef)
            _playerRef.IsComboInProgress = false;*/

        _spawnedSnareVFX = Instantiate(_snare, _playerRef.transform.localPosition, Quaternion.identity);
        yield return new WaitForSeconds(snareTime);
        Destroy(_spawnedSnareVFX, 0.25f);
        RemoveSnare();
        

    }

    public IEnumerator CheckPlayer()
    {
        yield return new WaitForSeconds(0.2f);
        if (!_isTouchingPlayer)
        {
            _lassoGO.SetActive(false);
            _wardenRef._isLassoing = false;
            Destroy(gameObject);

        }
    }

    private void RemoveSnare()
    {
        _playerRef.IsInputDisabled = false;
        _playerRef.IsMovementOnlyDisabled = false;
        _wardenRef._isLassoing = false;
        _isSnarring = false;
        _wardenRef._playerHasBeenSnared = false;
        _lassoGO.SetActive(false);
        Destroy(gameObject, 0.25f);

    }

    private void Update()
    {
        StartCoroutine(CheckPlayer());
    }
}
