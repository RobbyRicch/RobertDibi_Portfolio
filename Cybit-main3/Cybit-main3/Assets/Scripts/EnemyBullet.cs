using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float damageAmount = 25f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //EventManager.InvokePlayerHit(damageAmount);
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Roofs"))
        {
            Destroy(gameObject);
        }
    }
}
