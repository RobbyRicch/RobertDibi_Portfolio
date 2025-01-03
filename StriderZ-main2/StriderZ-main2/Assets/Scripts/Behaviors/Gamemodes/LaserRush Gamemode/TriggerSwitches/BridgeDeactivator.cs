using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeDeactivator : TriggerSwitch
{
    private const string _attractorTag = "Grapple";

    [SerializeField] private Transform _bridgePivot;
    [SerializeField] private bool _isBridgeOnline = true;
    [SerializeField] private float _deactivationDuration = 1.0f;
    [Range (0,1)][SerializeField] private float _minBridgeSizeZ = 0.0f;

    private IEnumerator _deactivator = null;
    private int _useCounter = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_attractorTag) && _useCounter == 0)
        {
            Trigger();
            _bridgePivot.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
        }
    }

    public override void Trigger()
    {
        _isBridgeOnline = false;

        if (_deactivator == null)
        {
            _deactivator = DeactivateBridge();
            StartCoroutine(_deactivator);
        }

        _useCounter++;
    }

    private IEnumerator DeactivateBridge()
    {
        float time = 0;
        Vector3 startSize = _bridgePivot.localScale;
        Vector3 targetSize = new(_bridgePivot.localScale.x, _bridgePivot.localScale.y, _minBridgeSizeZ);

        while (time < _deactivationDuration)
        {
            _bridgePivot.localScale = Vector3.Lerp(startSize, targetSize, time / _deactivationDuration);
            time += Time.deltaTime;
            yield return null;
        }
        _bridgePivot.localScale = targetSize;
        
        _deactivator = null;
    }
}
