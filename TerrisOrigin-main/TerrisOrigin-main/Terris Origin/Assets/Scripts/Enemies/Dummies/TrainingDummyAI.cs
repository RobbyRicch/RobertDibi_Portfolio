using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummyAI : MonoBehaviour
{
    private HealthHandler _dummyHealthHandler;
    // Start is called before the first frame update
    void Start()
    {
        _dummyHealthHandler = GetComponent<HealthHandler>();
        _dummyHealthHandler.OnDeathOccured += _dummyHealthHandler_OnDeathOccured;
    }

    private void _dummyHealthHandler_OnDeathOccured(object sender, System.EventArgs e)
    {
        _dummyHealthHandler._healthSystem.RefillHealth();
    }
}
