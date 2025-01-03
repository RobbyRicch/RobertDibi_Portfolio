using UnityEngine;

public class TaskKill : TaskBase
{
    [SerializeField] protected int _killGoal;
    [SerializeField] protected int _currentKills;

    public void AddToCurrentKills()
    {
        _currentKills++;
    }
}
