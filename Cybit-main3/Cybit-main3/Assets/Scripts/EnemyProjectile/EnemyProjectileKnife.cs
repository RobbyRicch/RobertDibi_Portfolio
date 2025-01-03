using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileKnife : EnemyProjectileBase
{
    protected override void OnTriggerEnter2D(Collider2D collision) => OnHit(collision);
    protected override void OnDestroy() { }

    protected override void OnHit(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(_wallsTag) || collision.gameObject.CompareTag(_roofsTag))
        {
            Destroy(gameObject);
            // should not destroy, should leave on wall without any interaction
            return;
        }

        if (collision.CompareTag(_targetTag))
        {
            EventManager.InvokePlayerHit(_direction, _damage, _knockBackPower);
            Destroy(gameObject);
        }

        if (_impactVFX)
            CreateImpact(null);
    }
}
