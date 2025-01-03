using UnityEngine;

public interface IPersistable
{
    public bool ShouldBeActiveOnStart { get; set; }

    public void OnStartRun();
}
