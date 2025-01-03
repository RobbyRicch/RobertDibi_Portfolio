using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public int Amount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < PlayerManager.Instance.AllPlayersAlive.Count; i++)
            {
                if (PlayerManager.Instance.AllPlayersAlive[i].gameObject == other.gameObject)
                {
                    if (other.GetComponent<Bank>().BankCurrentAmount < other.GetComponent<Bank>().BankMaxSize)
                    {
                        other.GetComponent<Bank>().UseBank(Amount);
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
