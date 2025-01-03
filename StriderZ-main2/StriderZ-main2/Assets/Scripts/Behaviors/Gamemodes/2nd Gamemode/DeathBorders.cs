using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBorders : MonoBehaviour
{
    [SerializeField] private GameObject _playerElimVFX;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerLives tempPlayerLives = other.GetComponent<PlayerLives>();
            Kill(other.transform, tempPlayerLives);
        }
    }

    public void Kill(Transform transform, PlayerLives tempPlayerLives)
    {
        tempPlayerLives.Lives--;
        GameObject Effect = Instantiate(_playerElimVFX, transform.position, Quaternion.identity, null);
        tempPlayerLives.Respawn();
        Destroy(Effect, 2);
    }
}
