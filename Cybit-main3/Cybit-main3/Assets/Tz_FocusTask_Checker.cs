using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tz_FocusTask_Checker : MonoBehaviour
{
    [Header("Goal : Check if all Rangers have been killed")]
    [SerializeField] private List<EnemyBase> _rangersList;
    [SerializeField] private Tz_PhaseManager _phaseManager;
    [SerializeField] private bool _allAreDead;

    private void Update()
    {
        CheckIfAllRangersAreDead();
    }

    private void CheckIfAllRangersAreDead()
    {
        // Assume all are dead initially
        _allAreDead = true;

        // Iterate through the list and check if any Ranger is still alive
        foreach (EnemyBase ranger in _rangersList)
        {
            if (ranger._isAlive)
            {
                _allAreDead = false;
                break; // No need to continue if one is found alive
            }
        }

        // Optionally, trigger the next phase if all are dead
        if (_allAreDead)
        {
            _phaseManager._focusFinished = true;
        }
    }
}



