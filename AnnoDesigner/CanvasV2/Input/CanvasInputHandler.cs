using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using AnnoDesigner.Services;
using AnnoDesigner.Undo.Operations;
using AnnoDesigner.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AnnoDesigner.CanvasV2.Input;

/// <summary>
/// Handles all mouse and keyboard input for the canvas.
/// migrated from AnnoCanvas.xaml.cs — Phase 2: Input Controller Extraction
/// </summary>
public class CanvasInputHandler
{
    private readonly IInputHandlerHost _host;
    private readonly IHotkeyService _hotkeys;

    // Mouse state
    private Point _mousePosition = new(double.NaN, double.NaN);
    private Point _mouseDragStart;
    private Rect _selectionRect;
    private Rect _collisionRect;
    private readonly List<(LayoutObject Item, Rect OldGridRect)> _oldObjectPositions = [];

    public CanvasInputHandler(
        IInputHandlerHost host,
        IHotkeyService hotkeys)
    {
        _host = host;
        _hotkeys = hotkeys;
    }

    // Compatibility constructor: accept existing HotkeyCommandManager instances
    // and wrap them in the new IHotkeyService adapter so existing call sites
    // can gradually migrate.
    public CanvasInputHandler(
        IInputHandlerHost host,
        HotkeyCommandManager hotkeys)
        : this(host, new HotkeyService(hotkeys))
    {
    }

    // Compatibility constructor (deprecated - use the simpler one above)
    public CanvasInputHandler(
        IInputHandlerHost host,
        HotkeyCommandManager hotkeys,
        ICoordinateHelper coordinateHelper,
        IAppSettings appSettings)
        : this(host, hotkeys)
    {
        // coordinateHelper and appSettings are now accessed via host.CoordinateHelper and host.AppSettings
    }

    public void HandleMouseWheel(MouseWheelEventArgs e, Point position)
    {
        int delta = e.Delta > 0 ? 1 : -1;
        _host.GridSize += delta;
        _host.InvalidateRender();
    }

    public void HandleKeyDown(KeyEventArgs e)
    {
        _hotkeys.HandleCommand(e);
        if (e.Handled)
        {
            _host.InvalidateRender();
        }
    }

