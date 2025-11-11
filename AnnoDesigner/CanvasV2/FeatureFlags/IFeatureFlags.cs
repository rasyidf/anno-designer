using System;

// File-scoped namespace, C# 10+
namespace AnnoDesigner.CanvasV2.FeatureFlags;

public interface IFeatureFlags
{
    bool IsEnabled(string name);
    bool TryGet<T>(string name, out T value);

    // Raised when a flag value changes at runtime (optional for implementations)
    event Action<string, object?>? FeatureChanged;
}
