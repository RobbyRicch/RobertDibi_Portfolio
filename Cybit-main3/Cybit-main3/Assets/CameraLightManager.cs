using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraLightManager : MonoBehaviour
{
    [SerializeField] private List<Light2D> _lightsToShutDown;

    private void OnEnable()
    {
        foreach (Light2D lights in _lightsToShutDown)
        {
            lights.enabled = false;
        }
    }

    private void OnDisable()
    {
        foreach (Light2D lights in _lightsToShutDown)
        {
            lights.enabled = true;
        }
    }

}
