using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSwitch : TriggerSwitch
{
    private const string _attractorTag = "Grapple";

    [SerializeField] private Transform _innerPartTr_01, _closed_01, _open_01;
    [SerializeField] private Transform _innerPartTr_02, _closed_02, _open_02;
    [Range(0, 3)][SerializeField] private float _deactivationDuration = 1.0f, _feedbackDuration = 0.5f;
    [SerializeField] private bool _isOnline = true;
    [SerializeField] private Vector3 _targetSize;
    /*[SerializeField] private bool _try = false;*/

    private IEnumerator _stateChanger = null, _feedbacker = null;
    private Vector3 _originalSize;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_attractorTag))
        {
            Trigger();
        }
    }
/*    private void Update()
    {
        if (_try)
        {
            Trigger();
            _try = false;
        }
    }*/

    public override void Trigger()
    {
        if (_feedbacker == null)
        {
            _originalSize = transform.localScale;
            _feedbacker = SwitchFeedback();
            StartCoroutine(SwitchFeedback());
        }

        if (_stateChanger == null)
        {
            _stateChanger = ChangeBridgeState(_isOnline);
            StartCoroutine(_stateChanger);
        }
    }

    private IEnumerator ChangeBridgeState(bool isOnline)
    {
        float time = 0;
        Vector3 currentPos_01 = _innerPartTr_01.position;
        Vector3 targetPos_01 = Vector3.zero;

        Vector3 currentPos_02 = _innerPartTr_02.position;
        Vector3 targetPos_02 = Vector3.zero;

        if (isOnline)
        {
            targetPos_01 = _closed_01.position;
            targetPos_02 = _closed_02.position;
        }
        else
        {
            targetPos_01 = _open_01.position;
            targetPos_02 = _open_02.position;
        }

        while (time < _deactivationDuration)
        {
            _innerPartTr_01.position = Vector3.Lerp(currentPos_01, targetPos_01, time / _deactivationDuration);
            _innerPartTr_02.position = Vector3.Lerp(currentPos_02, targetPos_02, time / _deactivationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        _innerPartTr_01.position = targetPos_01;
        _innerPartTr_02.position = targetPos_02;
        _isOnline = !isOnline;
        _stateChanger = null;
    }
    private IEnumerator SwitchFeedback()
    {
        float time = 0;
        Vector3 startScale = transform.localScale;

        while (time < _feedbackDuration / 2)
        {
            transform.localScale = Vector3.Lerp(startScale, _targetSize, time / (_feedbackDuration / 2));
            time += Time.deltaTime;
            yield return null;
        }

        startScale = transform.localScale;

        while (time < _feedbackDuration / 2)
        {
            transform.localScale = Vector3.Lerp(startScale, _originalSize, time / (_feedbackDuration / 2));
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = _originalSize;
        _feedbacker = null;
    }
}
