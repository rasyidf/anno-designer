using AnnoDesigner.CanvasV2.FeatureFlags;
using AnnoDesigner.CanvasV2.Input;
using AnnoDesigner.CanvasV2.Rendering;
using AnnoDesigner.Core.DataStructures;
using AnnoDesigner.Core.Extensions;
using AnnoDesigner.Core.Layout.Helper;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Models;
using AnnoDesigner.Core.Services;
using AnnoDesigner.CustomEventArgs;
using AnnoDesigner.Extensions;
using AnnoDesigner.Helper;
using AnnoDesigner.Models;
using AnnoDesigner.Services;
using AnnoDesigner.Undo;
using AnnoDesigner.Undo.Operations;
using AnnoDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AnnoDesigner.CanvasV2;

/// <summary>
/// Phase 1 ViewModel extraction. Provides state/logic surface for AnnoCanvasV2.
/// </summary>
public class AnnoCanvasViewModel : INotifyPropertyChanged, IInputHandlerHost, IScrollInfo
{


    public Rect Viewport => _viewport.Absolute;


    public LayoutObject GetObjectAt(Point position)
    {
        // Convert screen position to grid position with viewport offset
        if (PlacedObjects.Count == 0)
        {
            return null;
        }

        Point gridPos = CoordinateHelper.ScreenToFractionalGrid(position, GridSize);
        gridPos = _viewport.OriginToViewport(gridPos);
        Rect probeRect = new(gridPos, new Size(1, 1));
        foreach (LayoutObject lo in PlacedObjects.GetItemsIntersecting(probeRect))
        {
            if (lo.GridRect.Contains(gridPos))
            {
                return lo;
            }
        }
        return null;
    }

    public void AddSelectedObject(LayoutObject obj, bool includeSameObjects = false)
    {
        _ = SelectedObjects.Add(obj);
        if (includeSameObjects)
        {
            IEnumerable<LayoutObject> sameObjects = PlacedObjects.Where(o => o.Identifier == obj.Identifier);
            foreach (LayoutObject sameObj in sameObjects)
            {
                _ = SelectedObjects.Add(sameObj);
            }
        }
        CommandManager.InvalidateRequerySuggested();
    }

    public void AddSelectedObjects(IEnumerable<LayoutObject> objects, bool includeSameObjects = false)
    {
        foreach (LayoutObject obj in objects)
        {
            AddSelectedObject(obj, includeSameObjects);
        }
        CommandManager.InvalidateRequerySuggested();
    }

    public void RemoveSelectedObject(LayoutObject obj, bool includeSameObjects = false)
    {
        _ = SelectedObjects.Remove(obj);
        if (includeSameObjects)
        {
            List<LayoutObject> sameObjects = SelectedObjects.Where(o => o.Identifier == obj.Identifier).ToList();
            foreach (LayoutObject sameObj in sameObjects)
            {
                _ = SelectedObjects.Remove(sameObj);
            }
        }
        CommandManager.InvalidateRequerySuggested();
    }

    public void RemoveSelectedObjects(IEnumerable<LayoutObject> objects, bool includeSameObjects = false)
    {
        foreach (LayoutObject obj in objects)
        {
            RemoveSelectedObject(obj, includeSameObjects);
        }
        CommandManager.InvalidateRequerySuggested();
    }

    public void RemoveSelectedObjects(Predicate<LayoutObject> predicate)
    {
        List<LayoutObject> objectsToRemove = SelectedObjects.Where(obj => predicate(obj)).ToList();
        foreach (LayoutObject obj in objectsToRemove)
        {
            _ = SelectedObjects.Remove(obj);
        }
        CommandManager.InvalidateRequerySuggested();
    }

