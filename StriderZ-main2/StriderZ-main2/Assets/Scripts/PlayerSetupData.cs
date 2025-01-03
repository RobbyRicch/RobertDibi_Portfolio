using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSetupData
{
    private PlayerInput _Input;
    public PlayerInput Input { get => _Input; set => _Input = value; }

    private int _id;
    public int ID => _id;

    private string _nickname;
    public string Nickname => _nickname;

    private ModelType _chosenModelType;
    public ModelType ChosenModelType { get => _chosenModelType; set => _chosenModelType = value; }

    private Sprite _userPic, _helmetTex;
    public Sprite UserPic { get => _userPic; set => _userPic = value; }
    public Sprite HelmetSprite { get => _helmetTex; set => _helmetTex = value; }

    private ColorData _colorData;
    public ColorData ColorData { get => _colorData; set => _colorData = value; }

    private bool _isSetupDone = false;
    public bool IsSetupDone { get => _isSetupDone; set => _isSetupDone = value; }

    private bool _isColored = false;
    public bool IsColored { get => _isColored; set => _isColored = value; }

    public PlayerSetupData(PlayerInput playerInput)
    {
        _Input = playerInput;
        _id = playerInput.playerIndex;
        _nickname = "P 0" + _id + 1;
        _chosenModelType = (ModelType)_id;
        Debug.Log(_Input.actionEvents[0]);
    }
}
