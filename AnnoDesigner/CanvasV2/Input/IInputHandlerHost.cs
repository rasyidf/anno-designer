using AnnoDesigner.Core.DataStructures;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using AnnoDesigner.Undo;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AnnoDesigner.CanvasV2.Input;

/// <summary>
/// Abstraction implemented by the ViewModel so the input handler can be unit tested.
/// migrated from AnnoCanvas.xaml.cs — Phase 2
/// </summary>
public interface IInputHandlerHost
{
    QuadTree<LayoutObject> PlacedObjects { get; }
    HashSet<LayoutObject> SelectedObjects { get; }
    List<LayoutObject> CurrentObjects { get; }
    int GridSize { get; set; }
    MouseMode CurrentMode { get; set; }
    string StatusMessage { get; internal set; }
    Rect Viewport { get; }

    IUndoManager UndoManager { get; }
    ICoordinateHelper CoordinateHelper { get; }
    IAppSettings AppSettings { get; }

    LayoutObject GetObjectAt(Point position);
    void AddSelectedObject(LayoutObject obj, bool includeSameObjects = false);
    void AddSelectedObjects(IEnumerable<LayoutObject> objects, bool includeSameObjects = false);
    void RemoveSelectedObject(LayoutObject obj, bool includeSameObjects = false);
    void RemoveSelectedObjects(IEnumerable<LayoutObject> objects, bool includeSameObjects = false);
    void RemoveSelectedObjects(Predicate<LayoutObject> predicate);
    Rect ComputeBoundingRect(IEnumerable<LayoutObject> objects);
    bool TryPlaceCurrentObjects(bool isContinuousDrawing);
    void MoveCurrentObjectsToMouse(Point mousePosition);
    void ReportMousePosition(Point p);
    void RecalculateSelectionContainsNotIgnoredObject();
    void ReindexMovedObjects();
    bool ObjectIntersectionExists(LayoutObject a, LayoutObject b);

    void InvalidateRender();
    void InvalidateScroll();
    void InvalidateBounds();
    void UpdateStatistics(bool includeBuildings = true);
    void SetSelectionRect(Rect rect);
    void BringObjectsToFront(IEnumerable<LayoutObject> objects);
}