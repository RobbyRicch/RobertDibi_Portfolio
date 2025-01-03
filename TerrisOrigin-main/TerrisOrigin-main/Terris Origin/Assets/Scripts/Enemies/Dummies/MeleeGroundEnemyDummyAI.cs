using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGroundEnemyDummyAI : EnemyAI
{
    private MeleeAttack _meleeAttackRef;

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
    }
}
