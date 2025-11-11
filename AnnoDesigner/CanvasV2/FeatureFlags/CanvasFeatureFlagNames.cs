namespace AnnoDesigner.CanvasV2.FeatureFlags;

public static class CanvasFeatureFlagNames
{
    public const string UseCanvasV2 = "CanvasV2:Enabled";

    // Rendering behaviour
    public const string RenderGrid = "Canvas:RenderGrid";
    public const string RenderInfluences = "Canvas:RenderInfluences";
    public const string RenderIcons = "Canvas:RenderIcons";
    public const string RenderLabels = "Canvas:RenderLabels";
    public const string RenderTrueInfluenceRange = "Canvas:RenderTrueInfluenceRange";
    public const string RenderHarborBlockedArea = "Canvas:RenderHarborBlockedArea";
    public const string RenderPanorama = "Canvas:RenderPanorama";

    // Debug/diagnostics
    public const string DebugMode = "Canvas:DebugMode";
}
