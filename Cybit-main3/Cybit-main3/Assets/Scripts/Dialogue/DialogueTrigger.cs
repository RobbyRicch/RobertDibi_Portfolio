using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Ref")]
    public Dialogue _dialogue;

    [Header("Trigger Type")]
    [SerializeField] private bool _applyOnStart;
    [SerializeField] private bool _applyOnTrigger;
    [SerializeField] private bool _unavailable;

    public void TriggerDialogue()
    {
        _dialogue._dialogueManager.StartDialogue(_dialogue);
    }

    private void OnEnable()
    {
        if (_applyOnStart && !_unavailable)
        {
            TriggerDialogue();
        }

    }

/*    private void Update()
    {
        //if we want to make dialogue manually skipable in the future T.T ~ Robby
        if (_dialogue._dialogueManager._isDialogueRunning && _dialogue._dialogueManager._isSkippable)
        {
            if (Input.anyKeyDown)
            {
                _dialogue._dialogueManager.DisplayNextSentence();

            }

        }
    }*/
}
