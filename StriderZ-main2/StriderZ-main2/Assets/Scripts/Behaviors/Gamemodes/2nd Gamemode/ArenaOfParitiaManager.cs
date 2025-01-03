using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArenaOfParitiaManager : MonoBehaviour
{
    private void Start()
    {
        Invoke("AddPlayerLivesToPlayers", 1);
        Physics.IgnoreLayerCollision(6, 13, true);
    }
    private void Update()
    {
        if (PlayerManager.Instance.IsOnePlayerLeft() != null)
        {
            //StartCoroutine(WaitAfterDeath());
        }
    }
    private void AddPlayerLivesToPlayers()
    {
        foreach (var item in PlayerManager.Instance.AllPlayersAlive)
        {
            item.AddComponent<PlayerLives>();
        }
    }
    IEnumerator WaitAfterDeath()
    {
        yield return new WaitForSeconds(5);
        CustomSceneManager.ChangeScene(1);
    }
}