    public bool TryPlaceCurrentObjects(bool isContinuousDrawing)
    {
        if (CurrentObjects.Count == 0)
        {
            return false;
        }

        List<LayoutObject> objectsToPlace;

        if (isContinuousDrawing)
        {
            // Create copies of current objects for continuous placement
            objectsToPlace = CurrentObjects.Select(obj =>
                new LayoutObject(new AnnoObject(obj.WrappedAnnoObject), CoordinateHelper, _brushCache, _penCache)).ToList();
        }
        else
        {
            objectsToPlace = CurrentObjects.ToList();
        }

        // Register undo operation for placing objects
        UndoManager.RegisterOperation(new AddObjectsOperation<LayoutObject>
        {
            Objects = objectsToPlace,
            Collection = PlacedObjects
        });
        CommandManager.InvalidateRequerySuggested();

        foreach (LayoutObject obj in objectsToPlace)
        {
            PlacedObjects.Add(obj);
        }

        if (!isContinuousDrawing)
        {
            CurrentObjects.Clear();
        }

        InvalidateRender();
        return true;
    }

    public void MoveCurrentObjectsToMouse(Point mousePosition)
    {
        if (CurrentObjects.Count == 0)
        {
            return;
        }

        if (CurrentObjects.Count > 1)
        {
            // Get the center of the current selection
            Rect r = CurrentObjects[0].GridRect;
            foreach (LayoutObject obj in CurrentObjects.Skip(1))
            {
                r.Union(obj.GridRect);
            }

            Point center = CoordinateHelper.GetCenterPoint(r);
            Point mouseGridPosition = CoordinateHelper.ScreenToFractionalGrid(mousePosition, GridSize);
            double dx = mouseGridPosition.X - center.X;
            double dy = mouseGridPosition.Y - center.Y;

            foreach (LayoutObject obj in CurrentObjects)
            {
                Point pos = obj.Position;
                pos = _viewport.OriginToViewport(new Point(pos.X + dx, pos.Y + dy));
                pos = new Point(Math.Floor(pos.X), Math.Floor(pos.Y));
                obj.Position = pos;
            }
        }
        else
        {
            // Single object: center it on the mouse cursor
            Point pos = CoordinateHelper.ScreenToFractionalGrid(mousePosition, GridSize);
            Size size = CurrentObjects[0].Size;
            pos.X -= size.Width / 2;
            pos.Y -= size.Height / 2;
            pos = _viewport.OriginToViewport(pos);
            pos = new Point(Math.Round(pos.X, MidpointRounding.AwayFromZero), Math.Round(pos.Y, MidpointRounding.AwayFromZero));
            CurrentObjects[0].Position = pos;
        }

        InvalidateRender();
    }

    public void RecalculateSelectionContainsNotIgnoredObject()
    {
        // Migrated from v1: determines if selection contains non-ignored objects
        // Used for toolbar/UI state updates
        SelectionContainsNotIgnoredObject = SelectedObjects.Any(x => !x.IsIgnoredObject());
    }


    public void ReindexMovedObjects()
    {
        // Migrated from v1: reindex objects in QuadTree after drag operations
        // The input handler stores old positions in _oldObjectPositions
        // For now, just trigger a full rebuild of the QuadTree
        // This is less efficient than selective reindexing but ensures correctness
        List<LayoutObject> allObjects = PlacedObjects.ToList();
        PlacedObjects.Clear();
        foreach (LayoutObject obj in allObjects)
        {
            PlacedObjects.Add(obj);
        }
    }

    public void BringObjectsToFront(IEnumerable<LayoutObject> objects)
    {
        // Get the maximum Z-index currently in use
        int maxZIndex = PlacedObjects.Any() ? PlacedObjects.Max(o => o.ZIndex) : 0;

        // Increment and assign to moved objects
        foreach (LayoutObject obj in objects)
        {
            obj.ZIndex = ++maxZIndex;
        }

        InvalidateRender();
    }


    public bool ObjectIntersectionExists(LayoutObject a, LayoutObject b)
    {
        return a.Bounds.IntersectsWith(b.Bounds);
    }


    public void InvalidateBounds()
    {
        // Migrated from v1: compute layout bounds
        _layoutBounds = ComputeBoundingRect(PlacedObjects);

        // Update scrollable area
        Rect r = _viewport.Absolute;
        r.Union(_layoutBounds);
        _scrollableBounds = r;
    }


