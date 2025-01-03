using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLives : MonoBehaviour
{
    public int Lives = 3;
    public bool IsAlive = true;
    private Transform _deathSpawn;
    private void Start()
    {
        _deathSpawn = GameObject.FindGameObjectWithTag("DeathSpawn").transform;
    }

    private void Update()
    {
        if (Lives <= 0)
        {
            IsAlive = false;
            GetComponent<PlayerController>().IsAlive = false;
            gameObject.SetActive(false);
        }
    }

    public void Respawn()
    {
        if (IsAlive)
        {
            int playerIndex;
            List<PlayerInputHandler> allPlayers = PlayerManager.Instance.AllPlayers;
            for (int i = 0; i < allPlayers.Count; i++)
            {
                if (allPlayers[i].gameObject == this.gameObject)
                {
                    playerIndex = i;
                    allPlayers[i].Controller.Rb.velocity = Vector3.zero;
                    allPlayers[i].transform.position = _deathSpawn.position;
                    StartCoroutine(DelaySpawn(playerIndex));
                }
            }
        }
    }
    public void ReduceLive()
    {
        if (IsAlive)
        {
            Lives--;
        }
    }

    IEnumerator DelaySpawn(int playerIndex)
    {
        yield return new WaitForSeconds(2f);
        transform.position = LaserRushGameMode.Instance.PlayerSpawns[playerIndex].transform.position;
        PlayerManager.Instance.AllPlayers[playerIndex].Controller.InputHandler.Coliders[0].enabled = false;
        StartCoroutine(Invulnerable(playerIndex));
    }

    IEnumerator Invulnerable(int playerIndex)
    {
        yield return new WaitForSeconds(2f);
        PlayerManager.Instance.AllPlayers[playerIndex].Controller.InputHandler.Coliders[0].enabled = true;
    }
}
