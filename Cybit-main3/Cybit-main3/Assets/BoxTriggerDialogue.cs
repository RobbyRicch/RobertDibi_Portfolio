using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoxTriggerDialogue : MonoBehaviour
{
    [Header("Trigger")]
    [SerializeField] private BoxCollider2D _currentColider;

    [Header("Level Refrences")]
    [SerializeField] private GameObject _targetDialogue;
    [SerializeField] private DialogueManager _dialogueManagerRef;
    [SerializeField] private DynamicAudioManager _dynamicAudioManager;
    [SerializeField] public float _timeToEnd;
    [SerializeField] public bool _shouldEnd;
    [SerializeField] public bool _playerWillSpeak;
    [SerializeField] public bool _shouldChangeBGM;
    [SerializeField] public string _objectiveString;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            _currentColider.enabled = false;

            if (_playerWillSpeak)
            {
            StartCoroutine(PlayerDialogue());

            }
            else
            {
                StartCoroutine(HallwaylDialogue());

            }

            if (_shouldChangeBGM)
            {
                _dynamicAudioManager.SwitchMusicPhaseInstantly(_dynamicAudioManager._fightPhase);
            }
        }


    }

    private IEnumerator PlayerDialogue()
    {
        _targetDialogue.SetActive(true);
        yield return new WaitForSeconds(_timeToEnd);

        if (_shouldEnd)
        {
        _dialogueManagerRef.EndDialogue();
        EventManager.InvokeNewObjective(_objectiveString);

        }
    }

    private IEnumerator HallwaylDialogue()
    {
        _dialogueManagerRef.ManualCanvasEnter();
        _targetDialogue.SetActive(true);
        yield return new WaitForSeconds(_timeToEnd); 
        _dialogueManagerRef.ManualCanvasExitNormal();
        _dialogueManagerRef.EndDialogue();
        EventManager.InvokeNewObjective(_objectiveString);
    }
}
