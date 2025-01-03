using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauziloEnemyAI : EnemyAI
{
    private MeleeAttack _meleeAttackRef;
    private bool _attacking;

    private void Start()
    {
        _meleeAttackRef = GetComponent<MeleeAttack>();
    }

    // Update is called once per frame
    void Update()
    {
        RunEnemy();
    }

    public override void AttackPatternController()
    {
        _meleeAttackRef.DoAttack();
        _attacking = true;
    }

    public override void AnimationHandler()
    {
        if (AnimatorRef != null)
        {
            AnimatorRef.SetFloat("Speed", Agent.velocity.magnitude / Agent.speed);

            if (_attacking)
            {
                AnimatorRef.SetTrigger("Melee");
                _attacking = false;
            }
        }
    }
}
