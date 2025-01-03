using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_DeflectHit : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    public Collider2D Collider => _collider;

    [SerializeField] private GameObject _deflectVFX;
    [SerializeField] private float _deflectVFXDuration = 0.5f;

    private const string _projectileTag = "Bullet";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(_projectileTag))
        {
            EnemyProjectileBase enemyProjectile = other.GetComponent<EnemyProjectileBase>();
            enemyProjectile.Deflect();

            GameObject deflectVFX = SpawnDeflectVFX(enemyProjectile.transform.position, enemyProjectile.Direction);
            Destroy(deflectVFX, _deflectVFXDuration);
        }
    }

    private GameObject SpawnDeflectVFX(Vector2 spawnPosition, Vector2 direction)
    {
        Vector2 oppositeDirection = -direction;
        float angle = Mathf.Atan2(oppositeDirection.y, oppositeDirection.x) * Mathf.Rad2Deg;
        angle -= 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        return Instantiate(_deflectVFX, spawnPosition, rotation);
    }
}
