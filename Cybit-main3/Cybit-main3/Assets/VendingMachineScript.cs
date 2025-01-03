using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachineScript : MonoBehaviour
{
    [SerializeField] GameObject secretMsg;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            secretMsg.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            secretMsg.SetActive(false);
        }
    }
}
