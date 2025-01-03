using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFloors_Manager : MonoBehaviour
{
    [Header("Room GameObjects")]
    [SerializeField] private List<GameObject> _rooms;
    [SerializeField] private int _roomsSelected;
    [SerializeField] GameObject _SelectedRoom1, _SelectedRoom2;

    [Header("Door Triggers")]
    [SerializeField] public ElevatorDoor_Trigger _doorTrigger1;
    [SerializeField] public ElevatorDoor_Trigger _doorTrigger2;
    private void Start()
    {
        ShuffleRooms();
    }
    public void ShuffleRooms()
    {
        if (_roomsSelected == 0 && _rooms.Count >= 2)
        {
            // Select the first random room
            int randomRoom1 = Random.Range(0, _rooms.Count);
            _SelectedRoom1 = _rooms[randomRoom1].gameObject;
            _SelectedRoom1.SetActive(true);
            _doorTrigger1._correspondingRoom = _SelectedRoom1.GetComponent<Elevator_Room>();
            // Select the second random room, ensuring it’s different from the first
            int randomRoom2;
            do
            {
                randomRoom2 = Random.Range(0, _rooms.Count);
            } while (randomRoom2 == randomRoom1);

            _SelectedRoom2 = _rooms[randomRoom2].gameObject;
            _SelectedRoom2.SetActive(true);
            _doorTrigger2._correspondingRoom = _SelectedRoom2.GetComponent<Elevator_Room>();

            // Set rooms as selected
            _roomsSelected = 2;
        }
    }
}
