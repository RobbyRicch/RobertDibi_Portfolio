using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tz_PhaseManager : MonoBehaviour
{
    [Header("Goal : Run Down all the phases of the tutorial")]
    [Header("Dialogues")]
    [SerializeField] public List<GameObject> _annMoveDialogue;
    [SerializeField] public List<GameObject> _agentMoveDialogue;

    [SerializeField] public List<GameObject> _annMeleeDialogue;
    [SerializeField] public List<GameObject> _agentMeleeDialogue;

    [SerializeField] public List<GameObject> _annLinkIntegrityDialogue;
    [SerializeField] public List<GameObject> _agentLinkIntegrityDialogue;

    [SerializeField] public List<GameObject> _annCyberneticsDialogue;
    [SerializeField] public List<GameObject> _agentCyberneticsDialogue;

    [SerializeField] public List<GameObject> _annFinishDialogue;
    [SerializeField] public List<GameObject> _agentFinishDialogue;

    [SerializeField] public DialogueManager _dialogueManager;

    [Header("Movement - Refs")]
    [SerializeField] public bool[] _checkPoints;
    [SerializeField] public GameObject _checkpointGroupGO;
    [SerializeField] public Player_Controller _playerRef;
    [SerializeField] public Tz_MovePoint _finalPointRef;
    [SerializeField] public bool _movementPhaseIsInProgress;
    [SerializeField] public GameObject _staminaHUDGO;

    [Header("Melee - Refs")]
    [SerializeField] public bool _meleePhaseConvoIsInProgress;
    [SerializeField] public GameObject _guardsForMeleeTest;
    [SerializeField] public GameObject _healthHUDGO;

    [Header("Integrity Link - Refs")]
    [SerializeField] public LinkIntegritySystem _linkIntegritySystem;
    [SerializeField] public GameObject _linkIntegrityHUD;
    [SerializeField] public bool _linkIntegrityConvoIsInProgress;

    [Header("Cybernetics FOCUS - Refs")]
    [SerializeField] public bool _cyberneticsFocusConvoIsInProgress;
    [SerializeField] public GameObject _guardsForFocusTest;
    [SerializeField] public GameObject _focusHUDGO;

    [Header("Cybernetics ULT - Refs")]
    [SerializeField] public bool _cyberneticsUltConvoIsInProgress;
    [SerializeField] public GameObject _guardsForUltTest;
    [SerializeField] public GameObject _ultHUDGO;
    [SerializeField] public GameObject _middlePoint;
    [SerializeField] public GameObject _fakeWall;

    [Header("Cybernetics ULT - Refs")]
    [SerializeField] public bool _cyberneticsDeflectConvoIsInProgress;
    [SerializeField] public GameObject _guardsForDeflectTest;
    [SerializeField] public GameObject _deflectHUDGO;
    [SerializeField] public GameObject _fakeWalls;

    [Header("Finish Tutorial - Refs")]
    [SerializeField] public bool _finishingDialogue;
    [SerializeField] public Animator _cinemachineAnimatorRef;
    [SerializeField] public GameObject _hazyVision;
    [SerializeField] public GameObject _crosshairRef;
    [SerializeField] private Tz_LevelManager _LevelManager;

    [Header("Skips")]
    [SerializeField] private bool _introConvoFinished;
    [SerializeField] public bool _movementFinished;
    [SerializeField] public bool _meleeFinished;
    [SerializeField] public bool _linkFinished;
    [SerializeField] public bool _focusFinished;
    [SerializeField] public bool _ultFinished;
    [SerializeField] public bool _deflectFinished;
    private void Start()
    {

        if (!_introConvoFinished)
        {
            StartCoroutine(AnnAndAgentConvo());

        }
        _crosshairRef.SetActive(false);
    }



    private IEnumerator AnnAndAgentConvo()
    {

        _dialogueManager.ManualCanvasEnter();
        _annMoveDialogue[0].SetActive(true);


        yield return new WaitForSeconds(40);
        _introConvoFinished = true;
    }

    private void Update()
    {
        if (_introConvoFinished && !_movementPhaseIsInProgress && !_movementFinished)
        {
            StartCoroutine(MovementPhase());
        }

        if (_movementFinished && !_meleePhaseConvoIsInProgress && !_meleeFinished)
        {
            StartCoroutine(MeleePhase());
        }

        if (_movementFinished && _meleeFinished && !_linkIntegrityConvoIsInProgress && !_linkFinished)
        {
            StartCoroutine(LinkIntegrityPhase());
        }

        if (_movementFinished && _meleeFinished && _linkFinished && !_cyberneticsFocusConvoIsInProgress && !_focusFinished)
        {
            StartCoroutine(CyberneticsPhaseOne());
        }

        if (_movementFinished && _meleeFinished && _linkFinished && _focusFinished && !_cyberneticsUltConvoIsInProgress && !_ultFinished)
        {
            StartCoroutine(CyberneticsPhaseTwo());
        }

        if (_movementFinished && _meleeFinished && _linkFinished && _focusFinished && _ultFinished && !_cyberneticsDeflectConvoIsInProgress && !_deflectFinished)
        {
            StartCoroutine(CyberneticsPhaseThree());
        }

        if (_movementFinished && _meleeFinished && _linkFinished && _focusFinished && _ultFinished && _deflectFinished && !_finishingDialogue)
        {
            StartCoroutine(EndOfTutorial());
        }

        if (_playerRef._currentHealth < 28)
        {
            _playerRef._currentHealth = 28;
        }
    }

    //Phase One - Movement
    private IEnumerator MovementPhase()
    {
        _linkIntegritySystem._integrityFailureFactor = 0;
        _linkIntegritySystem._currentIntegrity = 12;
        _linkIntegritySystem.IsLinkCompromised = true;
        _movementPhaseIsInProgress = true;
        _annMoveDialogue[4].SetActive(true);
        yield return new WaitForSeconds(5);
        _dialogueManager.ManualCanvasExitFaded();
        _playerRef.IsInputDisabled = false;
        _playerRef.IsMovementOnlyDisabled = false;
        EventManager.InvokeNewObjective("USE <color=#00FF00>[WASD]</color> TO WALK TO THE CIRCLES OR PRESS <color=#00FF00>[SHIFT]</color> TO DASH");
        _checkpointGroupGO.SetActive(true);
        _crosshairRef.SetActive(true);
        _staminaHUDGO.SetActive(true);

    }

    //Phase Two - Melee
    private IEnumerator MeleePhase()
    {
        EventManager.InvokeCloseObjective(true);
        _linkIntegritySystem._hudInMiddleOfScreen = true;
        _meleePhaseConvoIsInProgress = true;
        _linkIntegritySystem._currentIntegrity = 50;
        _linkIntegritySystem.IsLinkCompromised = true;
        _linkIntegrityHUD.SetActive(true);

        StartCoroutine(_linkIntegritySystem.LinkHudPulse());
        yield return new WaitForSeconds(2);
        _linkIntegritySystem._hudInMiddleOfScreen = false;
        _checkpointGroupGO.SetActive(false);
        _agentMeleeDialogue[0].SetActive(true);
        yield return new WaitForSeconds(2);
        _dialogueManager.ManualCanvasEnter();
        /*   _annMeleeDialogue[0].SetActive(true);
           yield return new WaitForSeconds(3.5f);
           _dialogueManager.ManualCanvasFade();
           _agentMeleeDialogue[1].SetActive(true);
           yield return new WaitForSeconds(4);
           _dialogueManager.ManualCanvasSpeak();
           _annMeleeDialogue[1].SetActive(true);
           yield return new WaitForSeconds(10);
           _dialogueManager.ManualCanvasFade();
           _agentMeleeDialogue[2].SetActive(true);
           yield return new WaitForSeconds(3.5f);
           _dialogueManager.ManualCanvasSpeak();
           _annMeleeDialogue[2].SetActive(true);
           yield return new WaitForSeconds(2f);
           _dialogueManager.ManualCanvasFade();
           _agentMeleeDialogue[3].SetActive(true);
           yield return new WaitForSeconds(4.5f);
           _dialogueManager.ManualCanvasSpeak();
           _annMeleeDialogue[3].SetActive(true);
           yield return new WaitForSeconds(9f);*/
        yield return new WaitForSeconds(30);

        _dialogueManager.ManualCanvasExitNormal();
        _guardsForMeleeTest.SetActive(true);
        _healthHUDGO.SetActive(true);
        EventManager.InvokeNewObjective("TAP <color=#00FF00>[RMB]</color> TO COMBO MELEE ATTACKS");
    }

    //Phase Three - LinkIntegrity
    private IEnumerator LinkIntegrityPhase()
    {
        _linkIntegrityConvoIsInProgress = true;
        _guardsForMeleeTest.SetActive(false);

        if (!_linkIntegrityHUD.activeInHierarchy)
        {

            _linkIntegritySystem._currentIntegrity = 50;
            _linkIntegritySystem.IsLinkCompromised = true;
            _linkIntegrityHUD.SetActive(true);
        }
        EventManager.InvokeCloseObjective(true);

        _linkIntegritySystem._hudInMiddleOfScreen = true;
        _linkIntegritySystem._currentIntegrity = 100;
        StartCoroutine(_linkIntegritySystem.LinkHudPulse());
        yield return new WaitForSeconds(1);
        _linkIntegritySystem._hudInMiddleOfScreen = false;
        _agentLinkIntegrityDialogue[0].SetActive(true);
        yield return new WaitForSeconds(2);
        _dialogueManager.ManualCanvasEnter();
        /*        _annLinkIntegrityDialogue[0].SetActive(true);
                yield return new WaitForSeconds(3);
                _dialogueManager.ManualCanvasFade();
                _agentLinkIntegrityDialogue[1].SetActive(true);
                yield return new WaitForSeconds(4.5f);
                _dialogueManager.ManualCanvasSpeak();
                _annLinkIntegrityDialogue[1].SetActive(true);
                yield return new WaitForSeconds(18);
                _dialogueManager.ManualCanvasFade();
                _agentLinkIntegrityDialogue[2].SetActive(true);
                yield return new WaitForSeconds(2.5f);
                _dialogueManager.ManualCanvasSpeak();
                _annLinkIntegrityDialogue[2].SetActive(true);*/
        yield return new WaitForSeconds(30f);
        _linkFinished = true;

    }

    private IEnumerator CyberneticsPhaseOne()
    {
        _cyberneticsFocusConvoIsInProgress = true;

        if (!_linkIntegrityHUD.activeInHierarchy)
        {

            _linkIntegritySystem._currentIntegrity = 100;
            _linkIntegritySystem.IsLinkCompromised = true;
            _linkIntegrityHUD.SetActive(true);
        }
        _linkIntegritySystem._currentIntegrity = 150;
        StartCoroutine(_linkIntegritySystem.LinkHudPulse());
        yield return new WaitForSeconds(1);
        _linkIntegritySystem._hudInMiddleOfScreen = false;
        _dialogueManager.ManualCanvasFade();
        _agentCyberneticsDialogue[0].SetActive(true);
        /*        yield return new WaitForSeconds(5.5f);
                _dialogueManager.ManualCanvasSpeak();
                _annCyberneticsDialogue[0].SetActive(true);
                yield return new WaitForSeconds(13);
                _dialogueManager.ManualCanvasFade();
                _agentCyberneticsDialogue[1].SetActive(true);
                yield return new WaitForSeconds(2.5f);
                _dialogueManager.ManualCanvasSpeak();
                _annCyberneticsDialogue[1].SetActive(true);
                yield return new WaitForSeconds(16.5f);
                _playerRef.CurrentFocus = 200;*/
        yield return new WaitForSeconds(30f);
        _playerRef.Focus.CurrentFocus = _playerRef.Focus.MaxFocus;
        EventManager.InvokeNewObjective("PRESS <color=#00FF00>[F]</color> TO TOGGLE HYPER-FOCUS AND KILL THE GUARDS");
        _dialogueManager.ManualCanvasExitNormal();
        _guardsForFocusTest.SetActive(true);
        _focusHUDGO.SetActive(true);
    }

    private IEnumerator CyberneticsPhaseTwo()
    {
        _cyberneticsUltConvoIsInProgress = true;
        EventManager.InvokeCloseObjective(true);

        if (!_linkIntegrityHUD.activeInHierarchy)
        {

            _linkIntegritySystem._currentIntegrity = 150;
            _linkIntegritySystem.IsLinkCompromised = true;
            _linkIntegrityHUD.SetActive(true);
        }
        _guardsForFocusTest.SetActive(true);
        _linkIntegritySystem._currentIntegrity = 200;
        StartCoroutine(_linkIntegritySystem.LinkHudPulse());
        _dialogueManager.ManualCanvasEnter();
        _annCyberneticsDialogue[2].SetActive(true);
        /* yield return new WaitForSeconds(6f);
                _dialogueManager.ManualCanvasFade();
                _agentCyberneticsDialogue[2].SetActive(true);
                yield return new WaitForSeconds(4);
                _dialogueManager.ManualCanvasSpeak();
                _annCyberneticsDialogue[3].SetActive(true);
                yield return new WaitForSeconds(15);
                _playerRef.IsMovementOnlyDisabled = true;
                _playerRef.transform.position = _middlePoint.transform.position;
                yield return new WaitForSeconds(1);
                _fakeWall.SetActive(true);*/
        yield return new WaitForSeconds(25);
        _guardsForUltTest.SetActive(true);
        _dialogueManager.ManualCanvasExitNormal();
        EventManager.InvokeNewObjective("PRESS <color=#00FF00>[X]</color> TO TOGGLE ULT THEN <color=#00FF00>[RMB]</color> TO RELEASE");
        _playerRef.Ultimate.CurrentUltCharge = _playerRef.Ultimate.MaxUltCharge;
        _playerRef.IsMovementOnlyDisabled = false;
        _ultHUDGO.SetActive(true);

    }

    private IEnumerator CyberneticsPhaseThree()
    {
        _cyberneticsDeflectConvoIsInProgress = true;
        EventManager.InvokeCloseObjective(true);

        if (!_linkIntegrityHUD.activeInHierarchy)
        {

            _linkIntegritySystem._currentIntegrity = 200;
            _linkIntegritySystem.IsLinkCompromised = true;
            _linkIntegrityHUD.SetActive(true);
        }
        _linkIntegritySystem._currentIntegrity = 215;
        _guardsForUltTest.SetActive(false);
        StartCoroutine(_linkIntegritySystem.LinkHudPulse());
        _agentCyberneticsDialogue[3].SetActive(true);
        _fakeWall.SetActive(false);
        yield return new WaitForSeconds(4f);
        _annCyberneticsDialogue[4].SetActive(true);
        _dialogueManager.ManualCanvasEnter();
       /* yield return new WaitForSeconds(14f);
        _dialogueManager.ManualCanvasFade();
        _agentCyberneticsDialogue[4].SetActive(true);
        yield return new WaitForSeconds(4.5f);
        _dialogueManager.ManualCanvasSpeak();
        _annCyberneticsDialogue[5].SetActive(true);
        yield return new WaitForSeconds(5f);
        _dialogueManager.ManualCanvasFade();
        _agentCyberneticsDialogue[5].SetActive(true);*/
        yield return new WaitForSeconds(18f);
        _playerRef.IsMovementOnlyDisabled = true;
        _playerRef.transform.position = _middlePoint.transform.position;
        yield return new WaitForSeconds(1);
        _fakeWalls.SetActive(true);
        _playerRef.Deflect.CanDeflect = true;
        _deflectHUDGO.SetActive(true);
        _playerRef.IsMovementOnlyDisabled = false;
        yield return new WaitForSeconds(8);
        _dialogueManager.ManualCanvasExitFaded();
        _guardsForDeflectTest.SetActive(true);
        _playerRef.Deflect._deflectHudAnimator.SetTrigger("Pulse");
        EventManager.InvokeNewObjective("Press <color=#00FF00>[V]</color> TO ENTER DEFLECT STATE");

    }

    private IEnumerator EndOfTutorial()
    {
        _finishingDialogue = true;
        _guardsForDeflectTest.SetActive(false);
        EventManager.InvokeCloseObjective(true);

        if (!_linkIntegrityHUD.activeInHierarchy)
        {

            _linkIntegritySystem._currentIntegrity = 215;
            _linkIntegritySystem.IsLinkCompromised = true;
            _linkIntegrityHUD.SetActive(true);
        }
        _linkIntegritySystem._currentIntegrity = 250;
        StartCoroutine(_linkIntegritySystem.LinkHudPulse());
        yield return new WaitForSeconds(1);
        _annFinishDialogue[0].SetActive(true);
        _dialogueManager.ManualCanvasEnter();
        yield return new WaitForSeconds(7);
        _cinemachineAnimatorRef.SetTrigger("GoIn");
        _crosshairRef.SetActive(false);
        yield return new WaitForSeconds(2);
        _dialogueManager.ManualCanvasExitNormal();
        _hazyVision.SetActive(true);
        EventManager.InvokeTutorialComplete();
    }
}