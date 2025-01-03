using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        mousePosition.z = transform.position.z;

        transform.position = mousePosition;
    }
}
