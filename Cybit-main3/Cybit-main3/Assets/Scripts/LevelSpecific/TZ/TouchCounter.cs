using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCounter : MonoBehaviour
{
    [Header("Touches")]
    public int _currentTouches;
    public int _neededTouches;

    [Header("Bools")]
    public bool _touchesCompleted;
    public bool _countTouches;


    private void Update()
    {
        if (_currentTouches >= _neededTouches)
        {
            _touchesCompleted = true;
        }
        else
        {
            _touchesCompleted = false;
        }
    }
}
