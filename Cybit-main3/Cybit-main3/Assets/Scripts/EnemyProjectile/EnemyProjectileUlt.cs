using UnityEngine;

public class EnemyProjectileUlt : EnemyProjectileBase
{
    protected override void OnTriggerEnter2D(Collider2D collision) => OnHit(collision);
    protected override void OnCollisionEnter2D(Collision2D collision) { }
    protected override void OnDestroy() { }

    protected override void OnHit(Collider2D collision)
    {
        if (collision.CompareTag(_targetTag))
        {
            EventManager.InvokePlayerHit(_direction, _damage, _knockBackPower);
            Destroy(gameObject);
        }

        if (_impactVFX)
            CreateImpact(null);
    }
}
