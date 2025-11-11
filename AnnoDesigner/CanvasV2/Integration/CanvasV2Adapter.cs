using AnnoDesigner.CanvasV2.FeatureFlags;
using AnnoDesigner.CanvasV2.Rendering;
using AnnoDesigner.Core.DataStructures;
using AnnoDesigner.Core.Layout.Helper;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Models;
using AnnoDesigner.Core.Services;
using AnnoDesigner.CustomEventArgs;
using AnnoDesigner.Helper;
using AnnoDesigner.Models;
using AnnoDesigner.Undo;
using AnnoDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AnnoDesigner.CanvasV2.Integration;

/// <summary>
/// Adapter that exposes an AnnoCanvasViewModel / AnnoCanvasV2 as an IAnnoCanvas so the
/// rest of the application (MainViewModel) can interact with V2 via the existing interface.
/// This is intentionally an integration shim to start the MainWindow wiring; it can be
/// expanded later to fully match v1 behaviour.
/// </summary>
public class CanvasV2Adapter : IAnnoCanvas
{
    private readonly AnnoCanvasViewModel _vm;
    private readonly IFileSystem _fileSystem;

    public CanvasV2Adapter(AnnoCanvasViewModel vm, AnnoCanvasV2 view, IFileSystem fileSystem = null)
    {
        _vm = vm ?? throw new ArgumentNullException(nameof(vm));
        Control = view ?? throw new ArgumentNullException(nameof(view));
        _fileSystem = fileSystem ?? new FileSystem();

        // Forward VM events to adapter events
        _vm.StatisticsUpdated += (s, e) => StatisticsUpdated?.Invoke(s, e);
        _vm.ColorsInLayoutUpdated += (s, e) => ColorsInLayoutUpdated?.Invoke(s, e);
        _vm.OnLoadedFileChanged += (s, e) => OnLoadedFileChanged?.Invoke(s, e);
        _vm.OpenFileRequested += (s, e) => OpenFileRequested?.Invoke(s, e);
        _vm.SaveFileRequested += (s, e) => SaveFileRequested?.Invoke(s, e);
        // Forward VM requests that require UI interaction (dialogs) so the host can show dialogs
        _vm.RequestOpenFile += (s, e) => RequestOpenFile?.Invoke(s, e);
        _vm.RequestSaveAs += (s, e) => RequestSaveAs?.Invoke(s, e);
        _vm.RequestNewFile += (s, e) => RequestNewFile?.Invoke(s, e);
        _vm.OnCurrentObjectChanged += obj => OnCurrentObjectChanged?.Invoke(obj);
        _vm.OnStatusMessageChanged += msg => OnStatusMessageChanged?.Invoke(msg);
    }

    /// <summary>
    /// Self-contained constructor: creates the V2 ViewModel and View for you and wires them up.
    /// Pass optional building presets and icons to initialize the VM the same way MainWindow used to.
    /// </summary>
    public CanvasV2Adapter(IFeatureFlags featureFlags,
                           IUndoManager undoManager,
                           IClipboardService clipboardService,
                           IAppSettings appSettings,
                           ICoordinateHelper coordinateHelper,
                           IBrushCache brushCache,
                           IPenCache penCache,
                           AnnoDesigner.Core.Layout.Models.ILayoutLoader layoutLoader,
                           ILocalizationHelper localizationHelper,
                           IMessageBoxService messageBoxService,
                           HotkeyCommandManager hotkeyManager,
                           BuildingPresets buildingPresets = null,
                           Dictionary<string, IconImage> icons = null,
                           IFileSystem fileSystem = null)
        : this(new AnnoCanvasViewModel(featureFlags,
                                       undoManager,
                                       clipboardService,
                                       appSettings,
                                       coordinateHelper,
                                       brushCache,
                                       penCache,
                                       layoutLoader,
                                       localizationHelper,
                                       messageBoxService,
                                       hotkeyManager)
        {
            BuildingPresets = buildingPresets,
            Icons = icons ?? new Dictionary<string, IconImage>(StringComparer.OrdinalIgnoreCase)
        },
               new AnnoCanvasV2(),
               fileSystem)
    {
        // Ensure the view's DataContext is set to the VM created above
        Control.DataContext = _vm;
    }

