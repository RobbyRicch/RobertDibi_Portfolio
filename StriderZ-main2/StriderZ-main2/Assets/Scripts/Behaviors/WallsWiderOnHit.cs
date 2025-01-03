using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallsWiderOnHit : MonoBehaviour
{
    [SerializeField] private float _zSizeMultiplier = 1.05f, _duration = 0.1f;
    public GameObject collisionEffectPrefab; // Assign the prefab in the Inspector

    private Vector3 _originalSize, _targetSize;
    private IEnumerator _lerpSizeBig, _lerpSizeOriginal;

    private void Awake()
    {
        _originalSize = transform.localScale;
        _targetSize = transform.localScale;
        _targetSize.z = transform.localScale.z * _zSizeMultiplier;
        _targetSize.x = transform.localScale.x * _zSizeMultiplier;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            _lerpSizeBig = LerpSizeBig(_targetSize, _duration);
            StartCoroutine(_lerpSizeBig);

            // Instantiate the particle effect prefab
            GameObject effectInstance = Instantiate(collisionEffectPrefab, collision.contacts[0].point, Quaternion.identity);
            ParticleSystem particleSystem = effectInstance.GetComponentInChildren<ParticleSystem>();

            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            Destroy(effectInstance, particleSystem.main.duration); // Clean up after the particle system finishes
        }
    }
    private IEnumerator LerpSizeBig(Vector3 targetSize, float duration)
    {
        _lerpSizeOriginal = null;
        float time = 0;
        Vector3 startSize = transform.localScale;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetSize;
        _lerpSizeOriginal = LerpSizeOriginal(_originalSize, _duration);
        StartCoroutine(_lerpSizeOriginal);
    }

    private IEnumerator LerpSizeOriginal(Vector3 targetSize, float duration)
    {
        _lerpSizeBig = null;
        float time = 0;
        Vector3 startSize = transform.localScale;
        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startSize, targetSize, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetSize;
    }
}