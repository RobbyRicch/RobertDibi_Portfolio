using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TooltipType
{
    Health, 
    MeleeDamage
}

public class ToolltipManager : MonoBehaviour
{
    private static ToolltipManager _instance;
    public static ToolltipManager Instance => _instance;

    [SerializeField] private Tooltip[] _skillTooltipPrefabsOrderedByType;
    private Tooltip _tempTooltip = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
    }

    public Tooltip InstantiateToolTip(int i, Vector3 pos)
    {
        if (_skillTooltipPrefabsOrderedByType[i] != null)
        {
            _tempTooltip = Instantiate(_skillTooltipPrefabsOrderedByType[i], transform);
            _tempTooltip.transform.position = pos;
            _tempTooltip.transform.rotation = Quaternion.identity;
            return _tempTooltip;
        }

        _tempTooltip = null;
        Debug.LogError("No tooltip of type '" + (TooltipType)i + "' in _skillTnooltipPrefabsOrderedByType");
        return null;
    }
    public Tooltip InstantiateToolTip(TooltipType type, Vector3 pos)
    {
        if (_skillTooltipPrefabsOrderedByType[(int)type] != null)
        {
            _tempTooltip = Instantiate(_skillTooltipPrefabsOrderedByType[(int)type], transform);
            _tempTooltip.transform.position = pos;
            _tempTooltip.transform.rotation = Quaternion.identity;
            return _tempTooltip;
        }

        _tempTooltip = null;
        Debug.LogError("No tooltip of type '" + type + "' in _skillTnooltipPrefabsOrderedByType");
        return null;
    }
    public void DestroyTooltip()
    {
        if (!_tempTooltip) return;
        Destroy(_tempTooltip.gameObject);
        _tempTooltip = null;
    }
}
