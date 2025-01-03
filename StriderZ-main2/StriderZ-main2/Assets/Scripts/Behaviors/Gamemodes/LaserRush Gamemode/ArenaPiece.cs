using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaPiece : MonoBehaviour
{
    [SerializeField] private bool _shouldFall;
    public bool ShouldFall { get => _shouldFall; set => _shouldFall = value; }
    public MeshRenderer[] Renderers;
    public GameObject VFX;

    private void Update()
    {
        if (_shouldFall)
            GroundScrumbleManager.Instance.ArenaScrumbler.ApplyGravity(transform);
    }
}
