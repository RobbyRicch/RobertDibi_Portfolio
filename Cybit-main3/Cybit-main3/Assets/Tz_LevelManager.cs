using System.Collections;
using UnityEngine;

public class Tz_LevelManager : MonoBehaviour, IProfileSaveable
{
    [Header("Level")]
    [SerializeField] private CustomSceneManager _sceneManager = null;
    [SerializeField] private Transform _startTr;
    [SerializeField] private UIFader _fader;
    [SerializeField] private float _timeToFade = 1.0f;
    [SerializeField] private float _deflectCooldown = 0.25f;
    private bool _isTutorialComplete = false;

    [Header("Dialog")]
    [SerializeField] private Tz_PhaseManager _tzPhaseManager;
    [SerializeField] private Tz_RebootEvent _tzRebootManager;
    [SerializeField] private DialogueManager _dialogueManager;

    private void OnEnable()
    {
        EventManager.OnTutorialComplete += OnTutorialComplete;
    }
    private void Start()
    {
        StartCoroutine(InitializeLevel());
    }
    private void OnDisable()
    {
        EventManager.OnTutorialComplete -= OnTutorialComplete;
    }

    private IEnumerator InitializeLevel()
    {
        // get references
        SaveManager saveManager = SaveManager.Instance;
        saveManager.InitializeLevelData(_startTr);

        Player_Controller playerController = saveManager.Player;
        Player_Deflect deflectRef = playerController.Deflect;

        // pre logic setup
        playerController.LIS.enabled = true;
        yield return null;

        // player setup
        playerController.LIS.IsLinkCompromised = false;
        playerController.ShowCursor(false);
        playerController.Animations.Animator.SetBool("IsFacingRight", true);
        //playerController.ShowUIAndCrosshair(false);
        playerController.LoseGuns();
        playerController.IsInputDisabled = true;
        playerController.IsMovementOnlyDisabled = true;
        playerController.Ultimate.CurrentUltCharge = playerController.Ultimate.MaxUltCharge;
        yield return null;

        if (!_fader.gameObject.activeInHierarchy)
            _fader.gameObject.SetActive(true);

        // logic below
        deflectRef.DeflectCooldown = _deflectCooldown;

        yield return StartCoroutine(_fader.FadeOutRoutine(_timeToFade));
        yield return null;
    }

    private void OnTutorialComplete()
    {
        StartCoroutine(CompleteTutrial());
    }
    private IEnumerator CompleteTutrial()
    {
        _isTutorialComplete = true;
        SaveManager.Instance.SaveGame();

        yield return null;
        _sceneManager.GoToTraining();
    }

    public void LoadData(Profile data)
    {
        // nothing
    }
    public void SaveData(ref Profile data)
    {
        data.HasFinishedTutorial = _isTutorialComplete;
    }
}
