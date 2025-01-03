using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> _keys = new();
    [SerializeField] private List<TValue> _values = new();

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (KeyValuePair<TKey, TValue> kvp in this)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }
    public void OnAfterDeserialize()
    {
        Clear();

        if (_keys.Count != _values.Count)
        {
            Debug.LogError("Error: keys not equals to amount of values after Deserialization");
        }

        for (int i = 0; i < _keys.Count; i++)
        {
            Add(_keys[i], _values[i]);
        }
    }
}