    /// <summary>
    /// migrated from AnnoCanvas.xaml.cs — OnMouseDown
    /// </summary>
    public void HandleMouseDown(MouseButtonEventArgs e, Point mousePosition)
    {
        _mousePosition = mousePosition;
        _host.ReportMousePosition(mousePosition);
        _host.MoveCurrentObjectsToMouse(mousePosition);

        // Check for Ctrl+DoubleClick to trigger "Select Same Identifier"
        if (e.ClickCount == 2 && e.LeftButton == MouseButtonState.Pressed && IsControlPressed())
        {
            // Execute select same identifier command if there's a selected object with valid identifier
            if (_host.SelectedObjects.Count > 0 && _host.SelectedObjects.Any(o => !string.IsNullOrWhiteSpace(o.Identifier)))
            {
                // Get the first selected object with a valid identifier
                LayoutObject referenceObject = _host.SelectedObjects.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.Identifier));
                if (referenceObject != null)
                {
                    string targetIdentifier = referenceObject.Identifier;

                    // Select all objects with this identifier
                    List<LayoutObject> allObjectsWithIdentifier = _host.PlacedObjects.Where(o => o.Identifier == targetIdentifier).ToList();
                    _host.AddSelectedObjects(allObjectsWithIdentifier);

                    _host.RecalculateSelectionContainsNotIgnoredObject();
                    _host.UpdateStatistics(includeBuildings: true);
                    _host.InvalidateRender();
                    e.Handled = true;
                    return;
                }
            }
        }

        _hotkeys.HandleCommand(e);
        if (e.Handled)
        {
            _host.InvalidateRender();
            return;
        }
        _mouseDragStart = _mousePosition;

        if (e.LeftButton == MouseButtonState.Pressed && e.RightButton == MouseButtonState.Pressed)
        {
            // Two-button drag to pan entire canvas
            if (_host.CurrentMode == MouseMode.DragSelection)
            {
                _host.UndoManager.RegisterOperation(new MoveObjectsOperation<LayoutObject>()
                {
                    ObjectPropertyValues = _oldObjectPositions.Select(pair => (pair.Item, pair.OldGridRect, pair.Item.Bounds)).ToList(),
                    QuadTree = _host.PlacedObjects
                });
                _host.ReindexMovedObjects();
            }
            _host.CurrentMode = MouseMode.DragAllStart;
        }
        else if (e.LeftButton == MouseButtonState.Pressed && _host.CurrentObjects.Count != 0)
        {
            _ = _host.TryPlaceCurrentObjects(isContinuousDrawing: false);
        }
        else if (e.LeftButton == MouseButtonState.Pressed && _host.CurrentObjects.Count == 0)
        {
            LayoutObject obj = _host.GetObjectAt(_mousePosition);
            if (obj is null)
            {
                _host.CurrentMode = MouseMode.SelectionRectStart;
            }
            else
            {
                // If already selected, start drag; else select
                if (_host.SelectedObjects.Contains(obj))
                {
                    _host.CurrentMode = MouseMode.DragSelectionStart;
                }
                else
                {
                    if (!(IsControlPressed() || IsShiftPressed()))
                    {
                        _host.SelectedObjects.Clear();
                    }
                    _host.AddSelectedObject(obj);
                    _host.RecalculateSelectionContainsNotIgnoredObject();
                    _host.CurrentMode = MouseMode.DragSingleStart;
                }
            }
        }

        _host.InvalidateRender();
    }

    /// <summary>
    /// migrated from AnnoCanvas.xaml.cs — OnMouseMove
    /// </summary>
    public void HandleMouseMove(MouseEventArgs e, Point mousePosition)
    {
        _mousePosition = mousePosition;
        _host.ReportMousePosition(mousePosition);
        _host.MoveCurrentObjectsToMouse(mousePosition);

        // Check if user begins to drag (moved at least 1 pixel)
        if (Math.Abs(_mouseDragStart.X - _mousePosition.X) >= 1 ||
            Math.Abs(_mouseDragStart.Y - _mousePosition.Y) >= 1)
        {
            switch (_host.CurrentMode)
            {
                case MouseMode.SelectionRectStart:
                    _host.CurrentMode = MouseMode.SelectionRect;
                    _selectionRect = new Rect();
                    break;
                case MouseMode.DragSelectionStart:
                    _collisionRect = _host.ComputeBoundingRect(_host.SelectedObjects);
                    _host.CurrentMode = MouseMode.DragSelection;
                    break;
                case MouseMode.DragSingleStart:
                    // Do not clear selection, just start dragging the selected object
                    LayoutObject obj = _host.GetObjectAt(_mouseDragStart);
                    if (obj != null)
                    {
                        if (!_host.SelectedObjects.Contains(obj))
                        {
                            _host.SelectedObjects.Clear();
                            _host.AddSelectedObject(obj, ShouldAffectObjectsWithIdentifier());
                        }
                        _host.RecalculateSelectionContainsNotIgnoredObject();
                        _collisionRect = obj.GridRect;
                    }
                    _host.CurrentMode = MouseMode.DragSelection;
                    break;
                case MouseMode.DragAllStart:
                    _host.CurrentMode = MouseMode.DragAll;
                    break;
            }
        }

        if (_host.CurrentMode == MouseMode.DragAll)
        {
            HandleDragAllMove();
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (_host.CurrentObjects.Count != 0)
            {
                _host.CurrentMode = MouseMode.PlaceObjects;
                _ = _host.TryPlaceCurrentObjects(isContinuousDrawing: true);
            }
            else
            {
                // Selection of multiple objects
                switch (_host.CurrentMode)
                {
                    case MouseMode.SelectionRect:
                        HandleSelectionRectMove();
                        break;
                    case MouseMode.DragSelection:
                        HandleDragSelectionMove();
                        return; // Early return to skip InvalidateVisual at end
                }
            }
        }

        _host.InvalidateRender();
    }

    /// <summary>
    /// migrated from AnnoCanvas.xaml.cs — OnMouseUp
    /// </summary>
    public void HandleMouseUp(MouseButtonEventArgs e, Point mousePosition)
    {
        _mousePosition = mousePosition;
        _host.ReportMousePosition(mousePosition);
        _host.MoveCurrentObjectsToMouse(mousePosition);

        if (_host.CurrentMode == MouseMode.DragAll)
        {
            if (e.LeftButton == MouseButtonState.Released && e.RightButton == MouseButtonState.Released)
            {
                _host.CurrentMode = MouseMode.Standard;
            }
            return;
        }

        if (e.ChangedButton == MouseButton.Left && _host.CurrentObjects.Count == 0)
        {
            switch (_host.CurrentMode)
            {
                case MouseMode.DragSingleStart:
                    HandleStandardLeftClick();
                    break;
                case MouseMode.DragSelectionStart:
                    _host.CurrentMode = MouseMode.Standard;
                    break;
                default:
                    HandleStandardLeftClick();
                    break;
                case MouseMode.SelectSameIdentifier:
                    _host.CurrentMode = MouseMode.Standard;
                    break;
                case MouseMode.SelectionRect:
                    // Do NOT clear selection on mouse up; keep selection for copy/delete
                    _collisionRect = _host.ComputeBoundingRect(_host.SelectedObjects);
                    _host.SetSelectionRect(Rect.Empty);
                    _host.CurrentMode = MouseMode.Standard;
                    // Do not remove ignored objects here, keep selection for further actions
                    break;
                case MouseMode.DragSelection:
                    FinalizeDragSelection();
                    break;
            }
        }
        else if (e.ChangedButton == MouseButton.Left && _host.CurrentObjects.Count != 0)
        {
            _host.CurrentMode = MouseMode.PlaceObjects;
        }
        else if (e.ChangedButton == MouseButton.Right)
        {
            // Only clear selection if nothing is selected and no objects are active
            if (_host.SelectedObjects.Count > 0 && _host.CurrentObjects.Count == 0)
            {
                // Show context menu for copy/delete (handled elsewhere)
                // Do NOT clear selection here, keep selection highlight
                // Optionally, trigger context menu logic if needed
            }
            else
            {
                HandleRightClick();
            }
        }

        _host.InvalidateRender();
    }

    #region Private Helper Methods

    private void HandleDragAllMove()
    {
        // Pan the viewport
        int dx = (int)_host.CoordinateHelper.ScreenToGrid(_mousePosition.X - _mouseDragStart.X, _host.GridSize);
        int dy = (int)_host.CoordinateHelper.ScreenToGrid(_mousePosition.Y - _mouseDragStart.Y, _host.GridSize);

        Rect viewport = _host.Viewport;
        if (_host.AppSettings.InvertPanningDirection)
        {
            viewport.X -= dx;
            viewport.Y -= dy;
        }
        else
        {
            viewport.X += dx;
            viewport.Y += dy;
        }

        // Adjust drag start to compensate
        _mouseDragStart.X += _host.CoordinateHelper.GridToScreen(dx, _host.GridSize);
        _mouseDragStart.Y += _host.CoordinateHelper.GridToScreen(dy, _host.GridSize);

        _host.InvalidateScroll();
    }

    private void HandleSelectionRectMove()
    {
        if (IsControlPressed() || IsShiftPressed())
        {
            // Remove previously selected by the selection rect
            if (ShouldAffectObjectsWithIdentifier())
            {
                _host.RemoveSelectedObjects(
                    _host.SelectedObjects.Where(_ => _.CalculateScreenRect(_host.GridSize).IntersectsWith(_selectionRect)).ToList(),
                    true
                );
            }
            else
            {
                _host.RemoveSelectedObjects(x => x.CalculateScreenRect(_host.GridSize).IntersectsWith(_selectionRect));
            }
        }
        else
        {
            _host.SelectedObjects.Clear();
        }

        // Adjust selection rectangle
        _selectionRect = new Rect(_mouseDragStart, _mousePosition);
        _host.SetSelectionRect(_selectionRect);

        // Select intersecting objects
        Rect selectionRectGrid = _host.CoordinateHelper.ScreenToGrid(_selectionRect, _host.GridSize);
        Rect viewport = _host.Viewport;
        selectionRectGrid.X += viewport.X;
        selectionRectGrid.Y += viewport.Y;

        _host.AddSelectedObjects(_host.PlacedObjects.GetItemsIntersecting(selectionRectGrid),
                                ShouldAffectObjectsWithIdentifier());
        _host.RecalculateSelectionContainsNotIgnoredObject();
        _host.UpdateStatistics(includeBuildings: true);
    }

    private void HandleDragSelectionMove()
    {
        if (_oldObjectPositions.Count == 0)
        {
            _oldObjectPositions.AddRange(_host.SelectedObjects.Select(obj => (obj, obj.GridRect)));
        }

        // Calculate movement in grid cells, using integer truncation (not rounding) to match v1 behavior
        double deltaX = _mousePosition.X - _mouseDragStart.X;
        double deltaY = _mousePosition.Y - _mouseDragStart.Y;
        int dx = (int)_host.CoordinateHelper.ScreenToGrid(deltaX, _host.GridSize);
        int dy = (int)_host.CoordinateHelper.ScreenToGrid(deltaY, _host.GridSize);

        if (dx == 0 && dy == 0)
        {
            return; // No movement
        }

        // Check for collisions
        Rect offsetCollisionRect = _collisionRect;
        offsetCollisionRect.Offset(dx, dy);

        List<LayoutObject> unselectedObjects = [.. _host.PlacedObjects.GetItemsIntersecting(offsetCollisionRect).Where(_ => !_host.SelectedObjects.Contains(_))];

        bool collisionsExist = false;
        foreach (LayoutObject curLayoutObject in _host.SelectedObjects)
        {
            Point originalPosition = curLayoutObject.Position;
            curLayoutObject.Position = new Point(curLayoutObject.Position.X + dx, curLayoutObject.Position.Y + dy);

            bool collides = unselectedObjects.Any(_ => _host.ObjectIntersectionExists(curLayoutObject, _));
            curLayoutObject.Position = originalPosition;

            if (collides)
            {
                collisionsExist = true;
                break;
            }
        }

        // If no collisions, permanently move all selected objects
        if (!collisionsExist)
        {
            foreach (LayoutObject curLayoutObject in _host.SelectedObjects)
            {
                curLayoutObject.Position = new Point(curLayoutObject.Position.X + dx, curLayoutObject.Position.Y + dy);
            }

            _mouseDragStart.X += _host.CoordinateHelper.GridToScreen(dx, _host.GridSize);
            _mouseDragStart.Y += _host.CoordinateHelper.GridToScreen(dy, _host.GridSize);

            _collisionRect.X += dx;
            _collisionRect.Y += dy;

            _host.UpdateStatistics(includeBuildings: false);

            Rect oldLayoutBounds = _host.Viewport;
            _host.InvalidateBounds();
            if (oldLayoutBounds != _host.Viewport)
            {
                _host.InvalidateScroll();
            }
        }

        _host.InvalidateRender();
    }

    private void HandleStandardLeftClick()
    {
        // Clear selection if no modifier key is pressed
        if (!(IsControlPressed() || IsShiftPressed()))
        {
            _host.SelectedObjects.Clear();
        }
        LayoutObject obj = _host.GetObjectAt(_mousePosition);
        if (obj != null)
        {
            // Select object immediately on click
            if (!_host.SelectedObjects.Contains(obj))
            {
                _host.AddSelectedObject(obj);
            }
            _host.RecalculateSelectionContainsNotIgnoredObject();
        }
        _collisionRect = _host.ComputeBoundingRect(_host.SelectedObjects);
        _host.UpdateStatistics(includeBuildings: true);
        _host.CurrentMode = MouseMode.Standard;
        // Do not remove ignored objects here
    }

    private void FinalizeDragSelection()
    {
        _host.UndoManager.RegisterOperation(new MoveObjectsOperation<LayoutObject>()
        {
            ObjectPropertyValues = _oldObjectPositions.Select(pair => (pair.Item, pair.OldGridRect, pair.Item.Bounds)).ToList(),
            QuadTree = _host.PlacedObjects
        });
        CommandManager.InvalidateRequerySuggested();

        _host.ReindexMovedObjects();
        _oldObjectPositions.Clear();

        // Bring moved objects to front
        _host.BringObjectsToFront(_host.SelectedObjects);

        if (_host.SelectedObjects.Count == 1)
        {
            _host.SelectedObjects.Clear();
        }
        _host.CurrentMode = MouseMode.Standard;
    }

    private void HandleRightClick()
    {
        switch (_host.CurrentMode)
        {
            case MouseMode.PlaceObjects:
            case MouseMode.DeleteObject:
            case MouseMode.Standard:
                if (_host.CurrentObjects.Count != 0)
                {
                    _host.CurrentObjects.Clear();
                }
                _host.CurrentMode = MouseMode.Standard;
                break;

            case MouseMode.DragSelection:
                _host.UndoManager.RegisterOperation(new MoveObjectsOperation<LayoutObject>()
                {
                    ObjectPropertyValues = _oldObjectPositions.Select(pair => (pair.Item, pair.OldGridRect, pair.Item.Bounds)).ToList(),
                    QuadTree = _host.PlacedObjects
                });

                _host.ReindexMovedObjects();
                _oldObjectPositions.Clear();
                _host.SelectedObjects.Clear();

                if (_host.CurrentObjects.Count != 0)
                {
                    _host.CurrentObjects.Clear();
                }
                _host.CurrentMode = MouseMode.Standard;
                break;

            case MouseMode.SelectSameIdentifier:
                _host.CurrentMode = MouseMode.Standard;
                break;
        }
    }

    private static bool IsControlPressed()
    {
        return (Keyboard.Modifiers & ModifierKeys.Control) != 0;
    }

    private static bool IsShiftPressed()
    {
        return (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
    }

    private static bool ShouldAffectObjectsWithIdentifier()
    {
        return IsShiftPressed();
    }

    private static bool IsIgnoredObject(LayoutObject obj)
    {
        return obj.WrappedAnnoObject?.Template == "OrnamentalBuilding" ||
        obj.WrappedAnnoObject?.Road is not null;
    }

    #endregion
}
