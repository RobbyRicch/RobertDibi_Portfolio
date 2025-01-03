using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfiguration
{
    private PlayerInput _Input;
    public PlayerInput Input { get => _Input; set => _Input = value; }

    private Color _mainColor, _secondaryColor;
    public Color MainColor { get => _mainColor; set => _mainColor = value; }
    public Color SecondaryColor { get => _secondaryColor; set => _secondaryColor = value; }

    [ColorUsage(true, true)] private Color _mainEmmisionColor, _secondaryEmmisionColor;
    public Color MainEmmisionColor { get => _mainEmmisionColor; set => _mainEmmisionColor = value; }
    public Color SecondaryEmmisionColor { get => _secondaryEmmisionColor; set => _secondaryEmmisionColor = value; }

    private int _id;
    public int ID { get => _id; set => _id = value; }

    private int _modelNum = 0;
    public int ModelNum { get => _modelNum; set => _modelNum = value; }

    private bool _isCharacterSetupDone;
    public bool IsCharacterSetupDone { get => _isCharacterSetupDone; set => _isCharacterSetupDone = value; }

    public PlayerConfiguration(PlayerInput playerInput)
    {
        _Input = playerInput;
        _id = playerInput.playerIndex;
        Debug.Log(_Input.actionEvents[0]);
    }
}
