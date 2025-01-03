using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private HealthHandler _healthHandlerRef;

    [SerializeField] private Material _hitEffect;
    [SerializeField] private Renderer[] _skinedMeshRenderers;
    private List<Material> _materialInstances = new List<Material>();

    [SerializeField][Range(0, 1)] private float _effectStrenght = 0.5f;
    [SerializeField] private float _flashDuration = 0.1f;
    private float _flashDurationTimer;
    private bool _isFlashing;
    private Color _flashColor;


    private void Awake()
    {
        _healthHandlerRef = GetComponent<EnemyAI>().EnemyHealthHandlerGet;
        _healthHandlerRef.OnDamageOccured += _healthHandlerRef_OnDamageOccured;
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        EffectState();
    }


    private void EffectState()
    {
        if (_isFlashing)
        {
            if (_flashDurationTimer >= _flashDuration)
            {
                _flashDurationTimer = 0;
                _isFlashing = false;
                foreach (var mat in _materialInstances)
                {
                    mat.color = _hitEffect.color;
                }
            }
            else
            {
                foreach (var mat in _materialInstances)
                {
                    mat.color = _flashColor;
                }
            }
            _flashDurationTimer += Time.deltaTime;
        }
    }

    private void Setup()
    {
        _flashColor = _hitEffect.color;
        _flashColor.a = _effectStrenght;

        if (_skinedMeshRenderers != null)
        {
            foreach (var item in _skinedMeshRenderers)
            {
                for (int i = 0; i < item.materials.Length; i++)
                {
                    if (item.materials[i].name == _hitEffect.name + " (Instance)")
                    {
                        _materialInstances.Add(item.materials[i]);
                    }
                }
            }
        }
    }


    private void _healthHandlerRef_OnDamageOccured(object sender, System.EventArgs e)
    {
        _isFlashing = true;
        _flashDurationTimer = 0;
    }
}
