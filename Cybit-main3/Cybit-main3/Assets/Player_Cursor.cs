using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Cursor : MonoBehaviour
{
    [SerializeField] private float baseSmoothSpeed = 5f; // Base smooth speed
    private float adjustedSmoothSpeed;
    private Vector2 screenBounds;

    void Start()
    {
        // Hide the system cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        // Assume a method GetSystemCursorSpeed() retrieves system cursor speed
        float systemCursorSpeed = GetSystemCursorSpeed();
        adjustedSmoothSpeed = baseSmoothSpeed * systemCursorSpeed;

        // Calculate the screen bounds in world coordinates
        Camera mainCamera = Camera.main;
        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));
        screenBounds = new Vector2(screenTopRight.x - screenBottomLeft.x, screenTopRight.y - screenBottomLeft.y);
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePosition.z = transform.position.z;

        // Clamp the cursor position within screen bounds
        mousePosition.x = Mathf.Clamp(mousePosition.x, Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x);
        mousePosition.y = Mathf.Clamp(mousePosition.y, Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y);

        transform.position = Vector3.Lerp(transform.position, mousePosition, adjustedSmoothSpeed * Time.deltaTime);
    }

    private float GetSystemCursorSpeed()
    {
        // Placeholder for actual system cursor speed retrieval logic
        // For now, return a default value (e.g., 1.0f)
        return 1.0f;
    }
}

