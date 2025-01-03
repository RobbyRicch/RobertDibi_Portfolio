using System.Collections;
using UnityEngine;

public class TrainingRoomManager : MonoBehaviour
{
    [SerializeField] private GunPrimary[] _tier1PrimaryPrefabs;
    [SerializeField] private GunSideArm[] _tier1SideArmPrefabs;
    [SerializeField] private Transform[] _primaryGunsTr;
    [SerializeField] private Transform[] _sideArmsTr;
    [SerializeField] private Transform _startTr;
    [SerializeField] private float _timeToFade = 1.0f;
    [SerializeField] private float _roomEntranceTime = 3.0f;

    [SerializeField] private UIFader _fader;
    [SerializeField] private Player_Controller _playerController;
    [SerializeField] private Animator _camAnimator;

    private void Start()
    {
        StartCoroutine(InitializeLevel());
    }
    private IEnumerator InitializeLevel()
    {
        // get references
        SaveManager saveManager = SaveManager.Instance;
        saveManager.InitializeLevelData(_startTr);

        Player_Controller playerController = saveManager.Player;
        _playerController = playerController;

        SkillTreeManagerNew skillManager = saveManager.SkillManager;

        // pre logic setup
        _camAnimator.SetTrigger("WakeUp"); // vCam animator
        yield return null;

        // player setup
        playerController._hazyVision.SetActive(false);
        yield return null;

        saveManager.LoadGame();
        yield return null;

        if (!_fader.gameObject.activeInHierarchy)
            _fader.gameObject.SetActive(true);

        /*if (playerController.IsFirstTimeInTraining)
        {
            _annFirstTimeEntryDialogue.SetActive(true);
        }
        else
        {
            _annFirstTimeEntryDialogue.SetActive(false);

        }*/

        // need to fix all under
        _playerController.Revive(false);
        //EventManager.InvokePayCurrency(saveManager.GameData.Currency);
        //saveManager.GameData.Currency = 0;
        _playerController.LoseGuns();
        _playerController.SetTrainingCheats(true);
        _playerController.IsExternalControl = true;
        InstansiateGuns(saveManager);
        //saveManager.GameData.CurrentIntegrity = saveManager.GameData.MaxIntegrity;
        //saveManager.GameData.EquippedPrimaryId = string.Empty;
        //saveManager.GameData.EquippedSideArmId = string.Empty;
        playerController.transform.position = _startTr.transform.position;
        skillManager.ResetAllSkills();
        StartCoroutine(_fader.FadeOutRoutine(_timeToFade));
        StartCoroutine(EntranceCutscene());
        yield return null;
    }
    private void OnDisable()
    {
        _playerController.SetTrainingCheats(false);
    }

    private void InstansiateGuns(SaveManager saveManager)
    {
        // need to fix all under
        for (int i = 0; i < _tier1PrimaryPrefabs.Length; i++)
        {
            /*if (saveManager.GameData.UnlockableGunIds.ContainsKey(_tier1PrimaryPrefabs[i].Uid))
            {
                GunPrimary unlockedPrimary = Instantiate(_tier1PrimaryPrefabs[i], _primaryGunsTr[i]);
                unlockedPrimary.Pickup.WorldTr = _primaryGunsTr[i];
                continue;
            }*/
        }

        for (int i = 0; i < _tier1SideArmPrefabs.Length; i++)
        {
            /*if (saveManager.GameData.UnlockableGunIds.ContainsKey(_tier1SideArmPrefabs[i].Uid))
            {
                GunSideArm unlockedSideArm = Instantiate(_tier1SideArmPrefabs[i], _sideArmsTr[i]);
                unlockedSideArm.Pickup.WorldTr = _sideArmsTr[i];
                continue;
            }*/
        }
    }
    private IEnumerator EntranceCutscene()
    {
        EventManager.InvokeCutscene(true);

        while (_roomEntranceTime > 0)
        {
            _playerController.FakeMove(Vector2.down, 120);
            _roomEntranceTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        EventManager.InvokeCutscene(false);
        _playerController.IsExternalControl = false;
    }
}