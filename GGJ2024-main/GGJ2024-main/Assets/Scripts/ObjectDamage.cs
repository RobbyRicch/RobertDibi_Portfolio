using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDamage : MonoBehaviour
{
    public enum TrapState { Spikes, Lights }


    [SerializeField] TrapState trapState;
    [SerializeField] float _stunDamage;
    PlayerMovement _playerMovement;

    private void Start()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerMovement = other.GetComponent<PlayerMovement>();
            if (_playerMovement != null)
            {
                CheckTrap();

            }

        }
        
    }

    void CheckTrap()
    {
        switch (trapState)
        {
            case TrapState.Lights:
                StartCoroutine(LightTrapCoroutine());
                break;
            default:
                StartCoroutine(ApplyStun());
                break;
        }
    }

    IEnumerator LightTrapCoroutine()
    {

        _playerMovement.canMove = false;
        GameManager.Instance.OnLoseHeart();
        _playerMovement.canMove = true;
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
       

    }
    IEnumerator ApplyStun()
    {

        _playerMovement.canMove = false;
        GameManager.Instance.OnLoseHeart();
        yield return new WaitForSeconds(1);
        _playerMovement.canMove = true;
        

    }


}
