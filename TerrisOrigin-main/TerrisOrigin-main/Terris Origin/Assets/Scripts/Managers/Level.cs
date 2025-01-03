using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Level 
{

    [SerializeField] private int levelNumber;
    [SerializeField] private string barrier;
    [SerializeField] private List<string> enemies;
    [SerializeField] private int kills;
    [SerializeField] private bool Passed=false;

    public void pass()
    {
        Passed = true;
    }

    public bool GetPassed()
    {
        return Passed;
    }

    public string Getbarrier()
    {
        return barrier;
    }

    public void Setbarrier(string _barrier)
    {
        barrier = _barrier;
    }

    public List<string> GetEnemies()
    {
        return enemies;
    }

    public void AddNewEnemy(string newEnemey)
    {
        enemies.Add(newEnemey);
    }
    public int Getkills()
    {
        return kills;
    }

    public void Setkills(int _kills)
    {
        kills = _kills;
    }

    public void increaseKills()
    {
        kills++;
    }

    public override string ToString()
    {
        return ("Kills: "+kills.ToString()+" "+barrier.ToString());
    }
}
