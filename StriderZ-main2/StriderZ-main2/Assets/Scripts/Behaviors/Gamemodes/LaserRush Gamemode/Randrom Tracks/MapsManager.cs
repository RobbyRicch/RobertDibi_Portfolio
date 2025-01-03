using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapsManager : MonoBehaviour
{
    public List<Maps> maps;
    [SerializeField] private SpawnPlayerArea spawnPlayerArea;

    private void Awake()
    {
        int rand = Random.Range(0, maps.Count);
        GameObject newTrack = Instantiate(maps[rand].TracksTemplate, transform);
        Instantiate(maps[rand].BuildingTemplate, transform);
        spawnPlayerArea.Selected = rand;
    }
}
