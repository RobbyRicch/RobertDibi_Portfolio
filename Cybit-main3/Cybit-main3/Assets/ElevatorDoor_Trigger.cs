using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoor_Trigger : MonoBehaviour
{
    [SerializeField] ElevatorFloors_Manager _floorsManagerRef;
    [SerializeField] public Elevator_Room _correspondingRoom;
    [SerializeField] private Player_Controller _playerRef;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            _playerRef.transform.position = _correspondingRoom._entranceTransform.gameObject.transform.position;
        }
    }


}



