using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AnnoDesigner.CanvasV2.FeatureFlags;

// Basic in-memory implementation; to be replaced by settings-backed or persisted flags later.
public sealed class SimpleFeatureFlags : IFeatureFlags
{
    private readonly ConcurrentDictionary<string, object?> _values = new(StringComparer.OrdinalIgnoreCase);

    public event Action<string, object?>? FeatureChanged;

    public bool IsEnabled(string name)
    {
        return _values.TryGetValue(name, out object value) && value switch
        {
            bool b => b,
            int i => i != 0,
            string s when bool.TryParse(s, out bool parsed) => parsed,
            _ => false
        };
    }

    public bool TryGet<T>(string name, out T value)
    {
        if (_values.TryGetValue(name, out object raw) && raw is T casted)
        {
            value = casted;
            return true;
        }
        value = default!;
        return false;
    }

    public void Set(string name, object? value)
    {
        _values[name] = value;
        FeatureChanged?.Invoke(name, value);
    }

    // Convenience bulk loader
    public void Load(IDictionary<string, object?> initial)
    {
        foreach (KeyValuePair<string, object> kvp in initial)
        {
            _values[kvp.Key] = kvp.Value;
        }
    }
}
