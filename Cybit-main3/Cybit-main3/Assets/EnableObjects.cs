using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjects : MonoBehaviour
{
    [SerializeField] private SpawnerBase[] _spawnersToActivate;
    [SerializeField] private EnemyHenchman[] _henchmanToActivate;
    private const string _playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(_playerTag))
        {
            Player_Controller tempController = collision.gameObject.GetComponent<Player_Controller>();
            for (int i = 0; i < _spawnersToActivate.Length; i++)
            {
                _spawnersToActivate[i].gameObject.SetActive(true);
            }

            if (_henchmanToActivate.Length > 0)
            {
                for (int i = 0; i < _henchmanToActivate.Length; i++)
                {
                    _henchmanToActivate[i].PlayerTarget = tempController.transform;
                    _henchmanToActivate[i].gameObject.SetActive(true);
                }

            }

            gameObject.SetActive(false);
        }
    }
}
