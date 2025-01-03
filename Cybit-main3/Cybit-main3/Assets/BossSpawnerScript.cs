using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSpawnerScript : MonoBehaviour
{
    [Header("SpawnBank")]
    public GameObject bossGO;

    [Header("Timing")]
    public float timeToSpawnBoss;
    public float timeToDisableVFX;

    [Header("Refrences")]
    public Transform spawnPoint;
    public CutsceneManager CutsceneManager;

    [Header("VFX")]
    public GameObject spawnVFX;
    public GameObject EndHUD;

    [Header("State")]
    public bool shouldSpawnBoss;
    public bool isSpawningBoss;
    public bool bossSpawned;


    // Update is called once per frame
    void Update()
    {
        if (shouldSpawnBoss &!isSpawningBoss)
        {
            StartCoroutine(SpawnBoss());
        }

        if(bossSpawned)
        {
            Destroy(gameObject,4f);
        }
    }

    IEnumerator SpawnBoss()
    {
        spawnVFX.SetActive(true);
        isSpawningBoss = true;
        shouldSpawnBoss = false;
        yield return new WaitForSeconds(timeToSpawnBoss);
        Instantiate(bossGO, spawnPoint.position, spawnPoint.rotation);
        isSpawningBoss = false;
        bossSpawned = true;
        yield return new WaitForSeconds(timeToDisableVFX);
        spawnVFX.SetActive(false);
        yield return new WaitForSeconds(2f);
        EndHUD.SetActive(true);
        yield return new WaitForSeconds(4f);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu_Scene");
    }
}
