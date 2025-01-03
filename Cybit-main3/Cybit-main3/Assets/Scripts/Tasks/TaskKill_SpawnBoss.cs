using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TaskKill_SpawnBoss : TaskKill
{
    [Header("Boss To Spawn")]
    [SerializeField] private GameObject _bossGO;

    [Header("Timing")]
    [SerializeField] private float _timeToSpawnBoss;
    [SerializeField] private float _timeToDisableVFX;

    [Header("Spawner Components")]
    [SerializeField] private Transform _spawnPoint;

    [Header("Spawn VFXs")]
    [SerializeField] private GameObject _spawnVFX;

    [Header("Boss Components")]
    [SerializeField] private BossHUD_Manager _bossHUD;
    [SerializeField] private GameObject _endHUD;

    [Header("Boss Cutscene Components")]
    [SerializeField] private float _cameraFocusOnBossTime = 4.0f;
    [SerializeField] private float _cutSceneStartDelay = 0.5f;
    [SerializeField] private float _cybitVideoTime = 2.0f;

    [Header("Spawners")]
    [SerializeField] private SpawnerOfficer _spawnerRef1;
    [SerializeField] private SpawnerOfficer _spawnerRef2;
    [SerializeField] private GameObject _wardenGO;
    [SerializeField] private JackalWarden_AI warden;
    [SerializeField] private GameObject _entryDialogue;
    [SerializeField] private GameObject _playerResponseToBoss;
    [SerializeField] private DynamicAudioManager _dynamicAudioManager;


    private void Update()
    {
        if (!_isTaskComplete && _currentKills >= _killGoal)
        {
            _isTaskComplete = true;
            StartCoroutine(SpawnBoss());

            // Handle destroying enemies from both spawners
            HandleEnemies(_spawnerRef1._spawnedOfficer);
            HandleEnemies(_spawnerRef1._spawnedRanger);
            HandleEnemies(_spawnerRef2._spawnedOfficer);
            HandleEnemies(_spawnerRef2._spawnedRanger);

            // Destroy spawners after clearing enemies
            Destroy(_spawnerRef1);
            Destroy(_spawnerRef2);
            _dynamicAudioManager._backgroundMusicAudioSource.Stop();

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

    private IEnumerator SpawnBoss()
    {
        _playerController = SaveManager.Instance.Player;
        _vCam = _playerController.CurrentVirtualCamera;
        yield return new WaitForSeconds(_cutSceneStartDelay);

        EventManager.InvokeCutscene(true);
       _playerController.Animations.@Animator.Play("ANIM_Jake_Idle");
        _vCam.Follow = _spawnPoint;
        Animator _vcamAnimator = _vCam.GetComponent<Animator>();
        _vcamAnimator.SetBool("Zoom", true);
        _playerController.ShowUIAndCrosshair(false);
        _spawnVFX.SetActive(true);
        EventManager.InvokeCloseObjective(true);
        yield return new WaitForSeconds(_timeToSpawnBoss);
        _wardenGO.SetActive(true);
        _entryDialogue.SetActive(true);
        _dynamicAudioManager.SwitchMusicPhaseInstantly(_dynamicAudioManager._heavyPhase);
        yield return new WaitForSeconds(6);
        _playerResponseToBoss.SetActive(true);
        _vCam.Follow = _playerController.transform;

        warden.EndUI = _endHUD;
        warden.BossHUD = _bossHUD;
        warden.PlayerTarget = _playerController.transform;

        yield return new WaitForSeconds(_timeToDisableVFX);
        _spawnVFX.SetActive(false);

        yield return new WaitForSeconds(_cameraFocusOnBossTime);

        if (_bossHUD != null)
        {
            warden.BossHUD.gameObject.SetActive(true);
        }

        _vcamAnimator.SetBool("Zoom", false);
        EventManager.InvokeCutscene(false);
        _playerController.ShowUIAndCrosshair(true);

        yield return new WaitForSeconds(0.45f);
        warden.IsBossEnabled = true;
        warden.enabled = true;
        NavMeshAgent wardenAgent = warden.GetComponent<NavMeshAgent>();
        wardenAgent.enabled = true;
    }
}

