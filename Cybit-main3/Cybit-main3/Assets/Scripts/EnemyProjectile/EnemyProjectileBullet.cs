using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileBullet : EnemyProjectileBase
{
    protected override void CreateImpact(Collider2D collision)
    {
        base.CreateImpact(collision);
    }

    //protected override void OnTriggerEnter2D(Collider2D collision) => OnHit(collision);
    protected override void OnDestroy() 
    {
        base.OnDestroy();
    }
    
    protected override void OnHit(Collider2D collision)
    {
        if (!_canHit)
            return;

        _canHit = false;

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
    }
}
