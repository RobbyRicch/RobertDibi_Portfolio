using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTriggerWithTag : MonoBehaviour
{
    public int TriggeredPlayerIndex;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < PlayerManager.Instance.AllPlayersAlive.Count; i++)
            {
                if (PlayerManager.Instance.AllPlayersAlive[i].gameObject == other.gameObject)
                {
                    TriggeredPlayerIndex = i;
                }
            }
            
        }
    }
}
