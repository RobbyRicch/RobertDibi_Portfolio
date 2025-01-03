using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class TaskKillBarrier : TaskKill
{
    [Header("Task Components")]
    [SerializeField] private Barrier _barrier;
    [SerializeField] private bool _cutsceneIsPlaying;

    [Header("Cutscene Components")]
    [SerializeField] private float _distanceFromBarrierToStart = 10.0f;
    [SerializeField] private float _cameraFocusOnBarrierTime = 0.75f;
    [SerializeField] private float _cutSceneStartDelay = 0.5f;

    [Header("Spawners")]
    [SerializeField] private SpawnerOfficer _spawnerRef;

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

            // Check and kill all enemies if any are present
            HandleEnemies(_spawnerRef._spawnedOfficer);
            HandleEnemies(_spawnerRef._spawnedRanger);

            if (_cutsceneIsPlaying)
            {
                _spawnerRef.StopAllCoroutines();
            }
            Destroy(_spawnerRef);
        }
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
        _cutsceneIsPlaying = true;
        _playerController = SaveManager.Instance.Player;
        _vCam = _playerController.CurrentVirtualCamera;

        yield return new WaitForSeconds(_cutSceneStartDelay * 2); // *2 for killing before doing scene
        EventManager.InvokeCutscene(true);
        Animator _vcamAnimator = _vCam.GetComponent<Animator>();
        _vcamAnimator.SetBool("Zoom", true);
        _playerController.ShowUIAndCrosshair(false);
        _dynamicAudioManager.FadeOutMusicNormal(0.1f);

        yield return new WaitForSeconds(0.3f);
        _dynamicAudioManager.SwitchMusicPhaseInstantly(_dynamicAudioManager._calmPhase);
        _vCam.Follow = _barrier.transform;
        yield return new WaitUntil(() => Vector3.Distance(_vCam.transform.position, _barrier.transform.position) <= _distanceFromBarrierToStart);

        EventManager.InvokeBarrierDown(_barrier);
        yield return new WaitForSeconds(_barrier.TimeToOpen + _cameraFocusOnBarrierTime);
        _dynamicAudioManager.FadeInMusicNormal(0.1f);

        _vCam.Follow = _playerController.transform;
        _vcamAnimator.SetBool("Zoom", false);
        EventManager.InvokeCutscene(false);
        _playerController.ShowUIAndCrosshair(true);

        /*        gameObject.SetActive(false);
        */
    }
}

