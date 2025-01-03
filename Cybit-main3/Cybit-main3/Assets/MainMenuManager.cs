using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private CustomSceneManager _sceneManager;

    [SerializeField] private TextMeshProUGUI _playTextTMP;
    [SerializeField] private EventTrigger _playTrigger;
    [SerializeField] private GameObject _profilesManager;

    private void OnEnable()
    {
        EventManager.OnProfilesLoaded += OnProfilesLoaded;
    }
    private void OnDisable()
    {
        EventManager.OnProfilesLoaded -= OnProfilesLoaded;
    }

    private void Start()
    {
        _playTextTMP.text = "New Game";
        UpdatePointerClickEvent(_playTrigger, OnPointerClickNewGame);
    }

    private void OnProfilesLoaded(List<Profile> profilesLoaded) // happens only if profilesLoaded is not null or 0
    {
        _playTextTMP.text = "Continue";
        UpdatePointerClickEvent(_playTrigger, OnPointerClickContinue);
    }

    private void UpdatePointerClickEvent(EventTrigger trigger, UnityAction<BaseEventData> action)
    {
        EventTrigger.Entry clickEntry = trigger.triggers.Find(entry => entry.eventID == EventTriggerType.PointerClick);

        if (clickEntry == null)
        {
            clickEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
            trigger.triggers.Add(clickEntry);
        }

        clickEntry.callback.RemoveAllListeners();
        clickEntry.callback.AddListener(action);
    }

    private void OnPointerClickNewGame(BaseEventData eventData)
    {
        _profilesManager.SetActive(true);
    }
    private void OnPointerClickContinue(BaseEventData eventData)
    {
        _sceneManager.HandlePlayTransition();
    }
}
