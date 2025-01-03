using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] transforms;
    [SerializeField] private Transform[] underTrackTransforms;

    int rand;
    private void Start()
    {
        SpawnEnv();
    }

    void SpawnEnv()
    {
        foreach (var t in underTrackTransforms)
        {
            rand = Random.Range(0, EnvironmentManager.Instance.SmallBlocksList.Length);
            Instantiate(EnvironmentManager.Instance.SmallBlocksList[rand], t.position, EnvironmentManager.Instance.SmallBlocksList[rand].transform.rotation, t);
        }
        foreach (var t in transforms)
        {
            rand = Random.Range(0, EnvironmentManager.Instance.BlocksList.Length);
            Instantiate(EnvironmentManager.Instance.BlocksList[rand], t.position, EnvironmentManager.Instance.BlocksList[rand].transform.rotation,t);
        }
    }
}
