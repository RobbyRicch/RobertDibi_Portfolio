using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSpawner : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private float TimeToSpawn = 5f;
    [SerializeField] private int MaxSpawnAtOnce;
    [SerializeField] private List<GameObject> AllStatusEffects;
    private List<GameObject> _spawnedSEs = new List<GameObject>();
    private float _timer = 0;
    private void Update()
    {
        for (int i = 0; i < _spawnedSEs.Count; i++)
        {
            if (_spawnedSEs[i] == null)
                _spawnedSEs.Remove(_spawnedSEs[i]);
        }
        if (_spawnedSEs.Count >= MaxSpawnAtOnce)
            return;

        if (_timer < TimeToSpawn)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            Vector3 randomPosition = GetRandomPositionInBox();
            int rand = Random.Range(0, AllStatusEffects.Count);
            GameObject newSE = Instantiate(AllStatusEffects[rand], randomPosition, Quaternion.identity);
            _spawnedSEs.Add(newSE);
            _timer = 0;
        }
    }
    private Vector3 GetRandomPositionInBox()
    {
        Vector3 min = boxCollider.bounds.min;
        Vector3 max = boxCollider.bounds.max;

        float randomX = Random.Range(min.x, max.x);
        float randomY = Random.Range(min.y, max.y);
        float randomZ = Random.Range(min.z, max.z);

        return new Vector3(randomX, randomY, randomZ);
    }
}
