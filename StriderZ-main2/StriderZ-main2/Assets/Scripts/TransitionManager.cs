using System.Collections;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public GameObject objectToActivate;
    public GameObject objectToDisable;
    public GameObject particleSystemToPlay1;
    public GameObject particleSystemToPlay2;
    public float delayTimeBeforeActivation = 1f;
    public float delayTimeBeforeDeactivation = 3f;

    private bool hasStarted;

    public void OnButtonClicked()
    {
        // Check if the particle systems are not already playing to avoid restarting them
        if (!hasStarted)
        {
            // Play both particle systems
            PlayAllParticleSystems(particleSystemToPlay1);
            PlayAllParticleSystems(particleSystemToPlay2);

            hasStarted = true;

            // Start the coroutine to handle the activation and deactivation with delays
            StartCoroutine(PerformActionsWithDelay());
        }
    }

    private void PlayAllParticleSystems(GameObject parentObject)
    {
        ParticleSystem[] particleSystems = parentObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }

    private IEnumerator PerformActionsWithDelay()
    {
        // Wait for the specified delay before activating the object
        yield return new WaitForSeconds(delayTimeBeforeActivation);
        objectToActivate.SetActive(true);

        // Wait for the specified delay before disabling the object
        yield return new WaitForSeconds(delayTimeBeforeDeactivation - delayTimeBeforeActivation);
        objectToDisable.SetActive(false);
    }
}
