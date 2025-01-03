using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float _startTime;
    private float _elapsedTime;
    private bool _isTimerRunning = false;

    public void StartTimer()
    {
        _startTime = Time.time;
        _isTimerRunning = true;
    }
    public void StopTimer()
    {
        _isTimerRunning = false;
        _elapsedTime = Time.time - _startTime;
    }
    public void ResetTimer()
    {
        _elapsedTime = 0f;
        _isTimerRunning = false;
    }
    public float GetElapsedTime()
    {
        return _elapsedTime;
    }
    public bool IsRunning()
    {
        return _isTimerRunning;
    }
}
