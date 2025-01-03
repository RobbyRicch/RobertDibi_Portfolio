using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpawnPlayerArea : MonoBehaviour
{
    public int Selected;
    [SerializeField] private List<GameObject> _SpawnAreas;
    [SerializeField] private Volume GlobalVolume;
    [SerializeField] private List<VolumeProfile> volumeProfiles;
    void Start()
    {
        _SpawnAreas[Selected].SetActive(true);
        GlobalVolume.profile = volumeProfiles[Selected];
    }
}
