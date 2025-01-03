using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private Weapon _connectedWeapon;
    [SerializeField] private BoxCollider2D _damageTriggerBox;
    [SerializeField] private int _dmg;

    private void Start()
    {
        _dmg = _connectedWeapon._dmg;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyAttributesManager enemyAttributesManager = collision.gameObject.GetComponent<EnemyAttributesManager>();

            if (enemyAttributesManager._enemyCanTakeDmg && _damageTriggerBox.enabled) 
            {
                enemyAttributesManager.TakeDamage(_dmg);
            }
        }
    }
}
