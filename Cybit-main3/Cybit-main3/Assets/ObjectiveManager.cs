using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ObjectiveManager : MonoBehaviour
{
    [SerializeField] private Player_Controller _controller;

    [Header("Objective Type")]
    [SerializeField] bool killObjective;
    [SerializeField] bool breachSurvival;

    [Header("Kill Objective Stats")]
    public float KillGoal;
    public float CurrentKills;
    public bool killObjectiveDone = false;

    [Header("Breach Room Survive")]
    public bool breachObjectiveDone;
    public float timeAlive;
    public TextMeshProUGUI timerText;

    [Header("Reward")]
    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDisable;

    private void Start()
    {
        timeAlive = 0;
        if (breachSurvival && !breachObjectiveDone)
        {
            StartCoroutine(BreachSurvivalTimer());
        }
    }

    private void Update()
    {
        if (CurrentKills >= KillGoal && killObjective)
        {
            killObjectiveDone = true;
        }

        if (killObjectiveDone)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                obj.SetActive(true);
            }

            foreach (GameObject obj in objectsToDisable)
            {
                obj.SetActive(false);
            }
        }
    }

    public void AddToCurrentKills()
    {
        CurrentKills++;
    }

    public IEnumerator BreachSurvivalTimer()
    {
        while (!breachObjectiveDone)
        {
            timeAlive += Time.deltaTime;
            UpdateTimerText();
            yield return null;
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeAlive / 60f);
        int seconds = Mathf.FloorToInt(timeAlive % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void CalculateBreachReward()
    {
        _controller.LIS.MendLinkIntegrity(timeAlive);
        Debug.Log("Added Integrity : " + timeAlive);
    }
}
