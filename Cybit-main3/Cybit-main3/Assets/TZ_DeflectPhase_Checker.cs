using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TZ_DeflectPhase_Checker : MonoBehaviour
{
    [Header("Goal :Deflect group")]
    [SerializeField] private List<EnemyBase> _guardsList;
    [SerializeField] private GameObject _secondGroup;
    [SerializeField] private Tz_PhaseManager _phaseManager;
    [SerializeField] private bool _GroupDead;
    [SerializeField] private bool _isSecondGroup;

    private void Update()
    {
        CheckIfAllRangersAreDead();
    }

    private void CheckIfAllRangersAreDead()
    {
        // Assume all are dead initially
        _GroupDead = true;

        // Iterate through the list and check if any Ranger is still alive
        foreach (EnemyBase guards in _guardsList)
        {
            if (guards._isAlive)
            {
                _GroupDead = false;
                break; // No need to continue if one is found alive
            }
        }

        // Optionally, trigger the next phase if all are dead
        if (_GroupDead && !_isSecondGroup)
        {
            _secondGroup.SetActive(true);
        }

        if (_GroupDead && _isSecondGroup)
        {
            _phaseManager._deflectFinished = true;
        }
    }
}
