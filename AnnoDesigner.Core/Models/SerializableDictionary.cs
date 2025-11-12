using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AnnoDesigner.Core.Models;

[Serializable]
public class SerializableDictionary<T>
    : ISerializable
{
    public readonly Dictionary<string, T> Dict;

    public SerializableDictionary()
    {
        Dict = [];
    }

    protected SerializableDictionary(SerializationInfo info, StreamingContext context)
    {
        Dict = [];
        foreach (SerializationEntry entry in info)
        {
            Dict.Add(entry.Name, (T)Convert.ChangeType(entry.Value, typeof(T)));
        }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        foreach (string key in Dict.Keys)
        {
            info.AddValue(key, Dict[key]);
        }
    }

    public T this[string key]
    {
        get => Dict.ContainsKey(key) ? Dict[key] : default; set => Dict[key] = value;
    }
}