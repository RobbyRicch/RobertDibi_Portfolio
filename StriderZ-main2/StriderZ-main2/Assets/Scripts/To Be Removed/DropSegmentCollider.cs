using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSegmentCollider : MonoBehaviour
{
    [SerializeField] private Transform _segmentTranform;
    [SerializeField] private MeshRenderer _segmentGroundMesh;
    [SerializeField] private MeshRenderer[] _segmentGroundMeshes;
    [SerializeField] private Material _groundFlashMat;
    [SerializeField] private float _fallSpeed = 8.0f, _minimumHeight = -50.0f, _segmentDropDelay = 3.0f, _flashTimeDiminisher = 4.0f;
    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private bool _isActivated = false, _isSingleGround = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!_isActivated && other.CompareTag(_playerTag))
        {
            if (_isSingleGround)
                ActivateDropSegment();
            else
                ActivateMultipleDropSegments();
        }
    }

    private void ActivateDropSegment()
    {
        StartCoroutine(DropSegment());
        Tools.Instance.ActivateFlash(_groundFlashMat, _segmentGroundMesh, _segmentDropDelay, _flashTimeDiminisher);
        _isActivated = true;
    }
    private IEnumerator DropSegment()
    {
        yield return new WaitForSeconds(_segmentDropDelay);

        while (_segmentTranform.transform.position.y > _minimumHeight)
        {
            _segmentTranform.transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
    }

    private void ActivateMultipleDropSegments()
    {
        StartCoroutine(DropMultipleSegments());
        Tools.Instance.ActivateMultipleFlash(_groundFlashMat, _segmentGroundMeshes, _segmentDropDelay, _flashTimeDiminisher);
        _isActivated = true;
    }
    private IEnumerator DropMultipleSegments()
    {
        yield return new WaitForSeconds(_segmentDropDelay);

        while (_segmentTranform.transform.position.y > _minimumHeight)
        {
            _segmentTranform.transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
    }
}
