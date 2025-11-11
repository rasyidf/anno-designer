namespace AnnoDesigner.CanvasV2.FeatureFlags;

public static class FeatureFlagsExtensions
{
    public static CanvasFeatureFlags Snapshot(this IFeatureFlags flags)
        => CanvasFeatureFlags.From(flags);
}
