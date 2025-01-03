using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBullet : ProjectileBase
{
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

        if (collision.TryGetComponent(out EnemyBase enemy))
        {
            if (enemy is JackalWarden_AI warden)
            {
                warden.TakeDamage(_direction, _damage, 0);
                _animator.SetTrigger("HitEnemy");
                Destroy(gameObject);
                return;
            }

            enemy.TakeDamage(_direction, _damage, _knockBackPower);
            _animator.SetTrigger("HitEnemy");
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent(out Dummy dummy))
        {
            dummy.TakeDamage();
            _animator.SetTrigger("HitEnemy");
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent(out Barrier_System barrier) && barrier._isActive && barrier._canBeDamaged)
        {
            barrier.TakeDamage(_damage);
            Destroy(gameObject);
        }
        else if (collision.TryGetComponent(out HPCapsule_Interactable capsule) && !capsule._hasBeenShot && !capsule._isBroken)
        {
            capsule._hasBeenShot = true;
            Destroy(gameObject);
        }

    }

    


}
