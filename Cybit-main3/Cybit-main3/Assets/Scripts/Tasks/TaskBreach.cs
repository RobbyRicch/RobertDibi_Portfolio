using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class TaskBreach : TaskBase
{
    [Header("Breach Room Survive")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float _timeSinceEntrance = 0.0f;

    private void OnEnable()
    {
        StartCoroutine(BreachSurvivalTimer());
        _timeSinceEntrance = 0;
        _isTaskComplete = false;
    }

    private IEnumerator BreachSurvivalTimer()
    {
        while (!_isTaskComplete)
        {
            _timeSinceEntrance += Time.deltaTime;
            UpdateTimerText();
            yield return null;
        }
    }
    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(_timeSinceEntrance / 60f);
        int seconds = Mathf.FloorToInt(_timeSinceEntrance % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetTaskComplete(bool isComplete)
    {
        _isTaskComplete = isComplete;
    }
    public void CalculateBreachReward()
    {
        _playerController.LIS.MendLinkIntegrity(_timeSinceEntrance);

        if (_playerController.InteractionKey.gameObject.activeInHierarchy)
            _playerController.InteractionKey.gameObject.SetActive(false);

        Debug.Log("Added Integrity : " + _timeSinceEntrance);
    }
}