    public void UpdateStatistics(bool includeBuildings = true)
    {
        StatisticsUpdated?.Invoke(this, includeBuildings ? UpdateStatisticsEventArgs.All : UpdateStatisticsEventArgs.Empty);
    }
    // migrated from AnnoCanvas.xaml.cs � events
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<UpdateStatisticsEventArgs> StatisticsUpdated; // migrated from StatisticsUpdated
    public event EventHandler<EventArgs> ColorsInLayoutUpdated; // migrated from ColorsInLayoutUpdated
    public event EventHandler<FileLoadedEventArgs> OnLoadedFileChanged; // migrated from OnLoadedFileChanged
    public event EventHandler<OpenFileEventArgs> OpenFileRequested; // migrated from OpenFileRequested
    public event EventHandler<SaveFileEventArgs> SaveFileRequested; // migrated from SaveFileRequested
    // Request events: these tell the host (UI / MainViewModel) that it should show a dialog
    // or otherwise perform the actual file open/save interaction. The VM no longer
    // performs any direct file IO or shows dialogs.
    public event EventHandler RequestOpenFile; // raised when the VM wants the host to open a file (show dialog)
    public event EventHandler RequestSaveAs; // raised when the VM wants the host to perform Save As (show dialog)
    public event EventHandler RequestNewFile; // raised when the VM wants the host to create a new layout

    // Compatibility events expected by IAnnoCanvas consumers
    public event Action<LayoutObject> OnCurrentObjectChanged; // raised when current object changes
    public event Action<string> OnStatusMessageChanged; // raised for status message updates

    /// <summary>
    /// Raised when a render redraw is required (view calls InvalidateVisual).
    /// </summary>
    public event Action RenderInvalidated; // Phase 1 requirement
    /// <summary>
    /// Raised when scroll metrics changed (view triggers ScrollOwner.InvalidateScrollInfo()).
    /// </summary>
    public event Action ScrollInvalidated; // Phase 1 requirement

    private void RaisePropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // migrated from AnnoCanvas.xaml.cs � services
    public IUndoManager UndoManager { get; }
    public IClipboardService ClipboardService { get; }
    public ICoordinateHelper CoordinateHelper { get; }
    public IAppSettings AppSettings { get; }

    private readonly IBrushCache _brushCache;
    private readonly IPenCache _penCache;
    private readonly ILayoutLoader _layoutLoader;
    private readonly ILocalizationHelper _localizationHelper;
    private readonly IMessageBoxService _messageBoxService;
    private readonly IFeatureFlags _featureFlags;

    // state collections migrated
    public QuadTree<LayoutObject> PlacedObjects { get; set; }
    public HashSet<LayoutObject> SelectedObjects { get; set; }
    public List<LayoutObject> CurrentObjects { get; set; }

    // UI state helpers
    public bool SelectionContainsNotIgnoredObject
    {
        get; private set
        {
            if (field != value)
            {
                field = value;
                RaisePropertyChanged();
            }
        }
    }

    // configuration migrated
    public BuildingPresets BuildingPresets { get; init; } // migrated from BuildingPresets
    public Dictionary<string, IconImage> Icons { get; init; } = new(StringComparer.OrdinalIgnoreCase); // migrated from Icons

    // view state migrated properties
    public int GridSize
    {
        get; set
        {
            int tmp = value;
            if (tmp < Constants.GridStepMin)
            {
                tmp = Constants.GridStepMin;
            }
            else if (tmp > Constants.GridStepMax)
            {
                tmp = Constants.GridStepMax;
            }

            if (field != tmp)
            {
                field = tmp;
                RaisePropertyChanged();
                InvalidateRender();
            }
        }
    } = Constants.GridStepDefault;

    // Feature toggles now sourced from feature flags snapshot
    public CanvasFeatureFlags FeatureSnapshot => _featureFlags.Snapshot();

    // Derived simple flags (for parity with v1 public API); raising these triggers cache invalidation.
    public bool RenderGrid => FeatureSnapshot.RenderGrid; // migrated from RenderGrid
    public bool RenderInfluences => FeatureSnapshot.RenderInfluences; // migrated from RenderInfluences
    public bool RenderLabel => FeatureSnapshot.RenderLabels; // migrated from RenderLabel
    public bool RenderIcon => FeatureSnapshot.RenderIcons; // migrated from RenderIcon
    public bool RenderTrueInfluenceRange => FeatureSnapshot.RenderTrueInfluenceRange; // migrated from RenderTrueInfluenceRange
    public bool RenderHarborBlockedArea => FeatureSnapshot.RenderHarborBlockedArea; // migrated from RenderHarborBlockedArea
    public bool RenderPanorama => FeatureSnapshot.RenderPanorama; // migrated from RenderPanorama



