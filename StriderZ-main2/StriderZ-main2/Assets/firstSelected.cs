using UnityEngine;
using UnityEngine.EventSystems;

public class firstSelected : MonoBehaviour
{
    public GameObject defaultButton; // Reference to the button you want to auto-select

    void Start()
    {
        // Check if the default button is assigned
        if (defaultButton != null)
        {
            // Delay the button selection by one frame to allow the UI system to initialize
            Invoke("SelectDefaultButton", 0.01f);
        }
    }

    void SelectDefaultButton()
    {
        // Set the default button as the currently selected game object
        EventSystem.current.SetSelectedGameObject(defaultButton);
    }
}
