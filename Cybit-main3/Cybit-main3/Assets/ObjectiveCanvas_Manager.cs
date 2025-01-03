using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectiveCanvas_Manager : MonoBehaviour
{
    //This script manages all UI related info to the current Objective


    [Header("UI Refrences")]
    [SerializeField] private TextMeshProUGUI _hudText;
    [SerializeField] private Animator _hudAnimator;
    private string _lastText;
    private void Start()
    {
        _hudText.text = " ";
    }
    private void OnEnable()
    {
        EventManager.OnNewObjective += OnNewObjective;
        EventManager.OnCloseObjective += OnCloseObjective;
        EventManager.OnReOpenObjective += OnReOpenObjective;
    }
    private void OnDisable()
    {
        EventManager.OnNewObjective -= OnNewObjective;
        EventManager.OnCloseObjective -= OnCloseObjective;
        EventManager.OnReOpenObjective -= OnReOpenObjective;
    }

    private void OnNewObjective(string obj)
    {
        _hudText.text = obj;
        _lastText = obj;
        _hudAnimator.SetTrigger("SwingBy");
        StartCoroutine(TriggerHudAnimation());
    }
    private void OnCloseObjective(bool close)
    {
        if (close)
        {
            _hudText.text = " ";

        }
    }

    private void OnReOpenObjective(bool open)
    {
        if (open)
        {
            _hudText.text = _lastText;

        }
    }

    private IEnumerator TriggerHudAnimation()
    {
        yield return new WaitForSeconds(2);
        _hudAnimator.ResetTrigger("SwingBy");
        _hudAnimator.ResetTrigger("Leave");
    }
}
