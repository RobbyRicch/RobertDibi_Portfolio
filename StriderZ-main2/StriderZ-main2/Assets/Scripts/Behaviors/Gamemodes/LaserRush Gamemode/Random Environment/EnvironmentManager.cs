using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    private static EnvironmentManager _instance;
    public static EnvironmentManager Instance => _instance;

    public GameObject[] BlocksList;
    public GameObject[] SmallBlocksList;

    public List<GameObject> Blocks;

    private void Awake()
    {
        _instance = this;
    }
}
