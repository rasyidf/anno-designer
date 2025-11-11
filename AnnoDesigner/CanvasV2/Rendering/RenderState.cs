using AnnoDesigner.CanvasV2.FeatureFlags;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AnnoDesigner.CanvasV2.Rendering;

/// <summary>
/// Immutable snapshot of ViewModel state for a single render frame.
/// migrated from AnnoCanvas.xaml.cs — Phase 3: Renderer Extraction
/// </summary>
public readonly record struct RenderState(
    // Viewport and transforms
    Rect Viewport,
    Transform? Translate,
    GuidelineSet? GuidelineSet,

    // Objects to render
    IReadOnlyCollection<LayoutObject> ObjectsToDraw,
    IReadOnlyCollection<LayoutObject> SelectedObjects,
    IReadOnlyCollection<LayoutObject> CurrentObjects,
    IReadOnlyCollection<LayoutObject> AllPlacedObjects,

    // Grid and sizing
    int GridSize,

    // Interaction state
    Rect SelectionRect,
    Input.MouseMode CurrentMode,

    // Rendering toggles (from CanvasFeatureFlags)
    bool RenderGrid,
    bool RenderIcon,
    bool RenderLabel,
    bool RenderInfluences,
    bool RenderTrueInfluenceRange,
    bool RenderHarborBlockedArea,
    bool RenderPanorama,

    // Icons dictionary
    IReadOnlyDictionary<string, IconImage> Icons,

    // Helper services
    ICoordinateHelper CoordinateHelper,

    // Feature flags
    CanvasFeatureFlags FeatureFlags
);
