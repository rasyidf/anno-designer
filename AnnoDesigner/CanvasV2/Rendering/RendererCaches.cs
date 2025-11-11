using System.Windows.Media;

namespace AnnoDesigner.CanvasV2.Rendering;

/// <summary>
/// Manages cached brushes, pens, and DrawingGroups for the renderer.
/// migrated from AnnoCanvas.xaml.cs — Phase 3: cache management
/// </summary>
public sealed class RendererCaches
{
    // Version token for cache invalidation

    // Grid and selection
    private Pen? _gridPen;
    private Pen? _selectionPen;
    private Brush? _selectionFill;
    private Pen? _highlightPen;

    // Influence rendering
    private Brush? _lightBrush;
    private Pen? _radiusPen;
    private Brush? _influencedBrush;
    private Pen? _influencedPen;

    // DrawingGroup caches
    private DrawingGroup? _drawingGroupObjectSelection;
    private int _lastObjectSelectionVersion = -1;

    public RendererCaches()
    {
        CacheVersion = 0;
        InitializePensAndBrushes();
    }

    /// <summary>
    /// Invalidates all caches. Call when GridSize or visual settings change.
    /// </summary>
    public void Invalidate()
    {
        CacheVersion++;
        _lastObjectSelectionVersion = -1;
    }

    public int CacheVersion { get; private set; }

    private void InitializePensAndBrushes()
    {
        _gridPen = new Pen(new SolidColorBrush(Color.FromRgb(200, 200, 200)), 1);
        _gridPen.Freeze();

        _selectionPen = new Pen(Brushes.Yellow, 2);
        _selectionPen.Freeze();

        _selectionFill = new SolidColorBrush(Color.FromArgb(32, 255, 255, 0));
        _selectionFill.Freeze();

        _highlightPen = new Pen(Brushes.Yellow, 2);
        _highlightPen.Freeze();

        _lightBrush = new SolidColorBrush(Color.FromArgb(32, 255, 255, 255));
        _lightBrush.Freeze();

        _radiusPen = new Pen(Brushes.White, 1);
        _radiusPen.Freeze();

        _influencedBrush = new SolidColorBrush(Color.FromArgb(32, 0, 255, 0));
        _influencedBrush.Freeze();

        _influencedPen = new Pen(Brushes.LightGreen, 1);
        _influencedPen.Freeze();
    }

    public Pen GridPen => _gridPen!;
    public Pen SelectionPen => _selectionPen!;
    public Brush SelectionFill => _selectionFill!;
    public Pen HighlightPen => _highlightPen!;
    public Brush LightBrush => _lightBrush!;
    public Pen RadiusPen => _radiusPen!;
    public Brush InfluencedBrush => _influencedBrush!;
    public Pen InfluencedPen => _influencedPen!;

    public DrawingGroup GetOrCreateObjectSelectionDrawingGroup()
    {
        if (_drawingGroupObjectSelection is null || _drawingGroupObjectSelection.IsFrozen)
        {
            _drawingGroupObjectSelection = new DrawingGroup();
        }
        return _drawingGroupObjectSelection;
    }

    public void MarkObjectSelectionCacheUsed(int currentVersion)
    {
        _lastObjectSelectionVersion = currentVersion;
    }

    public bool IsObjectSelectionCacheValid(int currentVersion)
    {
        return _lastObjectSelectionVersion == currentVersion;
    }
}
