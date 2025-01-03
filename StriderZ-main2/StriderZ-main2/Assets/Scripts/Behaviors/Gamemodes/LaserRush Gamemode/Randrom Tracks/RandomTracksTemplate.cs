using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomTracksTemplate : MonoBehaviour
{
    private static RandomTracksTemplate _instance;
    public static RandomTracksTemplate Instance => _instance; 

    public List<GameObject> FirstTracksList;
    public List<GameObject> MiddleTracksList;
    public List<GameObject> LastTracksList;

    public List<GameObject> Tracks;
    public List<Transform> NavPoints = new List<Transform>();
    public List<GameObject> PlacementPillars;
    public GameObject Fork;
    public GameObject ForkEndless;
    public GameObject Arena;
    public int FirstTracksToSpawn = 5;
    public int MiddleTracksToSpawn = 5;
    public int LastTracksToSpawn = 5;
    public bool fork = false;
    public bool FinishedGeneration = false;
    public List<Transform> DollyLeftWaypoints;
    public List<Transform> DollyRightWaypoints;

    public SpawnBuildings spawnBuildings;
    public NavMeshSurface navmeshSurface;

    public int ConnectorCounter = 0;

    public bool UseRailing = false;

    private void Awake()
    {
        _instance = this;
        spawnBuildings = SpawnBuildings.Instance;
    }
}
