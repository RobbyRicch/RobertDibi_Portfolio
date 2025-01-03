using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tz_UltPhase_Check : MonoBehaviour
{
    [Header("Goal : Check if all guards have died")]
    [SerializeField] private List<EnemyBase> _guardsList;
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
        foreach (EnemyBase guards in _guardsList)
        {
            if (guards._isAlive)
            {
                _allAreDead = false;
                break; // No need to continue if one is found alive
            }
        }

        // Optionally, trigger the next phase if all are dead
        if (_allAreDead)
        {
            _phaseManager._ultFinished = true;
        }
    }
}
