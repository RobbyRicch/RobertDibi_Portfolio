using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] private GameObject MobRef;
    [SerializeField] private float MinTime = 4f;
    [SerializeField] private float MaxTime = 8f;

    private float _timer = 0;
    private float rand;

    private void Start()
    {
        rand = Random.Range(MinTime, MaxTime);
    }
    // Update is called once per frame
    void Update()
    {
        if (_timer >= rand)
        {
            Instantiate(MobRef,transform.position,Quaternion.identity);
            rand = Random.Range(MinTime, MaxTime);
            _timer = 0;
        }
        else
        {
            _timer += Time.deltaTime;
        }
    }
}
