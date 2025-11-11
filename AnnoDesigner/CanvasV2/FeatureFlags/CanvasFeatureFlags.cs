using System.Collections.Generic;
using System.Collections.Immutable;

namespace AnnoDesigner.CanvasV2.FeatureFlags;

// Strongly typed snapshot of canvas-related flags; immutable record for easy passing to renderer.
public sealed record CanvasFeatureFlags(
    bool UseCanvasV2,
    bool RenderGrid,
    bool RenderInfluences,
    bool RenderIcons,
    bool RenderLabels,
    bool RenderTrueInfluenceRange,
    bool RenderHarborBlockedArea,
    bool RenderPanorama,
    bool DebugMode)
{
    public static CanvasFeatureFlags From(IFeatureFlags source) => new(
        UseCanvasV2: source.IsEnabled(CanvasFeatureFlagNames.UseCanvasV2),
        RenderGrid: source.IsEnabled(CanvasFeatureFlagNames.RenderGrid),
        RenderInfluences: source.IsEnabled(CanvasFeatureFlagNames.RenderInfluences),
        RenderIcons: source.IsEnabled(CanvasFeatureFlagNames.RenderIcons),
        RenderLabels: source.IsEnabled(CanvasFeatureFlagNames.RenderLabels),
        RenderTrueInfluenceRange: source.IsEnabled(CanvasFeatureFlagNames.RenderTrueInfluenceRange),
        RenderHarborBlockedArea: source.IsEnabled(CanvasFeatureFlagNames.RenderHarborBlockedArea),
        RenderPanorama: source.IsEnabled(CanvasFeatureFlagNames.RenderPanorama),
        DebugMode: source.IsEnabled(CanvasFeatureFlagNames.DebugMode)
    );

    public ImmutableDictionary<string, object?> ToDictionary() => new Dictionary<string, object?>
    {
        [CanvasFeatureFlagNames.UseCanvasV2] = UseCanvasV2,
        [CanvasFeatureFlagNames.RenderGrid] = RenderGrid,
        [CanvasFeatureFlagNames.RenderInfluences] = RenderInfluences,
        [CanvasFeatureFlagNames.RenderIcons] = RenderIcons,
        [CanvasFeatureFlagNames.RenderLabels] = RenderLabels,
        [CanvasFeatureFlagNames.RenderTrueInfluenceRange] = RenderTrueInfluenceRange,
        [CanvasFeatureFlagNames.RenderHarborBlockedArea] = RenderHarborBlockedArea,
        [CanvasFeatureFlagNames.RenderPanorama] = RenderPanorama,
        [CanvasFeatureFlagNames.DebugMode] = DebugMode,
    }.ToImmutableDictionary();
}
