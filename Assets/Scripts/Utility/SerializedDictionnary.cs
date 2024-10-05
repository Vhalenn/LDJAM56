using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializedDictionnary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<TKey> keys = new();
    [SerializeField] private List<TValue> values = new();

    public void OnAfterDeserialize()
    {
        Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Something went wrong on dictionnary");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this[keys[i]] = values[i];
        }
    }

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (var pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }
}
