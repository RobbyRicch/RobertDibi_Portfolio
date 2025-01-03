using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflector : MonoBehaviour
{
    [SerializeField] private DeflectorAbility _abilityRef;
    [SerializeField] private Animator _animator;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private float _power = 50f;
    private Puck _puckRef;
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puck"))
        {
            if (_puckRef == null)
            {
                _puckRef = other.GetComponent<Puck>();
            }
            _puckRef.CurrentStatusEffect = _abilityRef.CurrentStatusEffect;
        }
    }*/
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Puck"))
        {
            if (_puckRef == null)
            {
                _puckRef = collision.gameObject.GetComponent<Puck>();
            }
            if (_abilityRef.CurrentStatusEffect != PuckStatusEffects.None)
            {
                _puckRef.CurrentStatusEffect = _abilityRef.CurrentStatusEffect;
                _puckRef.EffectChanged = true;
                _abilityRef.CurrentStatusEffect = PuckStatusEffects.None;
                //_puckRef.gameObject.AddComponent<PickableStatusEffect>();
            }

            collision.rigidbody.AddForce(transform.forward * _power, ForceMode.Impulse);
        }
    }
    private void Update()
    {
        /*if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Deflect"))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f)
            {
                _boxCollider.enabled = true;
            }
            else if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
            {
                _boxCollider.enabled = false;
            }
        }*/
    }
}
