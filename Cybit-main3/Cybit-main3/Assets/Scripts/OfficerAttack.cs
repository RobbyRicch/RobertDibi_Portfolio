using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficerAttack : MonoBehaviour
{
    private float _damage = 1;
    public float Damage { get => _damage; set => _damage = value; }

    private const string _playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(_playerTag))
        {
            EventManager.InvokePlayerHit(Vector2.zero, _damage, 2);
        }
    }
}
