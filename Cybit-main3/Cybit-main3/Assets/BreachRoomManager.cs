using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreachRoomManager : MonoBehaviour
{
    [Header("States")]
    public List<GameObject> enemiesInTheScene;
    public List<GameObject> otherSpawners;
    public List<GameObject> breachSpawners;
    [SerializeField] private Transform breachRoomEntrance;
    [SerializeField] private GameObject BreachVolume;
    [SerializeField] private GameObject _infoHud;
    [SerializeField] private TaskBreach _task;
    private Player_Controller playerRef;
    private Player_Controller _playerInBreach;
    private LinkIntegritySystem _lis;

    public bool transferingToRoom;

    private CinemachineVirtualCameraController _vCamController;
    public CinemachineVirtualCameraController VCamController { get => _vCamController; set => _vCamController = value; }
    
    [SerializeField] private GameObject _glitchOverlay;
    public GameObject GlitchOverlay { get => _glitchOverlay; set => _glitchOverlay = value; }

    [SerializeField] private GameObject _interactionKey;
    public GameObject InteractionKey { get => _interactionKey; set => _interactionKey = value; }

    [SerializeField] private UIFader _fader;
    public UIFader Fader { get => _fader; set => _fader = value; }

    [SerializeField] private GameObject _breachCamera;
    public GameObject BreachCamera { get => _breachCamera; set => _breachCamera = value; }

    public bool playerIsInRoom;
    
    private void OnEnable()
    {
        EventManager.OnInteract += OnInteract;
        //EventManager.OnLinkIntegrityThrehsoldPassed += OnLinkIntegrityThrehsoldPassed;
    }
    private void OnDisable()
    {
        EventManager.OnInteract -= OnInteract;
        //EventManager.OnLinkIntegrityThrehsoldPassed -= OnLinkIntegrityThrehsoldPassed;

        if (playerRef)
        {
            playerRef.InteractionKey.SetActive(false);
            playerRef = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!playerRef)
                playerRef = collision.GetComponent<Player_Controller>();

            if (playerRef)
                playerRef.InteractionKey.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerRef)
            {
                playerRef.InteractionKey.SetActive(false);
                playerRef = null;
            }
            
        }
    }

    private void ActivateSpawners()
    {
        foreach (GameObject spawner in breachSpawners)
            spawner.SetActive(true);
    }
    private void DeactivateSpawner()
    {
        foreach (GameObject spawner in breachSpawners)
            spawner.SetActive(false);

    }

    private void TriggerCutscene()
    {
        _interactionKey.gameObject.SetActive(false);
        _fader.FadeIn();
        _vCamController.DoZoom();
        _playerInBreach.IsInputDisabled = true;
        EventManager.InvokeCutscene(true);
        transferingToRoom = true;
        _lis.IsLinkCompromised = false;
        _playerInBreach.FadeOut();
    }

    private IEnumerator TransferPlayerToRoom()
    {
        TriggerCutscene();

        EventManager.InvokeCloseObjective(true);
        _playerInBreach.IsInBreach = true;
        _playerInBreach.IsInputDisabled = false;
        yield return new WaitForSeconds(1f);
        _playerInBreach.transform.position = breachRoomEntrance.transform.position;
        _glitchOverlay.gameObject.SetActive(true);
        _playerInBreach.FadeIn();
        yield return new WaitForSeconds(1f);
        BreachVolume.SetActive(true);
        _breachCamera.SetActive(true);
        transferingToRoom = false;
        playerIsInRoom = true;
        _vCamController.EndZoom();
        _task.SetTaskComplete(false);
        _infoHud.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _fader.FadeOut();
        EventManager.InvokeCutscene(false);
        yield return new WaitForSeconds(2f);
        ActivateSpawners();
    }
    public IEnumerator TransferPlayerBack()
    {
        _playerInBreach.SetStaticHealthCheat(true);
        _fader.FadeIn();
        _vCamController.DoZoom();
        EventManager.InvokeCutscene(true);
        _playerInBreach.FadeOut();
        yield return new WaitForSeconds(1f);
        _task.SetTaskComplete(true);
        _playerInBreach.gameObject.transform.position = gameObject.transform.position;
        _glitchOverlay.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        BreachVolume.SetActive(false);
        _breachCamera.SetActive(false);
        transferingToRoom = false;
        playerIsInRoom = false;
        _infoHud.gameObject.SetActive(false);
        _fader.FadeOut();
        _playerInBreach.FadeIn();
        yield return new WaitForSeconds(1f);
        _playerInBreach.IsInputDisabled = false;
        _task.SetTaskComplete(true);
        _lis.IsLinkCompromised = true;
        _task.CalculateBreachReward();
        _playerInBreach.ReplanishHealth();
        _vCamController.EndZoom();
        EventManager.InvokeCutscene(false);
        _playerInBreach.SetStaticHealthCheat(false);
        EventManager.InvokeReOpenObjective(true);

        /*for (int i = 0; i < otherSpawners.Count; i++)
            otherSpawners[i].SetActive(true);*/
        DeactivateSpawner();
        _playerInBreach.IsInBreach = false;
        _playerInBreach = null;
        //gameObject.SetActive(false);
    }

    private void OnInteract()
    {
        if (!playerRef)
            return;

        _playerInBreach = playerRef;
        playerRef = null;

        _playerInBreach.CurrentBreachRoom = this;
        _lis = _playerInBreach.LIS;
        StartCoroutine(TransferPlayerToRoom());
    }
    /*private void OnLinkIntegrityThrehsoldPassed()
    {

    }*/
}
