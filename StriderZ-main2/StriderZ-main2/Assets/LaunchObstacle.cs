using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchObstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {

        }
    }
}
