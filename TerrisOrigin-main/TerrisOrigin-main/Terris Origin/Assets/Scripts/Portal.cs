using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Portal Locations")]
    [SerializeField] GameObject portalStart;
    [SerializeField] GameObject portalEnd;
    private bool _teleported;

    public bool Teleported { get => _teleported; }

    private void Awake()
    {
        portalEnd.SetActive(false);
        portalStart.SetActive(false);
    }

    public void Activate()
    {
        portalEnd.SetActive(true);
        portalStart.SetActive(true);
    }

    public void Teleport(GameObject target)
    {
        if (!_teleported)
        {
            Debug.Log("Caij Teleported");
            target.transform.position = portalEnd.transform.position;
            _teleported = true;
            portalEnd.SetActive(false);
            portalStart.SetActive(false);
        }
    }
}
