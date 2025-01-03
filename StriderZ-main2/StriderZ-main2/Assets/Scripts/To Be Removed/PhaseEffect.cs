using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseEffect : MonoBehaviour
{
    private float _helmetEffectRemoved, _bodyEffectRemoved;

    [Header("Helmet Properties")]
    [SerializeField] private float _helmetEffectTime = 50.0f;
    public float HelmetEffectTime { get => _helmetEffectTime; set => _helmetEffectTime = value; }

    [SerializeField] private float _helmetDistortCutoffHeight = -1.0f, _helmetMinCutOffset = 0.75f;
    public float HelmetDistortCutoffHeight { get => _helmetDistortCutoffHeight; set => _helmetDistortCutoffHeight = value; }
    public float HelmetMinCutOffset { get => _helmetMinCutOffset; set => _helmetMinCutOffset = value; }

    [SerializeField] private float _helmetMaxCutOffset = 4.2f;
    public float HelmetMaxCutOffset { get => _helmetMaxCutOffset; set => _helmetMaxCutOffset = value; }

    [Header("Body Properties")]
    [SerializeField] private float _bodyEffectTime = 25.0f;
    public float BodyEffectTime { get => _bodyEffectTime; set => _bodyEffectTime = value; }

    [SerializeField] private float _bodyDistortCutoffHeight = -1.0f;
    public float BodyDistortCutoffHeight { get => _bodyDistortCutoffHeight; set => _bodyDistortCutoffHeight = value; }

    [SerializeField] private float _bodyMinCutOffset = -0.8f, _bodyMaxCutOffset = 1.61f;
    public float BodyMinCutOffset { get => _bodyMinCutOffset; set => _bodyMinCutOffset = value; }
    public float BodyMaxCutOffset { get => _bodyMaxCutOffset; set => _bodyMaxCutOffset = value; }

    //public int MaterialOrder = 0;
    //public ParticleSystem ParentParticle;

    void Start()
    {
        _helmetDistortCutoffHeight = HelmetMinCutOffset;
        _bodyDistortCutoffHeight = BodyMinCutOffset;
    }

    public IEnumerator PlayHelmetPhaseEffectActivate(PhasePickup phasePickup)
    {
        float timer = 0;
        float effectAdd = _helmetMaxCutOffset / (_helmetEffectTime / 2);
        _helmetEffectRemoved = effectAdd;
        //ParentParticle.Play();
        while (timer < HelmetEffectTime)
        {
            timer++;
            _helmetDistortCutoffHeight += effectAdd;

            foreach (Material mat in phasePickup.HelmetMaterials)
            {
                mat.SetFloat("_DistortCutoffHeight", _helmetDistortCutoffHeight);
            }
            yield return null;
        }
    }
    public IEnumerator PlayHelmetPhaseEffectDeactivate(PhasePickup phasePickup)
    {
        Debug.Log("before");
        yield return new WaitForEndOfFrame();
        Debug.Log("after");
        float timer = 0;
        //float effectAdd = CutoffMin / (EffectTime );
        //ParentParticle.Play();
        while (timer < HelmetEffectTime)
        {
            timer++;
            _helmetDistortCutoffHeight -= _helmetEffectRemoved;

            foreach (Material mat in phasePickup.HelmetMaterials)
            {
                mat.SetFloat("_DistortCutoffHeight", _helmetDistortCutoffHeight);
            }
            yield return null;
        }

        phasePickup.IsHelmetPhaseDone = true;

        if (phasePickup.IsBodyPhaseDone)
            phasePickup.IsPhaseDone = true;
    }
    public IEnumerator PlayBodyPhaseEffectActivate(PhasePickup phasePickup)
    {
        float timer = 0;
        float effectAdd = BodyMaxCutOffset / (_bodyEffectTime / 2);
        _bodyEffectRemoved = effectAdd;
        //ParentParticle.Play();
        while (timer < _bodyEffectTime)
        {
            timer++;
            _bodyDistortCutoffHeight += effectAdd;

            foreach (Material mat in phasePickup.BodyMaterials)
            {
                mat.SetFloat("_DistortCutoffHeight", _bodyDistortCutoffHeight);
            }
            yield return null;
        }
    }
    public IEnumerator PlayBodyPhaseEffectDeactivate(PhasePickup phasePickup)
    {
        Debug.Log("before");
        yield return new WaitForEndOfFrame();
        Debug.Log("after");
        float timer = 0;
        //float effectAdd = CutoffMin / (EffectTime );
        //ParentParticle.Play();
        while (timer < _bodyEffectTime)
        {
            timer++;
            _bodyDistortCutoffHeight -= _bodyEffectRemoved;

            foreach (Material mat in phasePickup.BodyMaterials)
            {
                mat.SetFloat("_DistortCutoffHeight", _bodyDistortCutoffHeight);
            }
            yield return null;
        }

        phasePickup.IsBodyPhaseDone = true;

        if (phasePickup.IsHelmetPhaseDone)
            phasePickup.IsPhaseDone = true;
    }
}
