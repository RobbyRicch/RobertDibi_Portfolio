using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrackShifter : TriggerSwitch
{
    private const string _playerTag = "Player";
    
    [SerializeField] private Transform _docker;
    [SerializeField] private Transform[] _tracksTr;
    [SerializeField] private bool[] _isTrackOnline;
    [SerializeField] float _shiftDuration;

    private IEnumerator _dockerMover = null;
    private int _currentActiveParentIndex;
    private int _useCounter = 0;

    private void Start()
    {
        Initialize();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag) && _useCounter == 0)
        {
            Trigger();
            GetComponent<Renderer>().material.color = Color.green;
        }
    }

    private void Initialize()
    {
        for (int i = 0; i < _tracksTr.Length; i++)
        {
            if (_tracksTr[i].GetChild(0) == _docker)
            {
                _isTrackOnline[i] = true;
                _currentActiveParentIndex = i;
                break;
            }
        }
    }
    public override void Trigger()
    {
        for (int i = 0; i < _isTrackOnline.Length; i++)
            _isTrackOnline[i] = false;

        int randomNum = UnityEngine.Random.Range(0, _tracksTr.Length);

        if (randomNum == _currentActiveParentIndex && _currentActiveParentIndex != _tracksTr.Length - 1)
            randomNum++;
        else if (randomNum == _currentActiveParentIndex)
            randomNum--;

        if (_dockerMover == null)
        {
            _currentActiveParentIndex = randomNum;
            _dockerMover = MoveDocker(randomNum);
            StartCoroutine(_dockerMover);
        }

        _useCounter++;
    }

    private IEnumerator MoveDocker(int newParentNum)
    {
        Transform newParent = _tracksTr[newParentNum];

        if (newParent)
        {
            float time = 0;
            Vector3 startPosition = _docker.position;
            Quaternion startRotation = _docker.rotation;

            while (time < _shiftDuration)
            {
                _docker.position = Vector3.Lerp(startPosition, newParent.position, time / _shiftDuration);
                _docker.rotation = Quaternion.Lerp(startRotation, newParent.rotation, time / _shiftDuration);
                time += Time.deltaTime;
                yield return null;
            }
            _docker.SetPositionAndRotation(newParent.position, newParent.rotation);
            _docker.SetParent(_tracksTr[newParentNum]);
        }
        else
        {
            Debug.LogError($"No new parent in {name}");
        }

        _dockerMover = null;
    }
}
