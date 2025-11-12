namespace AnnoDesigner.CanvasV2.Input;

/// <summary>
/// Mouse interaction modes for the canvas.
/// migrated from AnnoCanvas.xaml.cs — MouseMode enum
/// </summary>
public enum MouseMode
{
    /// <summary>
    /// Standard mode - no drag operation in progress
    /// </summary>
    Standard,

    /// <summary>
    /// User has started dragging a selection rectangle
    /// </summary>
    SelectionRectStart,

    /// <summary>
    /// Dragging a selection rectangle
    /// </summary>
    SelectionRect,

    /// <summary>
    /// User has started dragging selected objects
    /// </summary>
    DragSelectionStart,

    /// <summary>
    /// User has started dragging a single object
    /// </summary>
    DragSingleStart,

    /// <summary>
    /// Dragging selected objects
    /// </summary>
    DragSelection,

    /// <summary>
    /// User has started panning the entire canvas (two-button drag)
    /// </summary>
    DragAllStart,

    /// <summary>
    /// Panning the entire canvas
    /// </summary>
    DragAll,

    /// <summary>
    /// Placing objects from CurrentObjects
    /// </summary>
    PlaceObjects,

    /// <summary>
    /// Delete object under cursor mode
    /// </summary>
    DeleteObject,

    /// <summary>
    /// Select all objects with the same identifier
    /// </summary>
    SelectSameIdentifier
}
