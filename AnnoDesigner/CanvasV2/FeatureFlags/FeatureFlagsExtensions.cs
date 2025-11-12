namespace AnnoDesigner.CanvasV2.FeatureFlags;

public static class FeatureFlagsExtensions
{
    public static CanvasFeatureFlags Snapshot(this IFeatureFlags flags)
    {
        return CanvasFeatureFlags.From(flags);
    }
}
