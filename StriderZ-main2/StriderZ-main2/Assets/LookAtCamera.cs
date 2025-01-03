using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform[] objectsToLookAt;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera not found! Make sure there is a camera tagged as 'MainCamera' in the scene.");
        }
    }

    private void Update()
    {
        if (mainCamera != null && objectsToLookAt != null && objectsToLookAt.Length > 0)
        {
            foreach (Transform objTransform in objectsToLookAt)
            {
                if (objTransform != null)
                {
                    // Get the direction from the object to the camera
                    Vector3 directionToCamera = mainCamera.transform.position - objTransform.position;

                    // Rotate the object to face the camera
                    objTransform.rotation = Quaternion.LookRotation(directionToCamera, mainCamera.transform.up);
                }
            }
        }
    }
}
