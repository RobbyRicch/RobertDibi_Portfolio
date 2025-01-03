
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetAllBindings : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset inputAcitons;

    public void ResetBindings()
    {
        foreach(InputActionMap map in inputAcitons.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
    }
}
