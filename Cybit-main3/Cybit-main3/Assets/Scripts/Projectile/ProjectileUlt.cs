using UnityEngine;

public class ProjectileUlt : ProjectileBase
{
    //protected override void OnTriggerEnter2D(Collider2D collision) => OnHit(collision);
    //protected override void OnCollisionEnter2D(Collision2D collision) { }

    protected override void CheckOverlapBox() 
    {
        Vector2 boxPosition = (Vector2)transform.position + _boxPositionOffset;
        Collider2D[] collisions = Physics2D.OverlapBoxAll(boxPosition, _boxSize, _boxAngle);

        for (int i = 0; i < collisions.Length; i++)
        {
            Collider2D collision = collisions[i];
            if (collision.CompareTag(_targetTag))
            {
                if (collision.isTrigger)
                    OnHit(collision);
                else
                    OnHitObstacle(collision);

                break;
            }
        }
    }
    protected override void OnDestroy() { }

    protected override void OnHit(Collider2D collision)
    {
        if (collision.TryGetComponent(out EnemyBase enemy))
        {
            if (enemy is JackalWarden_AI warden)
            {
                warden.TakeDamage(_direction, _damage, 0);

                if (_animator)
                _animator.SetTrigger("HitEnemy");
                return;
            }
            enemy.TakeDamage(_direction, _damage, _knockBackPower);
        }
        else if (collision.TryGetComponent(out EnemeyAI enemyAI))
            enemyAI.TakeDamage(_direction, _damage, _knockBackPower);
        else if (collision.TryGetComponent(out GOBEnemy gobEnemy))
            gobEnemy.TakeDamage(_direction, _damage, _knockBackPower);
        else if (collision.TryGetComponent(out Dummy dummy))
            dummy.TakeDamage();
        else if (collision.TryGetComponent(out Barrier_System barrier) && barrier._isActive && barrier._canBeDamaged)
        {
            barrier.TakeDamage(_damage);

            Destroy(gameObject);
        }

        if (_impactVFX)
            CreateImpact(null);
    }
}