    /// <summary>
    /// The actual WPF control instance for CanvasV2. Host can add this to the visual tree.
    /// </summary>
    public AnnoCanvasV2 Control { get; }

    // Events
    public event EventHandler<EventArgs> ColorsInLayoutUpdated;
    public event EventHandler<UpdateStatisticsEventArgs> StatisticsUpdated;
    public event EventHandler<FileLoadedEventArgs> OnLoadedFileChanged;
    public event Action<string> OnStatusMessageChanged;
    public event Action<LayoutObject> OnCurrentObjectChanged;
    public event EventHandler<OpenFileEventArgs> OpenFileRequested;
    public event EventHandler<SaveFileEventArgs> SaveFileRequested;
    // Events the host can react to in order to show dialogs / create a new layout
    public event EventHandler RequestOpenFile;
    public event EventHandler RequestSaveAs;
    public event EventHandler RequestNewFile;

    // Collections and properties
    public QuadTree<LayoutObject> PlacedObjects { get => _vm.PlacedObjects; set => _vm.PlacedObjects = value; }
    public HashSet<LayoutObject> SelectedObjects { get => _vm.SelectedObjects; set => _vm.SelectedObjects = value; }
    public List<LayoutObject> CurrentObjects => _vm.CurrentObjects;
    public BuildingPresets BuildingPresets => _vm.BuildingPresets!;
    public Dictionary<string, IconImage> Icons => _vm.Icons;
    public IUndoManager UndoManager => _vm.UndoManager;
    public bool RenderGrid { get => _vm.RenderGrid; set => _ = _vm.TrySetFeatureFlag(CanvasFeatureFlagNames.RenderGrid, value); }
    public bool RenderInfluences { get => _vm.RenderInfluences; set => _ = _vm.TrySetFeatureFlag(CanvasFeatureFlagNames.RenderInfluences, value); }
    public bool RenderTrueInfluenceRange { get => _vm.RenderTrueInfluenceRange; set => _ = _vm.TrySetFeatureFlag(CanvasFeatureFlagNames.RenderTrueInfluenceRange, value); }
    public bool RenderHarborBlockedArea { get => _vm.RenderHarborBlockedArea; set => _ = _vm.TrySetFeatureFlag(CanvasFeatureFlagNames.RenderHarborBlockedArea, value); }
    public bool RenderPanorama { get => _vm.RenderPanorama; set => _ = _vm.TrySetFeatureFlag(CanvasFeatureFlagNames.RenderPanorama, value); }
    public bool RenderLabel { get => _vm.RenderLabel; set => _ = _vm.TrySetFeatureFlag(CanvasFeatureFlagNames.RenderLabels, value); }
    public bool RenderIcon { get => _vm.RenderIcon; set => _ = _vm.TrySetFeatureFlag(CanvasFeatureFlagNames.RenderIcons, value); }
    public string LoadedFile { get => _vm.LoadedFile ?? string.Empty; set => _vm.LoadedFile = value; }
    public int GridSize { get => _vm.GridSize; set => _vm.GridSize = value; }

    public ICommand RotateCommand => _vm.RotateCommand;

    public void ForceRendering()
    {
        // Ask VM to invalidate render; view will pick up via RenderInvalidated subscription.
        _ = _vm.CreateRenderState();
        _vm.InvalidateRender();
    }

    public void SetCurrentObject(LayoutObject obj)
    {
        _vm.SetCurrentObject(obj);
    }

    public void ResetZoom()
    {
        _vm.ResetZoom();
    }

    public void Normalize()
    {
        _vm.Normalize(1);
    }

    public void Normalize(int border)
    {
        _vm.Normalize(border);
    }

    public void ResetViewport()
    {
        _vm.ResetViewport();
    }

    public void RaiseStatisticsUpdated(UpdateStatisticsEventArgs args)
    {
        _vm.RaiseStatisticsUpdated(args);
    }

    public void RaiseColorsInLayoutUpdated()
    {
        _vm.RaiseColorsInLayoutUpdated();
    }

    public Rect ComputeBoundingRect(IEnumerable<LayoutObject> objects)
    {
        return _vm.ComputeBoundingRect(objects);
    }

    public Task<bool> CheckUnsavedChanges()
    {
        return _vm.CheckUnsavedChanges();
    }

    public void CheckUnsavedChangesBeforeCrash()
    {
        _vm.CheckUnsavedChangesBeforeCrash();
    }

