using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanfalEnemyAI : EnemyAI
{
    private ShootProjectile _shootProjRef;

    private void Start()
    {
        _shootProjRef = GetComponent<ShootProjectile>();
    }

    // Update is called once per frame
    void Update()
    {
        RunEnemy();
    }

    public override void AttackPatternController()
    {
        _shootProjRef.Shoot();
    }
}
