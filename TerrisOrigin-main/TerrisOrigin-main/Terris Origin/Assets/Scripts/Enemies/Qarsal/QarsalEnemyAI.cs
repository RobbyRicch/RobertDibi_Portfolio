using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QarsalEnemyAI : EnemyAI
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
        if (IsLunging) _meleeAttackRef.SetSpecialState(MeleeComboState.Third);
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
                if (IsLunging)
                {
                    AnimatorRef.SetTrigger("Lunge");
                }
                else
                {
                    if ((int)(_meleeAttackRef.ComboState) == 1)
                    {
                        AnimatorRef.SetBool("PunchMirror", false);
                    }
                    else if ((int)(_meleeAttackRef.ComboState) == 2)
                    {
                        AnimatorRef.SetBool("PunchMirror", true);
                    }
                    AnimatorRef.SetTrigger("Punch");
                }
                _attacking = false;
            }

            /*        AnimatorRef.SetBool("Dodging", SuccededDodging);
            */
        }
    }
}