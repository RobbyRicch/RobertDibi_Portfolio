using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform CamHolder;
    public Camera cam;
    public Transform Arms;
    private float xRotaton = 0f;

    public float xSensitivity = 30f;
    public float ySensitivity = 30f;

    public Vector2 LookInput;
    public void ProcessLook(Vector2 input)
    {
        LookInput = input;
        float mouseX = input.x;
        float mouseY = input.y;
        //calculate camera rotation for looking up and down
        xRotaton -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotaton = Mathf.Clamp(xRotaton, -80f, 80f);
        //apply this to the camera trasform
        //cam.transform.localRotation = Quaternion.Euler(xRotaton, 0f, 0f);
        //CamHolder.localRotation = Quaternion.Euler(xRotaton, 0f, 0f);
        Arms.localRotation = Quaternion.Euler(xRotaton, 0f, 0f);
        //rotate player to look left and right
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }
}