    // Fix for CS0738: Ensure the return type of CurrentMode matches the interface definition.
    public Input.MouseMode CurrentMode
    {
        get; set
        {
            if (field != value)
            {
                field = value;
                RaisePropertyChanged();
            }
        }
    } = Input.MouseMode.Standard;

    public string StatusMessage
    {
        get; set
        {
            if (field != value)
            {
                field = value;
                RaisePropertyChanged();
                OnStatusMessageChanged?.Invoke(field ?? string.Empty);
            }
        }
    }

    public string LoadedFile
    {
        get; set
        {
            if (field != value)
            {
                field = value;
                RaisePropertyChanged();
                OnLoadedFileChanged?.Invoke(this, new FileLoadedEventArgs(value));
            }
        }
    }

    // Viewport migrated minimal
    private readonly Viewport _viewport = new(); // migrated from _viewport
    private Rect _layoutBounds; // bounds of all placed objects
    private Rect _scrollableBounds; // scrollable area
    public Rect ViewportRect => _viewport.Absolute;

    // Input handler (Phase 2 skeleton)
    public CanvasInputHandler InputHandler { get; }

    // Floating toolbox (Phase 2 V2)
    public FloatingToolboxViewModel ToolboxViewModel { get; }

    // Hotkeys (Phase 1) - manager injected so tests can reuse
    public HotkeyCommandManager HotkeyCommandManager { get; }

    // Commands migrated (only subset; others can be added incrementally)
    public ICommand RotateCommand { get; }
    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand UndoCommand { get; }
    public ICommand RedoCommand { get; }
    public ICommand SelectSameIdentifierCommand { get; }

    // Internal selection collision rect (needed for input and renderer snapshot)
    internal Rect SelectionRectInternal { get; set; } // migrated from _selectionRect

    public void SetSelectionRect(Rect rect)
    {
        SelectionRectInternal = rect;
        InvalidateRender();
    }

    public AnnoCanvasViewModel(IFeatureFlags featureFlags,
                               IUndoManager undoManager,
                               IClipboardService clipboardService,
                               IAppSettings appSettings,
                               ICoordinateHelper coordinateHelper,
                               IBrushCache brushCache,
                               IPenCache penCache,
                               ILayoutLoader layoutLoader,
                               ILocalizationHelper localizationHelper,
                               IMessageBoxService messageBoxService,
                               HotkeyCommandManager hotkeyManager)
    {
        _featureFlags = featureFlags;
        UndoManager = undoManager;
        ClipboardService = clipboardService;
        AppSettings = appSettings;
        CoordinateHelper = coordinateHelper;
        _brushCache = brushCache;
        _penCache = penCache;
        _layoutLoader = layoutLoader;
        _localizationHelper = localizationHelper;
        _messageBoxService = messageBoxService;
        HotkeyCommandManager = hotkeyManager;

        PlacedObjects = new QuadTree<LayoutObject>(new Rect(-128, -128, 256, 256)); // migrated initialization
        SelectedObjects = [];
        CurrentObjects = [];

        // Subscribe to flag changes -> invalidate render
        _featureFlags.FeatureChanged += (_, __) => InvalidateRender();

        // Commands migrated with CanExecute predicates
        RotateCommand = new RelayCommand(
            _ => ExecuteRotate(),
            _ => CurrentObjects.Count > 0 || SelectedObjects.Count > 0);
        CopyCommand = new RelayCommand(
            _ => ExecuteCopy(),
            _ => SelectedObjects.Count > 0);
        PasteCommand = new RelayCommand(_ => ExecutePaste()); // Always enabled - Paste will handle empty clipboard
        DeleteCommand = new RelayCommand(
            _ => ExecuteDelete(),
            _ => SelectedObjects.Count > 0);
        UndoCommand = new RelayCommand(
            _ => { UndoManager.Undo(); CommandManager.InvalidateRequerySuggested(); InvalidateRender(); },
            _ => UndoManager.CanUndo);
        RedoCommand = new RelayCommand(
            _ => { UndoManager.Redo(); CommandManager.InvalidateRequerySuggested(); InvalidateRender(); },
            _ => UndoManager.CanRedo);
        SelectSameIdentifierCommand = new RelayCommand(
            _ => ExecuteSelectSameIdentifier(),
            _ =>
            {
                // Command should be enabled when there's at least one selected object with a valid identifier
                return SelectedObjects.Count > 0 && SelectedObjects.Any(o => !string.IsNullOrWhiteSpace(o.Identifier));
            });

        // Initialize floating toolbox
        ToolboxViewModel = new FloatingToolboxViewModel();
        ToolboxViewModel.SelectedToolChanged += OnToolboxToolChanged;

        // Input handler (Phase 2) constructed last so it can reference this host
        // Use the new HotkeyService adapter to decouple input from the concrete HotkeyCommandManager.
        InputHandler = new CanvasInputHandler(this, new HotkeyService(hotkeyManager));
    }

