using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficesFirst_Trigger : MonoBehaviour
{
    [Header("Goal : Activate the first group of enemies in the offices")]
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
