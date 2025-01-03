using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobbyRisingPlatform : MonoBehaviour
{
    [Header("Bools")]
    [SerializeField] private bool _shouldSpawn;

    [Header("Variables")]
    [SerializeField] private float _timeBetweenSpawn;
    [SerializeField] private float _platformSpeed;
    [SerializeField] private float _destroyTime; // New variable for destroy time

    [Header("Prefab")]
    [SerializeField] private GameObject _platformPrefab;

    [Header("Transforms")]
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private Vector3 _targetPosition; // Use Vector3 for target position

    private void Start()
    {
        StartCoroutine(SpawnPlatformPerSec());
    }

    private IEnumerator SpawnPlatformPerSec()
    {
        while (_shouldSpawn)
        {
            yield return new WaitForSeconds(_timeBetweenSpawn);

            GameObject platform = Instantiate(_platformPrefab, _spawnPosition.position, Quaternion.identity);

            StartCoroutine(MovePlatform(platform.transform));

            // Destroy the platform after _destroyTime
            Destroy(platform, _destroyTime);
        }
    }

    private IEnumerator MovePlatform(Transform platformTransform)
    {
        float elapsedTime = 0f;

        Vector3 spawnPosition = _spawnPosition.position;

        while (elapsedTime < _platformSpeed)
        {
            if (platformTransform == null) // Check if the platform is still valid
                yield break; // Exit the coroutine if the platform is no longer valid

            Vector3 newPosition = Vector3.Lerp(spawnPosition, _targetPosition, elapsedTime / _platformSpeed);
            platformTransform.position = new Vector3(platformTransform.position.x, newPosition.y, platformTransform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Use WaitForEndOfFrame to ensure the platform is completely destroyed
        yield return new WaitForEndOfFrame();

        if (platformTransform != null)
            platformTransform.position = new Vector3(platformTransform.position.x, _targetPosition.y, platformTransform.position.z);
    }
}