using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollect : MonoBehaviour
{
    [SerializeField] private Bank BankRef;
    private void Start()
    {
        BankRef = GetComponentInParent<Bank>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Bank otherBank = other.GetComponent<Bank>();
            BankRef.UseBank(otherBank.BankCurrentAmount);
            otherBank.UseBank(-otherBank.BankCurrentAmount);
        }
    }
}
