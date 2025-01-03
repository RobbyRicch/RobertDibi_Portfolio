using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : MonoBehaviour
{
    [Range(0f, 0.2f)]
    [SerializeField] private float WaveSize = 0.2f;
    [Range(-1f, 1.4f)]
    [SerializeField] private float DistortCutoffHeight = -1f;
    [Range(0f, 6f)]
    [SerializeField] private float GeoNoiseScale = 5f;

    [SerializeField] private ParticleSystem ParentParticle;

    [SerializeField] private float EffectTime = 10f;

    private Material material;
    private bool effectDone = false;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetFloat("_WaveSize", WaveSize);
        material.SetFloat("_GeoNoiseScale", GeoNoiseScale);
        if (!effectDone)
            StartCoroutine(PlayStunEffect());
    }

    IEnumerator PlayStunEffect()
    {
        yield return new WaitForSeconds(2);
        float timer = 0;
        float effectAdd = 1.4f / (EffectTime * 50);
        ParentParticle.Play();
        while (timer < EffectTime)
        {
            timer++;
            DistortCutoffHeight += effectAdd;
            Debug.Log(DistortCutoffHeight);
            material.SetFloat("_DistortCutoffHeight", DistortCutoffHeight);
            yield return null;
        }
        effectDone = true;
    }
}
