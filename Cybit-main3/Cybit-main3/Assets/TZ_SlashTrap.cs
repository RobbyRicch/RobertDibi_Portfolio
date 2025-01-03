using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TZ_SlashTrap : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Melee Attack"))
        {
            Debug.Log("box touched");
            Destroy(gameObject);
        }
    }
}
