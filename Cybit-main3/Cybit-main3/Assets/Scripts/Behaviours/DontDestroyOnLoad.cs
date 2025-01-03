using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour, IPersistable
{
    [SerializeField] private bool _shouldBeActiveOnStart = true;
    public bool ShouldBeActiveOnStart { get => _shouldBeActiveOnStart; set => _shouldBeActiveOnStart = value; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OnStartRun()
    {
        if (_shouldBeActiveOnStart)
            gameObject.SetActive(true);
    }
    public MonoBehaviour GetMonoBehaviour()
    {
        return this;
    }
}
