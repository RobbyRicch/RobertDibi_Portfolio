using UnityEngine;
using UnityEngine.SceneManagement;

public class RobbyPositionUpdater : MonoBehaviour
{
    // Reference to the object that you want to update
    public GameObject objectToUpdate;

    // The position and rotation you want to apply to the object
    public Vector3 newPosition;
    public Vector3 newRotation;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the object to update is not null
        if (objectToUpdate != null)
        {
            // Apply the new position and rotation
            objectToUpdate.transform.position = newPosition;
            objectToUpdate.transform.eulerAngles = newRotation;
        }
    }
}
