using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HallwayEnemies_Trigger : MonoBehaviour
{

    [Header("Goal : Activate the group of enemies in the hallway")]
    [Header("Guards")]
    [SerializeField] GameObject _groupGO;

    [Header("Trigger")]
    [SerializeField] private BoxCollider2D _currentColider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            _groupGO.SetActive(true);

            _currentColider.enabled = false;

        }


    }
}
