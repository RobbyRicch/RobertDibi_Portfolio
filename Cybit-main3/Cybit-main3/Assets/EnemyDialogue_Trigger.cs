using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDialogue_Trigger : MonoBehaviour
{
    [Header("Dialogue Ref")]
    public Dialogue _dialogue;

    [Header("Trigger Type")]
    [SerializeField] private bool _applyOnStart;
    [SerializeField] private bool _applyOnTrigger;
    [SerializeField] private bool _unavailable;

    private EnemyDialogueManager _dialogueManagerRef;
    public void TriggerDialogue()
    {
        _dialogueManagerRef = FindObjectOfType<EnemyDialogueManager>();
        _dialogueManagerRef.StartDialogue(_dialogue);
    }

    private void OnEnable()
    {
        if (_applyOnStart && !_unavailable)
        {
            TriggerDialogue();
        }

    }

    private void Update()
    {
        //if we want to make dialogue manually skipable in the future T.T ~ Robby
        if (_dialogueManagerRef._isDialogueRunning && _dialogueManagerRef._isSkippable)
        {
            if (Input.anyKeyDown)
            {
                _dialogueManagerRef.DisplayNextSentence();

            }

        }
    }
}
