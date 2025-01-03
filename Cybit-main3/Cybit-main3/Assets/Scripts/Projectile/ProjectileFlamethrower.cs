using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFlamethrower : ProjectileBase
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

        if (collision.TryGetComponent(out EnemyBase enemy))
            enemy.TakeDamage(_direction, _damage, 0);
        else if (collision.TryGetComponent(out EnemeyAI enemyAI))
            enemyAI.TakeDamage(_direction, _damage, 0);
        else if (collision.TryGetComponent(out GOBEnemy gobEnemy))
            gobEnemy.TakeDamage(_direction, _damage, 0);
        else if (collision.TryGetComponent(out Dummy dummy))
            dummy.TakeDamage();

        if (_impactVFX)
            CreateImpact(collision);
    }
}
