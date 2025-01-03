using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaskKillDoubleBarrier : TaskKill
{
    [Header("Task Components")]
    [SerializeField] private Barrier[] _barriers;

    [Header("Cutscene Components")]
    [SerializeField] private Transform _newCamTarget;
    [SerializeField] private float _distanceFromBarrierToStart = 10.0f;
    [SerializeField] private float _cameraFocusOnBarrierTime = 0.75f;
    [SerializeField] private float _cutSceneStartDelay = 0.5f;

    [Header("Other Components")]
    [SerializeField] private SpawnerOfficer _spawnerRef1;
    [SerializeField] private SpawnerOfficer _spawnerRef2;

    [Header("Level Refs")]
    [SerializeField] private DynamicAudioManager _dynamicAudioManager;
    [SerializeField] private DialogueManager _dialogueManager;

    [Header("Dialogues")]
    [SerializeField] private List<GameObject> _AgentDialogues;
    [SerializeField] private List<GameObject> _AnnDialogues;

    private void Update()
    {
        if (!_isTaskComplete && _currentKills >= _killGoal)
        {
            _isTaskComplete = true;
            StartCoroutine(BarrierDownCutscene());
            StartCoroutine(KillSpawners());  // Initiate killing spawners
        }
    }

    private IEnumerator KillSpawners()
    {
        yield return new WaitForSeconds(1f);

        // Handle destroying enemies from both spawners
        HandleEnemies(_spawnerRef1._spawnedOfficer);
        HandleEnemies(_spawnerRef1._spawnedRanger);
        HandleEnemies(_spawnerRef2._spawnedOfficer);
        HandleEnemies(_spawnerRef2._spawnedRanger);

        // Destroy spawners after clearing enemies
        Destroy(_spawnerRef1.gameObject);
        Destroy(_spawnerRef2.gameObject);
    }

    // Method to handle killing and destroying enemies
    private void HandleEnemies<T>(List<T> enemies) where T : EnemyBase
    {
        foreach (T enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.Die(enemy);
                enemy._isAlive = false;

                NavMeshAgent enemyAgent = enemy.GetComponent<NavMeshAgent>();
                EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();

                if (enemyAgent != null) enemyAgent.enabled = false;
                if (enemyScript != null) enemyScript.enabled = false;

                Destroy(enemy.gameObject);  // Remove the enemy from the game
            }
        }
    }

    private IEnumerator BarrierDownCutscene()
    {
        _playerController = SaveManager.Instance.Player;
        _vCam = _playerController.CurrentVirtualCamera;

        yield return new WaitForSeconds(_cutSceneStartDelay);

        // Start the cutscene
        EventManager.InvokeCutscene(true);
        Animator _vcamAnimator = _vCam.GetComponent<Animator>();
        _dynamicAudioManager.SwitchMusicPhaseInstantly(_dynamicAudioManager._calmPhase);
        _vcamAnimator.SetBool("Zoom", true);
        _playerController.ShowUIAndCrosshair(false);

        // Focus the camera on the new target
        _vCam.Follow = _newCamTarget;
        yield return new WaitUntil(() => Vector3.Distance(_vCam.transform.position, _newCamTarget.position) <= _distanceFromBarrierToStart);

        // Trigger the barriers to go down
        EventManager.InvokeBarrierDown(_barriers[0]);
        EventManager.InvokeBarrierDown(_barriers[1]);

        // Wait for the barriers to finish opening
        yield return new WaitForSeconds(_barriers[0].TimeToOpen + _cameraFocusOnBarrierTime);

        // Return camera control to the player
        _vCam.Follow = _playerController.transform;
        EventManager.InvokeCutscene(false);
        _vcamAnimator.SetBool("Zoom", false);
        _playerController.ShowUIAndCrosshair(true);

        // Deactivate this game object after the cutscene
        gameObject.SetActive(false);
    }
}
