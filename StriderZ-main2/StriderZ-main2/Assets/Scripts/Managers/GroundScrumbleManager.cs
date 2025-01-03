using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScrumbleManager : MonoBehaviour
{
    private static GroundScrumbleManager _instance;
    public static GroundScrumbleManager Instance => _instance;

    [SerializeField] private ArenaScrumbler _arenaScrumbler;
    public ArenaScrumbler ArenaScrumbler => _arenaScrumbler;

    [SerializeField] private Material _groundFlashMat;

    private bool _isArenaScrumbling = false;
    public bool IsArenaScrumbling => _isArenaScrumbling;

    private void Awake()
    {
        _instance = this;
    }
    private void Update()
    {
        if (_arenaScrumbler == null)
            _arenaScrumbler = ArenaScrumbler.Instance;
    }

    public void ScrumbleArena()
    {
        ArenaScrumbler.BreakCurrentRing();
        _isArenaScrumbling = true;
    }
}
