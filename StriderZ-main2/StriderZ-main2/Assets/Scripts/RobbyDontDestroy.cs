using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobbyDontDestroy : MonoBehaviour
{
    private string objectID;

    private void Awake()
    {
        objectID = name + transform.position.ToString() + transform.eulerAngles.ToString();
    }
    void Start()
    {
        for (int i = 0; i < Object.FindObjectsOfType<RobbyDontDestroy>().Length; i++)
        {
            if (Object.FindObjectsOfType<RobbyDontDestroy>()[i] != this)
            {
                if (Object.FindObjectsOfType<RobbyDontDestroy>()[i].objectID == objectID)
                {
                    Destroy(gameObject);
                }
            }
        }

        DontDestroyOnLoad(gameObject);
    }


}
