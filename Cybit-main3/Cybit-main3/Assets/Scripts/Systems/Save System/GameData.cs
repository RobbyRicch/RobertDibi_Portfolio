using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string ActiveProfileName;
    public List<string> ProfileNames;

    public GameData()
    {
        ActiveProfileName = string.Empty;
        ProfileNames = new();
    }
}
