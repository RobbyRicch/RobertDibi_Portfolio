using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnHit : MonoBehaviour
{
    [Header("Player Controller")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Transform _fullBodyTransform;
    [SerializeField] private int _onHitLayer0 = 11, _onHitLayer1, _onHitLayer2;
    private const string _grappleTag = "Grapple";

    #region Flash Data
    [Header("Flash")]
    [SerializeField] private Material _flashMaterial;
    [SerializeField] private float _flashDuration;
    [SerializeField] private bool _isFlashableOnHit;
    private Material _originalBodyMat, _originalHelmetMat01, _originalHelmetMat02;
    #endregion

    #region Expand Data
    [Header("Expand")]
    [SerializeField] private float _ySizeMultiplaier = 1.05f, _changeSizeDuration = 0.1f;
    [SerializeField] private bool _isExpandableOnHit;
    private Vector3 _originalSize = Vector3.zero, _targetSize = Vector3.zero;
    private IEnumerator _lerpSizeBig, _lerpSizeOriginal;
    #endregion

    #region Spawn Particles Data
    [Header("Spawn Particles")]
    [SerializeField] private ParticleSystem _wallParticlePartOne;
    [SerializeField] private ParticleSystem _wallParticlePartTwo;
    [SerializeField] private float _particleDuration = 0.1f;
    [SerializeField] private bool _canSpawnParticles;
    #endregion

    #region Debugs
    [Header("Debug")]
    [SerializeField] private bool _debugExpand;
    #endregion

    private void Awake()
    {
        _originalSize = _fullBodyTransform.localScale;
        _targetSize = _fullBodyTransform.localScale;
        _targetSize.y = _fullBodyTransform.localScale.y * _ySizeMultiplaier;
        _originalBodyMat = _playerController.InputHandler.Data.ModelData.BodyMesh.material;
        _originalHelmetMat01 = _playerController.InputHandler.Data.ModelData.HelmetMesh.materials[0];
        _originalHelmetMat02 = _playerController.InputHandler.Data.ModelData.HelmetMesh.materials[1];
    }
    private void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == _onHitLayer0 || collision.collider.gameObject.layer == _onHitLayer1 || collision.collider.gameObject.layer == _onHitLayer2)
        {
            //if (_isFlashableOnHit && collision.collider.CompareTag(_grappleTag))
            //{
            //    _playerController.InputHandler.Data.BodyMesh.material = _flashMaterial;
            //
            //    Material[] materials = _playerController.InputHandler.Data.HelmetMesh.materials;
            //    materials[0] = _flashMaterial;
            //    materials[1] = _flashMaterial;
            //    _playerController.InputHandler.Data.HelmetMesh.materials = materials;
            //
            //    StartCoroutine(Flash());
            //}
            if (_canSpawnParticles)
            {
                InstantiateParticleOnImpactPoint(_wallParticlePartOne, collision);
                InstantiateParticleOnImpactPoint(_wallParticlePartTwo, collision);
            }
            if (_isExpandableOnHit)
            {
                _lerpSizeBig = LerpSizeBig(_targetSize, _changeSizeDuration);
                StartCoroutine(_lerpSizeBig);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (_isFlashableOnHit && other.CompareTag(_grappleTag))
        {
            _playerController.InputHandler.Data.ModelData.BodyMesh.material = _flashMaterial;

            Material[] materials = _playerController.InputHandler.Data.ModelData.HelmetMesh.materials;
            materials[0] = _flashMaterial;
            materials[1] = _flashMaterial;
            _playerController.InputHandler.Data.ModelData.HelmetMesh.materials = materials;

            StartCoroutine(Flash());
        }
    }

    #region Flash Logic
    IEnumerator Flash()
    {
        yield return new WaitForSeconds(_flashDuration);
        _playerController.InputHandler.Data.ModelData.BodyMesh.material = _originalBodyMat;

        Material[] materials = new Material[2];
        materials[0] = _originalHelmetMat01;
        materials[1] = _originalHelmetMat02;
        _playerController.InputHandler.Data.ModelData.HelmetMesh.materials = materials;
    }
    #endregion

    #region Expand Logic
    private IEnumerator LerpSizeBig(Vector3 targetSize, float duration)
    {
        _lerpSizeOriginal = null;
        float time = 0;
        Vector3 startSize = _fullBodyTransform.localScale;
        while (time < duration)
        {
            _fullBodyTransform.localScale = Vector3.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _fullBodyTransform.localScale = targetSize;
        _lerpSizeOriginal = LerpSizeOriginal(_originalSize, _changeSizeDuration);
        StartCoroutine(_lerpSizeOriginal);

        if (_debugExpand) Debug.Log("Expanded");
    }
    private IEnumerator LerpSizeOriginal(Vector3 targetSize, float duration)
    {
        _lerpSizeBig = null;
        float time = 0;
        Vector3 startSize = _fullBodyTransform.localScale;
        while (time < duration)
        {
            _fullBodyTransform.localScale = Vector3.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        _fullBodyTransform.localScale = targetSize;

        if (_debugExpand) Debug.Log("Shrunk");
    }
    #endregion

    #region Spawn Particles Logic
    private void InstantiateParticleOnImpactPoint(ParticleSystem particle, Collision collision)
    {
        ParticleSystem particleClone = Instantiate(particle, _playerController.gameObject.transform);
        Vector3 particleOneOriginalSize = particleClone.transform.localScale;
        particleClone.transform.parent = collision.collider.transform;
        particleClone.transform.localScale = particleOneOriginalSize;

        // make look in right direction (Not working)
        //Vector3 contactDiretion = (collision.transform.position - transform.position).normalized;
        //particleClone.transform.LookAt(contactDiretion);
    }
    IEnumerator WallParticles()
    {
        yield return new WaitForSeconds(_particleDuration);

        Destroy(_wallParticlePartOne);

    }
    #endregion
}