using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AssetHolder : MonoBehaviour
{
    private static AssetHolder _instance;
    public static AssetHolder Instance => _instance;

    [SerializeField] private InputActionAsset _playerActionAsset;
    public InputActionAsset PlayerActionAsset => _playerActionAsset;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
}
