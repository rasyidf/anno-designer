using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using AnnoDesigner.Core.Extensions;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using AnnoDesigner.CanvasV2.Input;
using AnnoDesigner.Helper;

namespace AnnoDesigner.CanvasV2.Rendering;

/// <summary>
/// Pure renderer for the canvas. Accepts an immutable RenderState and renders to DrawingContext.
/// migrated from AnnoCanvas.xaml.cs — Phase 3: Renderer Extraction
/// </summary>
public sealed class LayoutRenderer
{
    private static readonly Typeface TYPEFACE = new("Segoe UI");

    private readonly RendererCaches _caches;
    private readonly Pen _borderPen;

    public LayoutRenderer()
    {
        _caches = new RendererCaches();
        _borderPen = new Pen(Brushes.Black, 1);
        _borderPen.Freeze();
    }

    /// <summary>
    /// Main render entry point. Renders the entire canvas based on the provided state snapshot.
    /// </summary>
    public void Render(DrawingContext dc, RenderState state)
    {
        if (dc is null) return;
        
        int grid = state.GridSize;
        double widthPx = state.Viewport.Width * grid;
        double heightPx = state.Viewport.Height * grid;
        
        if (double.IsNaN(widthPx) || widthPx <= 0 || double.IsNaN(heightPx) || heightPx <= 0)
        {
            return;
        }

        // Background
        dc.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, widthPx, heightPx));

        // Grid (before viewport translate)
        if (state.RenderGrid)
        {
            RenderGrid(dc, state, widthPx, heightPx, grid);
        }

        // Push viewport transform
        if (state.Translate is not null)
        {
            dc.PushTransform(state.Translate);
        }

        if (state.GuidelineSet is not null)
        {
            dc.PushGuidelineSet(state.GuidelineSet);
        }

        // Draw placed objects
        RenderPlacedObjects(dc, state, grid);

        // Draw current objects (being placed)
        RenderCurrentObjects(dc, state, grid);

        // Draw selection highlight
        if (state.SelectedObjects.Count > 0)
        {
            RenderObjectSelection(dc, state);
        }

        // Draw influences if enabled
        if (state.RenderInfluences && (state.SelectedObjects.Count > 0 || state.CurrentObjects.Count > 0))
        {
            RenderObjectInfluenceRange(dc, state);
        }

        if (state.GuidelineSet is not null)
        {
            dc.Pop();
        }

        if (state.Translate is not null)
        {
            dc.Pop();
        }

        // Selection rectangle (screen space, drawn after popping transforms)
        if (!state.SelectionRect.IsEmpty && state.CurrentMode == Input.MouseMode.SelectionRect)
        {
            dc.DrawRectangle(_caches.SelectionFill, _caches.SelectionPen, state.SelectionRect);
        }
    }

    #region Private Rendering Methods

    private void RenderGrid(DrawingContext dc, RenderState state, double widthPx, double heightPx, int grid)
    {
        // Compute pixel offset from fractional viewport to keep grid stable while panning
        double fracX = state.Viewport.Left - Math.Floor(state.Viewport.Left);
        double fracY = state.Viewport.Top - Math.Floor(state.Viewport.Top);
        double startX = (1.0 - fracX) % 1.0 * grid;
        double startY = (1.0 - fracY) % 1.0 * grid;
        
        if (double.IsNaN(startX)) startX = 0;
        if (double.IsNaN(startY)) startY = 0;

        Pen gridPen = _caches.GridPen;
        for (double x = startX; x < widthPx; x += grid)
        {
            dc.DrawLine(gridPen, new Point(x, 0), new Point(x, heightPx));
        }
        for (double y = startY; y < heightPx; y += grid)
        {
            dc.DrawLine(gridPen, new Point(0, y), new Point(widthPx, y));
        }
    }

    private void RenderPlacedObjects(DrawingContext dc, RenderState state, int gridSize)
    {
        // Sort objects by Z-index (ascending) so higher Z-index objects render on top
        var sortedObjects = state.ObjectsToDraw.OrderBy(o => o.ZIndex).ToList();
        
        foreach (var lo in sortedObjects)
        {
            RenderSingleObject(dc, state, lo, gridSize);
        }
    }

    private void RenderCurrentObjects(DrawingContext dc, RenderState state, int gridSize)
    {
        // Push opacity for transparency effect (like v1)
        dc.PushOpacity(0.5);
        
        foreach (var lo in state.CurrentObjects)
        {
            RenderSingleObject(dc, state, lo, gridSize);
        }
        
        dc.Pop();
    }

    private void RenderSingleObject(DrawingContext dc, RenderState state, LayoutObject obj, int gridSize)
    {
        Rect objRect = obj.CalculateScreenRect(gridSize);
        Brush brush = obj.RenderBrush;

        // Draw main object rectangle
        dc.DrawRectangle(brush, _borderPen, objRect);

        // Draw harbor blocked area if enabled
        if (state.RenderHarborBlockedArea)
        {
            Rect? blockedRect = obj.CalculateBlockedScreenRect(gridSize);
            if (blockedRect.HasValue)
            {
                dc.DrawRectangle(obj.BlockedAreaBrush, _borderPen, blockedRect.Value);
            }
        }

        // Draw icon
        bool iconRendered = false;
        if (state.RenderIcon && !string.IsNullOrEmpty(obj.WrappedAnnoObject?.Icon))
        {
            iconRendered = TryRenderIcon(dc, state, obj, gridSize);
        }

        // Draw label
        if (state.RenderLabel && !string.IsNullOrEmpty(obj.WrappedAnnoObject?.Label))
        {
            RenderLabel(dc, obj, objRect, iconRendered);
        }
    }

    private static bool TryRenderIcon(DrawingContext dc, RenderState state, LayoutObject obj, int gridSize)
    {
        string? iconName = obj.IconNameWithoutExtension;
        if (string.IsNullOrEmpty(iconName))
        {
            return false;
        }

        // Try to get cached icon
        if (obj.Icon is null && state.Icons.TryGetValue(iconName, out IconImage? iconImage))
        {
            obj.Icon = iconImage;
        }

        if (obj.Icon is not null)
        {
            Rect iconRect = obj.GetIconRect(gridSize);
            dc.DrawImage(obj.Icon.Icon, iconRect);
            return true;
        }

        return false;
    }

    private static void RenderLabel(DrawingContext dc, LayoutObject obj, Rect objRect, bool iconRendered)
    {
        TextAlignment textAlignment = iconRendered ? TextAlignment.Left : TextAlignment.Center;
        FormattedText text = obj.GetFormattedText(
            textAlignment,
            Thread.CurrentThread.CurrentCulture,
            TYPEFACE,
            App.DpiScale.PixelsPerDip,
            objRect.Width,
            objRect.Height
        );

        Point textLocation = objRect.TopLeft;
        if (iconRendered)
        {
            // Place text in top left corner if icon is present
            textLocation.X += 3;
            textLocation.Y += 2;
        }
        else
        {
            // Center text if no icon
            textLocation.Y += (objRect.Height - text.Height) / 2;
        }

        dc.DrawText(text, textLocation);
    }

    private void RenderObjectSelection(DrawingContext dc, RenderState state)
    {
        Pen highlightPen = _caches.HighlightPen;
        foreach (var lo in state.SelectedObjects)
        {
            Rect r = lo.CalculateScreenRect(state.GridSize);
            dc.DrawRectangle(null, highlightPen, r);
        }
    }

    private void RenderObjectInfluenceRange(DrawingContext dc, RenderState state)
    {
        // Combine selected and current objects for influence rendering
        var objectsWithInfluence = state.SelectedObjects
            .Concat(state.CurrentObjects)
            .Where(o => o.WrappedAnnoObject?.Radius >= 0.5 || o.WrappedAnnoObject?.InfluenceRange > 0.5)
            .ToList();

        if (objectsWithInfluence.Count == 0)
        {
            return;
        }

        // Render influenced buildings (highlighted) if RenderTrueInfluenceRange is enabled
        Moved2DArray<AnnoObject>? gridDictionary = null;
        List<AnnoObject>? placedAnnoObjects = null;

        if (state.RenderTrueInfluenceRange && state.AllPlacedObjects.Count > 0)
        {
            var allObjects = state.AllPlacedObjects.Concat(objectsWithInfluence).ToHashSet();
            placedAnnoObjects = allObjects.Select(o => o.WrappedAnnoObject).ToList();
            Dictionary<AnnoObject, LayoutObject> placedObjectDictionary = allObjects.ToDictionary(o => o.WrappedAnnoObject);

            void Highlight(AnnoObject objectInRange)
            {
                if (placedObjectDictionary.TryGetValue(objectInRange, out var layoutObj))
                {
                    dc.DrawRectangle(_caches.InfluencedBrush, _caches.InfluencedPen, layoutObj.CalculateScreenRect(state.GridSize));
                }
            }

            gridDictionary = RoadSearchHelper.PrepareGridDictionary(placedAnnoObjects);
            if (gridDictionary != null)
            {
                _ = RoadSearchHelper.BreadthFirstSearch(
                    placedAnnoObjects,
                    objectsWithInfluence.Select(o => o.WrappedAnnoObject).Where(o => o.InfluenceRange > 0.5),
                    o => (int)o.InfluenceRange + 1, // increase distance to get objects touching even the last road cell
                    gridDictionary,
                    Highlight);
            }
        }

        // Render influence polygons in parallel for performance
        ConcurrentBag<(long index, StreamGeometry geometry)> geometries = new();
        _ = Parallel.ForEach(objectsWithInfluence, (curLayoutObject, _, index) =>
        {
            // Basic radius rendering
            if (curLayoutObject.WrappedAnnoObject.Radius >= 0.5)
            {
                // Radius circles are rendered directly on the main thread below
                // to avoid thread-safety issues with geometries
            }

            // Influence range polygon rendering
            if (curLayoutObject.WrappedAnnoObject.InfluenceRange > 0.5)
            {
                StreamGeometry sg = new();
                // Use EvenOdd fill rule so polygons with holes are rendered correctly when multiple
                // figures (outer boundary + holes) are emitted into the same geometry.
                sg.FillRule = FillRule.EvenOdd;
                using (StreamGeometryContext sgc = sg.Open())
                {
                    if (state.RenderTrueInfluenceRange && gridDictionary != null && placedAnnoObjects != null)
                    {
                        DrawTrueInfluenceRangePolygon(curLayoutObject, sgc, state, gridDictionary, placedAnnoObjects);
                    }
                    else
                    {
                        DrawInfluenceRangePolygon(curLayoutObject, sgc, state);
                    }
                }

                if (sg.CanFreeze)
                {
                    sg.Freeze();
                }
                geometries.Add((index, sg));
            }
        });

        // Draw radius circles (must be on main thread)
        foreach (var obj in objectsWithInfluence.Where(o => o.WrappedAnnoObject.Radius >= 0.5))
        {
            double radius = obj.GetScreenRadius(state.GridSize);
            EllipseGeometry circle = obj.GetInfluenceCircle(state.GridSize, radius);
            dc.DrawGeometry(_caches.LightBrush, _caches.RadiusPen, circle);
        }

        // Draw influence polygons in order
        foreach ((_, StreamGeometry geometry) in geometries.OrderBy(p => p.index))
        {
            dc.DrawGeometry(_caches.LightBrush, _caches.RadiusPen, geometry);
        }
    }

    /// <summary>
    /// Draws the true influence range polygon using road connectivity via RoadSearchHelper.
    /// migrated from AnnoCanvas.xaml.cs — DrawTrueInfluenceRangePolygon
    /// </summary>
    private static void DrawTrueInfluenceRangePolygon(
        LayoutObject curLayoutObject,
        StreamGeometryContext sgc,
        RenderState state,
        Moved2DArray<AnnoObject> gridDictionary,
        List<AnnoObject> placedAnnoObjects)
    {
        bool stroked = true;
        bool smoothJoin = true;
        bool geometryFill = true;
        bool geometryStroke = true;

        AnnoObject[] startObjects = new[] { curLayoutObject.WrappedAnnoObject };

        bool[][] cellsInInfluenceRange = RoadSearchHelper.BreadthFirstSearch(
            placedAnnoObjects,
            startObjects,
            o => (int)o.InfluenceRange,
            gridDictionary);

        if (cellsInInfluenceRange == null || cellsInInfluenceRange.Length == 0)
        {
            return;
        }

        // Support polygons with holes: GetBoundaryPointsWithHoles returns the outer boundary
        // first and then holes. We use EvenOdd fill rule so holes will be treated correctly.
        var polygons = PolygonBoundaryFinderHelper.GetBoundaryPointsWithHoles(cellsInInfluenceRange).ToList();
        if (polygons.Count == 0)
        {
            return;
        }

        // For multiple polygons (outer + holes) ensure EvenOdd fill handling
        // Note: caller opens the StreamGeometryContext, so we only emit figures here.
        foreach (var points in polygons)
        {
            if (points == null || points.Count < 1) continue;

            // Convert grid coordinate of first point to screen and begin a new figure
            Point startPoint = state.CoordinateHelper.GridToScreen(
                new Point(points[0].x + gridDictionary.Offset.x, points[0].y + gridDictionary.Offset.y),
                state.GridSize);

            sgc.BeginFigure(startPoint, geometryFill, geometryStroke);

            for (int i = 1; i < points.Count; i++)
            {
                Point screenPoint = state.CoordinateHelper.GridToScreen(
                    new Point(points[i].x + gridDictionary.Offset.x, points[i].y + gridDictionary.Offset.y),
                    state.GridSize);
                sgc.LineTo(screenPoint, stroked, smoothJoin);
            }
        }
    }

    /// <summary>
    /// Draws the basic influence range polygon (octagon) without road connectivity.
    /// migrated from AnnoCanvas.xaml.cs — DrawInfluenceRangePolygon
    /// </summary>
    private static void DrawInfluenceRangePolygon(LayoutObject curLayoutObject, StreamGeometryContext sgc, RenderState state)
    {
        // Octagon drawing logic migrated from v1
        Point topLeftCorner = curLayoutObject.Position;
        Point topRightCorner = new(curLayoutObject.Position.X + curLayoutObject.Size.Width, curLayoutObject.Position.Y);
        Point bottomLeftCorner = new(curLayoutObject.Position.X, curLayoutObject.Position.Y + curLayoutObject.Size.Height);
        Point bottomRightCorner = new(curLayoutObject.Position.X + curLayoutObject.Size.Width, curLayoutObject.Position.Y + curLayoutObject.Size.Height);

        double influenceRange = curLayoutObject.WrappedAnnoObject.InfluenceRange;

        Point startPoint = new(topLeftCorner.X, topLeftCorner.Y - influenceRange);
        bool stroked = true;
        bool smoothJoin = true;
        bool geometryFill = true;
        bool geometryStroke = true;

        sgc.BeginFigure(state.CoordinateHelper.GridToScreen(startPoint, state.GridSize), geometryFill, geometryStroke);

        // Draw in width of object
        sgc.LineTo(state.CoordinateHelper.GridToScreen(new Point(topRightCorner.X, startPoint.Y), state.GridSize), stroked, smoothJoin);

        // Draw quadrant 2 (top-right)
        startPoint = new Point(topRightCorner.X, topRightCorner.Y - influenceRange);
        Point endPoint = new(topRightCorner.X + influenceRange, topRightCorner.Y);

        Point currentPoint = new(startPoint.X, startPoint.Y);
        while (endPoint != currentPoint)
        {
            currentPoint = new Point(currentPoint.X, currentPoint.Y + 1);
            sgc.LineTo(state.CoordinateHelper.GridToScreen(currentPoint, state.GridSize), stroked, smoothJoin);
            currentPoint = new Point(currentPoint.X + 1, currentPoint.Y);
            sgc.LineTo(state.CoordinateHelper.GridToScreen(currentPoint, state.GridSize), stroked, smoothJoin);
        }

        // Draw in height of object
        startPoint = endPoint;
        sgc.LineTo(state.CoordinateHelper.GridToScreen(new Point(startPoint.X, bottomRightCorner.Y), state.GridSize), stroked, smoothJoin);

        // Draw quadrant 3 (bottom-right)
        startPoint = new Point(startPoint.X, bottomRightCorner.Y);
        endPoint = new Point(bottomRightCorner.X, bottomRightCorner.Y + influenceRange);

        currentPoint = new Point(startPoint.X, startPoint.Y);
        while (endPoint != currentPoint)
        {
            currentPoint = new Point(currentPoint.X - 1, currentPoint.Y);
            sgc.LineTo(state.CoordinateHelper.GridToScreen(currentPoint, state.GridSize), stroked, smoothJoin);
            currentPoint = new Point(currentPoint.X, currentPoint.Y + 1);
            sgc.LineTo(state.CoordinateHelper.GridToScreen(currentPoint, state.GridSize), stroked, smoothJoin);
        }

        // Draw in width of object
        startPoint = endPoint;
        sgc.LineTo(state.CoordinateHelper.GridToScreen(new Point(bottomLeftCorner.X, startPoint.Y), state.GridSize), stroked, smoothJoin);

        // Draw quadrant 4 (bottom-left)
        startPoint = new Point(bottomLeftCorner.X, startPoint.Y);
        endPoint = new Point(bottomLeftCorner.X - influenceRange, bottomRightCorner.Y);

        currentPoint = new Point(startPoint.X, startPoint.Y);
        while (endPoint != currentPoint)
        {
            currentPoint = new Point(currentPoint.X, currentPoint.Y - 1);
            sgc.LineTo(state.CoordinateHelper.GridToScreen(currentPoint, state.GridSize), stroked, smoothJoin);
            currentPoint = new Point(currentPoint.X - 1, currentPoint.Y);
            sgc.LineTo(state.CoordinateHelper.GridToScreen(currentPoint, state.GridSize), stroked, smoothJoin);
        }

        // Draw in height of object
        startPoint = endPoint;
        sgc.LineTo(state.CoordinateHelper.GridToScreen(new Point(startPoint.X, topLeftCorner.Y), state.GridSize), stroked, smoothJoin);

        // Draw quadrant 1 (top-left)
        startPoint = new Point(startPoint.X, topLeftCorner.Y);
        endPoint = new Point(topLeftCorner.X, topLeftCorner.Y - influenceRange);

        currentPoint = new Point(startPoint.X, startPoint.Y);
        while (endPoint != currentPoint)
        {
            currentPoint = new Point(currentPoint.X + 1, currentPoint.Y);
            sgc.LineTo(state.CoordinateHelper.GridToScreen(currentPoint, state.GridSize), stroked, smoothJoin);
            currentPoint = new Point(currentPoint.X, currentPoint.Y - 1);
            sgc.LineTo(state.CoordinateHelper.GridToScreen(currentPoint, state.GridSize), stroked, smoothJoin);
        }
    }

    /// <summary>
    /// Invalidates renderer caches. Call when GridSize or visual settings change.
    /// </summary>
    public void InvalidateCaches()
    {
        _caches.Invalidate();
    }

    #endregion
}
