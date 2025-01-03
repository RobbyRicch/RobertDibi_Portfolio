using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Actions //has to be static and not monobehaviur
{
   
    public static Action<bool> PlayerTaskDone;
    public static Action<bool> SetNewTask;
    public static Action<bool> Timed_UI_Done;
    public static Action<int> EnergyNumberUpdate;
    public static Action<int> DiamondsNumberUpdate;
    public static Action<int> ExpNumberUpdate;
    public static Action<int> LevelNumberUpdate;
    public static Action<int> IconChange;
    
    public enum stateGame { none, Start, Tasks, Slime }

}