    /// <summary>
    /// Render the provided layout to a file using the v2 renderer.
    /// This aims to provide parity with the v1 export pipeline used by MainViewModel.
    /// </summary>
    public void RenderToFile(string filename, IEnumerable<AnnoDesigner.Core.Models.AnnoObject> placedObjects, IEnumerable<AnnoDesigner.Core.Models.AnnoObject> selectedObjects, int border, AnnoDesigner.Models.CanvasRenderSetting renderSettings = null)
    {
        renderSettings ??= new AnnoDesigner.Models.CanvasRenderSetting() { RenderGrid = true, RenderIcon = true };

        // Calculate bounding rect and statistics
        StatisticsCalculationHelper statsHelper = new();
        StatisticsCalculationResult statistics = statsHelper.CalculateStatistics(placedObjects, true, true);
        Rect statsRect = (Rect)statistics;

        // Build layout objects and quad tree (use fresh caches for export)
        QuadTree<LayoutObject> quadTree = new((Rect)statistics);
        List<LayoutObject> layoutObjects = [];
        BrushCache exportBrushCache = new();
        PenCache exportPenCache = new();
        foreach (AnnoObject ao in placedObjects)
        {
            LayoutObject lo = new(ao, _vm.CoordinateHelper, exportBrushCache, exportPenCache);
            layoutObjects.Add(lo);
            quadTree.Add(lo);
        }

        // Normalize (move objects so min is at border)
        double minX = layoutObjects.Min(o => o.Position.X);
        double minY = layoutObjects.Min(o => o.Position.Y);
        double dx = minX - border;
        double dy = minY - border;
        if (dx != 0 || dy != 0)
        {
            foreach (LayoutObject lo in layoutObjects)
            {
                lo.Position = new Point(lo.Position.X - dx, lo.Position.Y - dy);
            }
        }

        int grid = renderSettings.GridSize ?? _vm.GridSize;

        // Compute pixel dimensions
        double maxX = layoutObjects.Max(o => o.Position.X + o.Size.Width);
        double maxY = layoutObjects.Max(o => o.Position.Y + o.Size.Height);
        double widthPx = _vm.CoordinateHelper.GridToScreen(maxX + border, grid);
        double heightPx = _vm.CoordinateHelper.GridToScreen(maxY + border, grid) + 1;

        // Prepare render state
        LayoutRenderer renderer = new();
        List<LayoutObject> selectedLayoutObjects = [];
        if (selectedObjects != null)
        {
            foreach (AnnoObject so in selectedObjects)
            {
                selectedLayoutObjects.Add(new LayoutObject(so, _vm.CoordinateHelper, exportBrushCache, exportPenCache));
            }
        }

        RenderState state = new(
            Viewport: new Rect(minX - border, minY - border, maxX - minX + (border * 2), maxY - minY + (border * 2)),
            Translate: new TranslateTransform(-(minX - border) * grid, -(minY - border) * grid),
            GuidelineSet: null,
            ObjectsToDraw: layoutObjects,
            SelectedObjects: selectedLayoutObjects,
            CurrentObjects: [],
            AllPlacedObjects: layoutObjects,
            GridSize: grid,
            SelectionRect: Rect.Empty,
            CurrentMode: AnnoDesigner.CanvasV2.Input.MouseMode.Standard,
            RenderGrid: renderSettings.RenderGrid,
            RenderIcon: renderSettings.RenderIcon,
            RenderLabel: renderSettings.RenderLabel,
            RenderInfluences: renderSettings.RenderInfluences,
            RenderTrueInfluenceRange: renderSettings.RenderTrueInfluenceRange,
            RenderHarborBlockedArea: renderSettings.RenderHarborBlockedArea,
            RenderPanorama: renderSettings.RenderPanorama,
            Icons: _vm.Icons,
            CoordinateHelper: _vm.CoordinateHelper,
            FeatureFlags: _vm.FeatureSnapshot
        );

        // Render into DrawingVisual
        DrawingVisual dv = new();
        using (DrawingContext dc = dv.RenderOpen())
        {
            renderer.Render(dc, state);
        }

        // Render base canvas
        int baseWidth = Math.Max(1, (int)Math.Ceiling(widthPx));
        int baseHeight = Math.Max(1, (int)Math.Ceiling(heightPx));
        const int dpi = 96;
        RenderTargetBitmap baseBitmap = new(baseWidth, baseHeight, dpi, dpi, PixelFormats.Pbgra32);
        baseBitmap.Render(dv);

        // Determine overlay dimensions (approximation of v1 docking layout)
        bool addStats = renderSettings.RenderStatistics;
        bool addVersion = renderSettings.RenderVersion;
        int statsWidth = addStats ? 170 : 0; // heuristic width
        int versionHeight = addVersion ? 22 : 0; // heuristic height
        int finalWidth = baseWidth + statsWidth;
        int finalHeight = baseHeight + versionHeight;

        DrawingVisual composite = new();
        using (DrawingContext ctx = composite.RenderOpen())
        {
            // Draw base image
            ctx.DrawImage(baseBitmap, new Rect(0, 0, baseWidth, baseHeight));

            // Statistics overlay (right side)
            if (addStats)
            {
                Rect statsPanelRect = new(baseWidth, 0, statsWidth, baseHeight);
                ctx.DrawRectangle(Brushes.White, new Pen(Brushes.Gray, 1), statsPanelRect);

                // Basic statistics (building count, selected, distinct identifiers)
                int buildingCount = layoutObjects.Count;
                int selectedCount = selectedLayoutObjects.Count;
                int distinctIds = layoutObjects.Select(o => o.Identifier).Distinct().Count();
                string statsText = $"Objects: {buildingCount}\nSelected: {selectedCount}\nTypes: {distinctIds}";
                FormattedText ft = new(
                    statsText,
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    12,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(Control).PixelsPerDip);
                ctx.DrawText(ft, new Point(baseWidth + 6, 6));
            }

            // Version overlay (bottom)
            if (addVersion)
            {
                Rect verRect = new(0, baseHeight, finalWidth, versionHeight);
                ctx.DrawRectangle(Brushes.White, new Pen(Brushes.Gray, 1), verRect);
                string version = "";
                try
                {
                    string versionFile = System.IO.Path.Combine(App.ApplicationPath, "version.txt");
                    if (_fileSystem.File.Exists(versionFile))
                    {
                        version = _fileSystem.File.ReadAllText(versionFile).Trim();
                    }
                }
                catch { }
                if (string.IsNullOrWhiteSpace(version))
                {
                    version = "AnnoDesigner";
                }
                string versionText = $"Version: {version}";
                FormattedText vft = new(
                    versionText,
                    System.Globalization.CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    12,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(Control).PixelsPerDip);
                ctx.DrawText(vft, new Point(6, baseHeight + 4));
            }
        }

        RenderTargetBitmap finalBitmap = new(finalWidth, finalHeight, dpi, dpi, PixelFormats.Pbgra32);
        finalBitmap.Render(composite);

        // Save final image
        BitmapEncoder encoder = AnnoDesigner.Helper.Constants.GetExportImageEncoder();
        encoder.Frames.Add(BitmapFrame.Create(finalBitmap));
        using Stream fs = _fileSystem.FileStream.Create(filename, FileMode.Create);
        encoder.Save(fs);
    }

    // IHotkeySource compatibility
    public void RegisterHotkeys(HotkeyCommandManager manager)
    {
        if (manager == null)
        {
            return;
        }

        try
        {
            // Copy hotkeys known to the VM into the provided manager to keep behaviour parity.
            foreach (Hotkey hot in _vm.HotkeyCommandManager.GetHotkeys())
            {
                try
                {
                    manager.AddHotkey(hot);
                }
                catch
                {
                    // ignore duplicates or mapping errors
                }
            }
        }
        catch
        {
            // Best-effort only; do not throw during UI wiring
        }
    }

    public HotkeyCommandManager HotkeyCommandManager
    {
        get => _vm.HotkeyCommandManager;
        set
        {
            if (value == null)
            {
                return;
            }

            try
            {
                // Merge provided manager hotkeys into VM's manager where possible
                foreach (Hotkey hot in value.GetHotkeys())
                {
                    try
                    {
                        _vm.HotkeyCommandManager.AddHotkey(hot);
                    }
                    catch
                    {
                        // ignore duplicates
                    }
                }
            }
            catch
            {
                // swallow - best effort
            }
        }
    }
}
