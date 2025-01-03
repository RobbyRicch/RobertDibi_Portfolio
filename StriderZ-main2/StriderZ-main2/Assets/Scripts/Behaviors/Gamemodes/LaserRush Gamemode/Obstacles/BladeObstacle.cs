using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeObstacle : MonoBehaviour
{
    [SerializeField] private float _bounceForce = 20.0f, _dangerTime = 10.0f;
    [SerializeField] private GameObject _hitEffect;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInputHandler player = collision.gameObject.GetComponent<PlayerInputHandler>();

            Vector3 collisionNormal = collision.gameObject.transform.position - collision.contacts[0].point;
            collisionNormal.Normalize();
            player.Controller.Rb.AddForce(collisionNormal * _bounceForce, ForceMode.Impulse);
            Instantiate(_hitEffect, collision.contacts[0].point, Quaternion.Euler(collision.contacts[0].point), transform);
            if (player.Data.DangerCounter > 2)
                player.Controller.IsAlive = false;
            else
            {
                player.Controller.SetDangerTime();
            }
        }
    }
}
