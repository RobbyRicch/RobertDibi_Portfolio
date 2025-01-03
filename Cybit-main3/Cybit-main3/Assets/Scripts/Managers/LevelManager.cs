using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _breachDoors;
    [SerializeField] private Transform _startTr;
    [SerializeField] private DialogueManager _levelDialogueManager;
    [SerializeField] private UIFader _fader;
    [SerializeField] private float _timeToFade = 1.0f;
    [SerializeField] private bool _shouldActivateSpawners = true;

    [Header("Info Canvases")]
    [SerializeField] private InfoCanvasManager[] _infocanvases;

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

        // pre logic setup
        playerController.IsFirstTimeInTraining = false;
        playerController.LIS.enabled = true;
        yield return null;

        /*if (SaveManager.Instance.GameData.UnlockableGunIds.Keys.Count > 0) need to fix weapon creation
        {
            StartCoroutine(saveManager.InstantiateWeaponsWithDelay(newPrimaryTr, newSideArmTr));
        }*/

        // player setup
        //playerController.LIS._currentIntegrity = saveManager.GameData.CurrentIntegrity;
        playerController.LIS.IsLinkCompromised = true;
        EventManager.InvokeInitializeLink();

        playerController.transform.position = _startTr.position;
        playerController.ShowUIAndCrosshair(true);
        EventManager.InvokeUpdateCurrency();
        yield return null;

        if (!_fader.gameObject.activeInHierarchy)
            _fader.gameObject.SetActive(true);

        yield return StartCoroutine(_fader.FadeOutRoutine(_timeToFade));
        yield return null;

        /*if (_shouldActivateSpawners)
            _allSpawners[0].gameObject.SetActive(true);*/
    }
}