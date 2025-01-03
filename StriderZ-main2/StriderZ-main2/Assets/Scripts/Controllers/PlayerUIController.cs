using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerInputHandler _inputHandler;
    public PlayerInputHandler InputHandler => _inputHandler;

    private const string _keyboardName = "Keyboard:/Keyboard";
    private string _playerControllerName;

    private bool _isCyclingNextColor = false, _isCyclingPreviousColor = false;
    public bool IsCyclingNextColor { get => _isCyclingNextColor; set => _isCyclingNextColor = value; }
    public bool IsCyclingPreviousColor { get => _isCyclingPreviousColor; set => _isCyclingPreviousColor = value; }

    [SerializeField] private bool _isDebugMessagesOn;

    private void Start()
    {
        _playerControllerName = _inputHandler.SetupData.Input.devices[0].ToString();
    }
    private void Update()
    {
        if (_playerControllerName == _keyboardName)
        {
            //_inputHandler.Config.Input.defaultControlScheme = "Keyboard";
        }
        else
        {

        }
    }

    #region Player Interactions
    public void OnLeftTrigger(InputAction.CallbackContext context)
    {
        bool isPressed = context.ReadValue<float>() == 0 ? false : true;

        if (!isPressed && context.ReadValue<float>() == 0)
        {
            Debug.Log("Grapple left canceled.");
            _inputHandler.Attractor.CancelAttractorLeft(true);
        }
        else if (isPressed)
        {
            Debug.Log("Grapple left started.");
            _inputHandler.Attractor.StartAttractorLeft(true);
        }

        // grapple controller
        /*if (context.canceled)
        {
            Debug.Log("Left trigger canceled.");
        }
        else if (context.performed)
        {
            Debug.Log("Left trigger performed");
        }
        else if (context.started)
        {
            Debug.Log("Left trigger started");
        }
        else Debug.LogError("Left trigger failed: No input started, preformed or canceled.");*/
    }
    public void OnRightTrigger(InputAction.CallbackContext context)
    {
        
    }
    public void OnLeftShoulderBtn(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Skill A activated.");
        }
        else Debug.LogError("Left shoulder button action failed: No logic.");
    }
    public void OnRightShoulderBtn(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Cycling Next Color.");
            _isCyclingNextColor = true;
        }
        else if (context.performed)
        {
            Debug.Log("Skill B Started.");
            _isCyclingNextColor = false;
        }
        else if (context.canceled)
        {
            Debug.Log("Skill B Started.");
            _isCyclingNextColor = false;
        }
        else Debug.LogError("Right shoulder button action failed: No logic.");
    }
    public void OnSouthBtn(InputAction.CallbackContext context)
    {
        
    }
    public void OnWestBtn(InputAction.CallbackContext context)
    {

    }
    public void OnNorthBtn(InputAction.CallbackContext context)
    {

    }
    public void OnEastBtn(InputAction.CallbackContext context)
    {

    }
    public void OnStartBtn(InputAction.CallbackContext context)
    {

    }
    #endregion
}
