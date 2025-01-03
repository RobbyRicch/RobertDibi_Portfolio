using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileFlamethrower : EnemyProjectileBase
{
    protected override void OnDestroy() { }
    protected override void CreateImpact(Collider2D collision)
    {
        if (_impactVFX)
        {
            ImpactVFX impact = Instantiate(_impactVFX, collision.transform.position, Quaternion.identity);
            impact.transform.SetParent(collision.transform);
        }
    }
    protected override void OnHit(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(_wallsTag) || collision.gameObject.CompareTag(_roofsTag))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag(_targetTag))
        {
            EventManager.InvokePlayerHit(_direction, _damage, _knockBackPower);
            Destroy(gameObject);
        }

        if (_impactVFX)
            CreateImpact(collision);
    }
}
