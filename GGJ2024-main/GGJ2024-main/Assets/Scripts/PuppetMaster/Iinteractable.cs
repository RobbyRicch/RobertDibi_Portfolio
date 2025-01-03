using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Iinteractable
{
    void OnInteract();
    void OnRelease();
    void InteractCD();
}
