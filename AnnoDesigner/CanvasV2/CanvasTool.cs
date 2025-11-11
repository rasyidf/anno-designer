namespace AnnoDesigner.CanvasV2;

/// <summary>
/// Defines the available tools in the floating toolbox for CanvasV2.
/// Each tool provides a specific interaction mode with the canvas.
/// </summary>
public enum CanvasTool
{
    /// <summary>
    /// Standard selection and manipulation mode.
    /// Click to select objects, drag to move or create selection rectangle.
    /// Hotkey: Esc
    /// </summary>
    Select,

    /// <summary>
    /// Continuous placement mode for drawing buildings.
    /// Click to place objects continuously without returning to select mode.
    /// Hotkey: D
    /// </summary>
    Draw,

    /// <summary>
    /// Pan/drag the entire canvas view.
    /// Drag with this tool to move the viewport.
    /// Hotkey: Space (hold) + Drag
    /// </summary>
    Move,

    /// <summary>
    /// Quick duplicate mode.
    /// Click on an object to create a duplicate and immediately place it.
    /// Hotkey: Ctrl+D
    /// </summary>
    Duplicate,

    /// <summary>
    /// Delete objects by clicking on them.
    /// Click on an object to remove it from the canvas.
    /// Hotkey: Del (with selection)
    /// </summary>
    Delete,

    /// <summary>
    /// Measure distances and areas on the canvas.
    /// Click to start measuring, click again to complete.
    /// (Future implementation)
    /// </summary>
    Measure,

    /// <summary>
    /// Copy object properties (color, type, etc.) from existing objects.
    /// Click on an object to copy its properties for placing new objects.
    /// (Future implementation)
    /// </summary>
    Eyedropper
}
