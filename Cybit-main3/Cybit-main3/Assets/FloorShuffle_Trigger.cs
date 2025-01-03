using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorShuffle_Trigger : MonoBehaviour
{

    [SerializeField] private ElevatorFloors_Manager _floorManagerRef;
    [SerializeField] private ElevatorDoor_Trigger _currentDoorTrigger1;
    [SerializeField] private ElevatorDoor_Trigger _currentDoorTrigger2;

    [SerializeField] private ElevatorDoor_Trigger _nextDoorTrigger1;
    [SerializeField] private ElevatorDoor_Trigger _nextDoorTrigger2;

    private void Start()
    {
        _currentDoorTrigger1 = _floorManagerRef._doorTrigger1;
        _currentDoorTrigger2 = _floorManagerRef._doorTrigger2;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _floorManagerRef.ShuffleRooms();
            SwitchDoorTriggers();
        }
    }

    private void SwitchDoorTriggers()
    {
        _floorManagerRef._doorTrigger1 = _nextDoorTrigger1;
        _floorManagerRef._doorTrigger2 = _nextDoorTrigger2;
    }
}
