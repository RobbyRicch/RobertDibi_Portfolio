using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_handler : MonoBehaviour
{
    [SerializeField] private Player_Controller _controller;
    public Player_Controller Controller => _controller;

    [SerializeField] private Player_Animations _animations;
    public Player_Animations Animation => _animations;
}
