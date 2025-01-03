using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeflectorAbility : MonoBehaviour
{
    public PuckStatusEffects CurrentStatusEffect;
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _deflectorOjb;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Material _deflectorDefaultMaterials;
    [SerializeField] private PuckEffectsManager _effectManager;

    public void Use()
    {
        _deflectorOjb.SetActive(true);
        _animator.SetTrigger("Deflect");
        StartCoroutine(DisableDeflector());
    }

    IEnumerator DisableDeflector()
    {
        yield return new WaitForSeconds(1);
        _deflectorOjb.SetActive(false);
    }
    private void Update()
    {
        switch (CurrentStatusEffect)
        {
            case PuckStatusEffects.None:
                _renderer.material = _deflectorDefaultMaterials;
                break;
            case PuckStatusEffects.EnlargePuck:
                for (int i = 0; i < _effectManager.effects.Count; i++)
                {
                    if (_effectManager.effects[i].Status == PuckStatusEffects.EnlargePuck)
                    {
                        _renderer.material = _effectManager.effects[i].material;
                    }
                }
                break;
            case PuckStatusEffects.Unblockable:
                for (int i = 0; i < _effectManager.effects.Count; i++)
                {
                    if (_effectManager.effects[i].Status == PuckStatusEffects.EnlargePuck)
                    {
                        _renderer.material = _effectManager.effects[i].material;
                    }
                }
                break;
            case PuckStatusEffects.Impulse:
                for (int i = 0; i < _effectManager.effects.Count; i++)
                {
                    if (_effectManager.effects[i].Status == PuckStatusEffects.EnlargePuck)
                    {
                        _renderer.material = _effectManager.effects[i].material;
                    }
                }
                break;
        }
    }
}
