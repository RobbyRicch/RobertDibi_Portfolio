using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tz_MeleeGuardsHandler : MonoBehaviour
{
    [SerializeField] private Tz_PhaseManager _phaseManager;
    [SerializeField] private List<EnemyBase> _Officers;
    [SerializeField] private bool _bothOfficersAreDead;

    private void Update()
    {
        if (_Officers != null && _Officers.Count > 0)
        {
            // Check if all officers are dead
            _bothOfficersAreDead = _Officers.All(officer => !officer._isAlive);
        }

        if (_bothOfficersAreDead)
        {
            _phaseManager._meleeFinished = true;
        }
    }
}
