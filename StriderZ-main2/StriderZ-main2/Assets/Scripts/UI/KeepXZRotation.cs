using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepXZRotation : MonoBehaviour
{
    private void Update()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.x = 90.0f;
        rotation.z = 0.0f;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