    /// <summary>
    /// Attempt to set a runtime feature flag if the backing implementation supports it.
    /// Returns true when the flag was updated, false otherwise.
    /// This enables adapters to toggle canvas rendering features at runtime.
    /// </summary>
    public bool TrySetFeatureFlag(string name, object value)
    {
        if (_featureFlags is SimpleFeatureFlags simple)
        {
            simple.Set(name, value);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Handles tool changes from the floating toolbox.
    /// Maps the selected tool to the appropriate mouse mode.
    /// </summary>
    private void OnToolboxToolChanged(object sender, CanvasTool tool)
    {
        switch (tool)
        {
            case CanvasTool.Select:
                CurrentMode = Input.MouseMode.Standard;
                // Clear any current objects being placed
                if (CurrentObjects.Count > 0)
                {
                    CurrentObjects.Clear();
                    InvalidateRender();
                }
                break;

            case CanvasTool.Draw:
                // Draw mode will be activated when objects are placed
                // This is handled by the placement logic
                CurrentMode = Input.MouseMode.PlaceObjects;
                break;

            case CanvasTool.Move:
                // Pan mode - will be activated on drag
                CurrentMode = Input.MouseMode.DragAllStart;
                break;

            case CanvasTool.Delete:
                CurrentMode = Input.MouseMode.DeleteObject;
                break;

            case CanvasTool.Duplicate:
                // Duplicate is handled via command when clicking on objects
                CurrentMode = Input.MouseMode.Standard;
                break;

            case CanvasTool.Measure:
            case CanvasTool.Eyedropper:
                // Future implementation
                CurrentMode = Input.MouseMode.Standard;
                break;
        }
    }

    private void ExecuteRotate()
    {
        if (CurrentObjects.Count == 1)
        {
            // migrated from ExecuteRotate
            CurrentObjects[0].Size = CoordinateHelper.Rotate(CurrentObjects[0].Size);
            CurrentObjects[0].Direction = CoordinateHelper.Rotate(CurrentObjects[0].Direction);
        }
        else if (CurrentObjects.Count > 1)
        {
            foreach (LayoutObject lo in CurrentObjects)
            {
                lo.Bounds = CoordinateHelper.Rotate(lo.Bounds);
                lo.Direction = CoordinateHelper.Rotate(lo.Direction);
            }
        }
        else if (SelectedObjects.Count > 0)
        {
            // Rotate selected objects in place without creating copies
            foreach (LayoutObject lo in SelectedObjects)
            {
                lo.Size = CoordinateHelper.Rotate(lo.Size);
                lo.Direction = CoordinateHelper.Rotate(lo.Direction);
            }
        }
        InvalidateRender();
    }

    private void ExecuteCopy()
    {
        if (SelectedObjects.Count != 0)
        {
            ClipboardService.Copy(SelectedObjects.Select(x => x.WrappedAnnoObject));
            StatusMessage = $"{SelectedObjects.Count} {_localizationHelper.GetLocalization(SelectedObjects.Count == 1 ? "ItemCopied" : "ItemsCopied")}";
        }
    }

    private void ExecutePaste()
    {
        ICollection<AnnoObject> objects = ClipboardService.Paste();
        if (objects.Count > 0)
        {
            CurrentObjects = objects.Select(x => new LayoutObject(x, CoordinateHelper, _brushCache, _penCache)).ToList();
            InvalidateRender();
        }
    }

    private void ExecuteDelete()
    {
        if (SelectedObjects.Count == 0)
        {
            return;
        }

        UndoManager.RegisterOperation(new RemoveObjectsOperation<LayoutObject>
        {
            Objects = SelectedObjects.ToList(),
            Collection = PlacedObjects
        });
        CommandManager.InvalidateRequerySuggested();
        foreach (LayoutObject item in SelectedObjects)
        {
            _ = PlacedObjects.Remove(item);
        }
        SelectedObjects.Clear();
        StatisticsUpdated?.Invoke(this, UpdateStatisticsEventArgs.All);
        ColorsInLayoutUpdated?.Invoke(this, EventArgs.Empty);
        InvalidateRender();
    }

    // Track last mouse position reported by input handler for cursor-centric commands.
    private Point _lastMousePosition = new(double.NaN, double.NaN);
    public void ReportMousePosition(Point p)
    {
        if (_lastMousePosition != p)
        {
            _lastMousePosition = p;
            // Invalidate command states when mouse moves, so CanExecute re-evaluates
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private void ExecuteSelectSameIdentifier()
    {
        // Get the first selected object with a valid identifier
        LayoutObject referenceObject = SelectedObjects.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.Identifier));
        if (referenceObject is null)
        {
            return;
        }

        string targetIdentifier = referenceObject.Identifier;

        // Check if all objects with this identifier are already selected
        List<LayoutObject> allObjectsWithIdentifier = PlacedObjects.Where(o => o.Identifier == targetIdentifier).ToList();
        bool allAlreadySelected = allObjectsWithIdentifier.All(SelectedObjects.Contains);

        if (allAlreadySelected)
        {
            // Deselect all objects with this identifier
            List<LayoutObject> objectsToRemove = SelectedObjects.Where(o => o.Identifier == targetIdentifier).ToList();
            RemoveSelectedObjects(objectsToRemove);
        }
        else
        {
            // Select all objects with this identifier
            AddSelectedObjects(allObjectsWithIdentifier);
        }

        RecalculateSelectionContainsNotIgnoredObject();
        UpdateStatistics(includeBuildings: true);
        InvalidateRender();
    }

    public void SetCurrentObject(LayoutObject obj) // migrated from SetCurrentObject
    {
        CurrentObjects.Clear();
        CurrentObjects.Add(obj);
        OnCurrentObjectChanged?.Invoke(obj);
        InvalidateRender();
    }

    /// <summary>
    /// Raise statistics event for external callers (compatibility with IAnnoCanvas).
    /// </summary>
    public void RaiseStatisticsUpdated(UpdateStatisticsEventArgs args)
    {
        StatisticsUpdated?.Invoke(this, args);
    }

    /// <summary>
    /// Raise colors-in-layout updated event for external callers.
    /// </summary>
    public void RaiseColorsInLayoutUpdated()
    {
        ColorsInLayoutUpdated?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Reset viewport to origin (compatibility helper).
    /// </summary>
    public void ResetViewport()
    {
        _viewport.Left = 0;
        _viewport.Top = 0;
        InvalidateScroll();
        InvalidateRender();
    }

    public void ResetZoom()
    {
        GridSize = Constants.GridStepDefault; // migrated from ResetZoom
    }

    public void Normalize(int border) // migrated from Normalize(int)
    {
        if (PlacedObjects.Count == 0)
        {
            return;
        }

        double dx = PlacedObjects.Min(_ => _.Position.X) - border;
        double dy = PlacedObjects.Min(_ => _.Position.Y) - border;
        Vector diff = new(dx, dy);
        if (diff.LengthSquared <= 0)
        {
            return;
        }

        UndoManager.RegisterOperation(new MoveObjectsOperation<LayoutObject>
        {
            ObjectPropertyValues = PlacedObjects.Select(obj => (obj, obj.Bounds, new Rect(obj.Position - diff, obj.Size))).ToList(),
            QuadTree = PlacedObjects
        });
        foreach (LayoutObject item in PlacedObjects.ToList())
        {
            PlacedObjects.Move(item, -diff);
        }
        InvalidateBounds(); // Ensure bounds are recalculated after normalization
        InvalidateRender();
    }

    /// <summary>
    /// Checks for unsaved changes and prompts user to save. Migrated from v1.
    /// </summary>
    public async Task<bool> CheckUnsavedChanges()
    {
        if (UndoManager.IsDirty)
        {
            bool? save = await _messageBoxService.ShowQuestionWithCancel(
                _localizationHelper.GetLocalization("SaveUnsavedChanges"),
                _localizationHelper.GetLocalization("UnsavedChanged")
            );

            if (save == null)
            {
                return false;
            }
            if (save.Value)
            {
                return Save();
            }
        }

        return true;
    }

    /// <summary>
    /// Checks for unsaved changes before crash. Migrated from v1.
    /// </summary>
    public async void CheckUnsavedChangesBeforeCrash()
    {
        if (UndoManager.IsDirty)
        {
            bool save = await _messageBoxService.ShowQuestion(
                _localizationHelper.GetLocalization("SaveUnsavedChanges"),
                _localizationHelper.GetLocalization("UnsavedChangedBeforeCrash")
            );

            if (save)
            {
                _ = SaveAs();
            }
        }
    }

    /// <summary>
    /// Saves the current layout. Migrated from v1.
    /// </summary>
    public bool Save()
    {
        if (string.IsNullOrEmpty(LoadedFile))
        {
            // No filename known: ask host/UI to perform Save As (choose location and then call back into SaveFileRequested)
            RequestSaveAs?.Invoke(this, EventArgs.Empty);
            return false;
        }

        // Request the host to save to the current loaded file path. The actual write is performed by the host.
        SaveFileRequested?.Invoke(this, new SaveFileEventArgs(LoadedFile));
        // Fire OnLoadedFileChanged event to trigger recalculation of MainWindowTitle
        OnLoadedFileChanged?.Invoke(this, new FileLoadedEventArgs(LoadedFile));
        return true;
    }

    /// <summary>
    /// Opens a dialog and saves the current layout to file. Migrated from v1.
    /// </summary>
    public bool SaveAs()
    {
        // VM must not show UI dialogs. Ask the host/UI to perform a Save As dialog and save.
        RequestSaveAs?.Invoke(this, EventArgs.Empty);
        return false; // host will perform the actual save and set LoadedFile when done
    }

    /// <summary>
    /// Request the host/UI to open a file (show an Open dialog). The VM does not perform dialogs.
    /// The host should call back into the MainViewModel (e.g. OpenFileAsync) and then call
    /// the VM's Open/Load flow by raising OpenFileRequested with the chosen path.
    /// </summary>
    public void RequestOpen()
    {
        RequestOpenFile?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Request the host/UI to create a new layout. The host should ensure unsaved changes are handled
    /// and then initialize a new layout (via MainViewModel.OpenLayout or similar).
    /// </summary>
    public void RequestNew()
    {
        RequestNewFile?.Invoke(this, EventArgs.Empty);
    }

    // Selection helpers migrated
    public void AddSelectedObject(LayoutObject obj)
    {
        _ = SelectedObjects.Add(obj);
        System.Windows.Input.CommandManager.InvalidateRequerySuggested(); // Refresh command states
    }

    public void RemoveSelectedObject(LayoutObject obj)
    {
        _ = SelectedObjects.Remove(obj);
        System.Windows.Input.CommandManager.InvalidateRequerySuggested(); // Refresh command states
    }

    public Rect ComputeBoundingRect(IEnumerable<LayoutObject> objects) // migrated from ComputeBoundingRect
    {
        StatisticsCalculationHelper statsHelper = new();
        return (Rect)statsHelper.CalculateStatistics(objects.Select(o => o.WrappedAnnoObject), includeRoads: true, includeIgnoredObjects: true);
    }

    internal void InvalidateRender()
    {
        RenderInvalidated?.Invoke();
    }

    internal void InvalidateScroll()
    {
        ScrollInvalidated?.Invoke();
    }

    // Expose as interface implementation
    void IInputHandlerHost.InvalidateRender()
    {
        InvalidateRender();
    }

    void IInputHandlerHost.InvalidateScroll()
    {
        InvalidateScroll();
    }

    // Phase 3: produce immutable snapshot for renderer.
    public RenderState CreateRenderState()
    {
        // Simplified objects to draw: all inside viewport for now.
        Rect vp = ViewportRect;
        List<LayoutObject> objects = PlacedObjects.GetItemsIntersecting(vp).ToList();
        CanvasFeatureFlags featureFlags = FeatureSnapshot;

        return new RenderState(
            Viewport: vp,
            Translate: new TranslateTransform(-_viewport.Left * GridSize, -_viewport.Top * GridSize),
            GuidelineSet: null,
            ObjectsToDraw: objects,
            SelectedObjects: SelectedObjects.ToList(),
            CurrentObjects: CurrentObjects.ToList(),
            AllPlacedObjects: PlacedObjects.ToList(),
            GridSize: GridSize,
            SelectionRect: SelectionRectInternal,
            CurrentMode: CurrentMode,
            RenderGrid: featureFlags.RenderGrid,
            RenderIcon: featureFlags.RenderIcons,
            RenderLabel: featureFlags.RenderLabels,
            RenderInfluences: featureFlags.RenderInfluences,
            RenderTrueInfluenceRange: featureFlags.RenderTrueInfluenceRange,
            RenderHarborBlockedArea: featureFlags.RenderHarborBlockedArea,
            RenderPanorama: featureFlags.RenderPanorama,
            Icons: Icons,
            CoordinateHelper: CoordinateHelper,
            FeatureFlags: featureFlags);
    }

    #region IScrollInfo pass-through implementation
    public ScrollViewer? ScrollOwner { get; set; }
    public bool CanVerticallyScroll { get; set; }
    public bool CanHorizontallyScroll { get; set; }
    public double ExtentWidth => _scrollableBounds.Width;
    public double ExtentHeight => _scrollableBounds.Height;
    public double ViewportWidth
    {
        get => _viewport.Width;
        set
        {
            if (_viewport.Width != value)
            {
                _viewport.Width = value;
                InvalidateScroll();
            }
        }
    }
    public double ViewportHeight
    {
        get => _viewport.Height;
        set
        {
            if (_viewport.Height != value)
            {
                _viewport.Height = value;
                InvalidateScroll();
            }
        }
    }
    public double HorizontalOffset => _viewport.Left;
    public double VerticalOffset => _viewport.Top;


    public void LineUp() { _viewport.Top -= 1; InvalidateScroll(); InvalidateRender(); }
    public void LineDown() { _viewport.Top += 1; InvalidateScroll(); InvalidateRender(); }
    public void LineLeft() { _viewport.Left -= 1; InvalidateScroll(); InvalidateRender(); }
    public void LineRight() { _viewport.Left += 1; InvalidateScroll(); InvalidateRender(); }
    public void PageUp() { _viewport.Top -= _viewport.Height; InvalidateScroll(); InvalidateRender(); }
    public void PageDown() { _viewport.Top += _viewport.Height; InvalidateScroll(); InvalidateRender(); }
    public void PageLeft() { _viewport.Left -= _viewport.Width; InvalidateScroll(); InvalidateRender(); }
    public void PageRight() { _viewport.Left += _viewport.Width; InvalidateScroll(); InvalidateRender(); }
    public void MouseWheelUp() { }
    public void MouseWheelDown() { }
    public void MouseWheelLeft() { }
    public void MouseWheelRight() { }
    public void SetHorizontalOffset(double offset) { _viewport.Left = offset; InvalidateScroll(); InvalidateRender(); }
    public void SetVerticalOffset(double offset) { _viewport.Top = offset; InvalidateScroll(); InvalidateRender(); }
    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
        return ViewportRect;
    }
    #endregion
}
