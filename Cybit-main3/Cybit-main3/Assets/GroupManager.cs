using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupManager : MonoBehaviour
{
    [Header("Current Group Enemies")]
    [SerializeField] private List<EnemyBase> _enemies;
    [SerializeField] private bool _allDead;
    [SerializeField] private GameObject _playerGO;

    [Header("Next Group Of Enemies")]
    [SerializeField] private bool _hasNextGroup;
    [SerializeField] private GameObject _nextGroup;

    [Header("Additional Components")]
    [SerializeField] private bool _shouldActivateExtraComponent;
    [SerializeField] private List<GameObject> _extras;

    private void Start()
    {
        Player_Controller playerController = FindObjectOfType<Player_Controller>();
        if (playerController != null)
        {
            _playerGO = playerController.gameObject;
        }
        else
        {
            Debug.LogError("Player_Controller not found in the scene.");
        }

        foreach (EnemyBase enemiesInGroup in _enemies)
        {
            enemiesInGroup.PlayerTarget = _playerGO.transform;
        }
    }

    private void Update()
    {

        _allDead = _enemies.All(enemy => !enemy._isAlive);

        if (_allDead && _hasNextGroup)
        {
            _nextGroup.SetActive(true);

            if (_shouldActivateExtraComponent)
            {
                foreach (GameObject extras in _extras)
                {
                    if (extras)
                        extras.SetActive(true);
                }
            }
        }

    }

}
