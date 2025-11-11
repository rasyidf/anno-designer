using AnnoDesigner.Core.Helper;
using System.Windows;

namespace AnnoDesigner.Helper;

/// <summary>
/// Encapsulates properties about the current viewport, and provides methods to convert to and from origin and viewport coordinates.
/// </summary>
public class Viewport
{
    /// <summary>
    /// The top offset of the viewport.
    /// </summary>
    public double Top
    {
        get;
        set
        {
            field = value;
            VerticalAlignmentValue = -MathHelper.FractionalValue(field);
        }
    }

    /// <summary>
    /// The Left offset of the viewport.
    /// </summary>
    public double Left
    {
        get;
        set
        {
            field = value;
            HorizontalAlignmentValue = -MathHelper.FractionalValue(field);
        }
    }

    /// <summary>
    /// Fractional value between -1 and 1 used to align the edge of the viewport to an integer value.
    /// </summary>
    public double HorizontalAlignmentValue { get; private set; }

    /// <summary>
    /// Fractional value between -1 and 1 used to align the edge of the viewport to an integer value.
    /// </summary>
    public double VerticalAlignmentValue { get; private set; }

    /// <summary>
    /// The Width of the viewport.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// The Height of the viewport.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Retrieve a <see cref="Rect"/> that represents the current viewport.
    /// </summary>
    public Rect Absolute => new(Left, Top, Width, Height);

    /// <summary>
    /// Converts a <see cref="Rect"/> that is relative to the origin to one that is relative to the viewport.
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public Rect OriginToViewport(Rect rect)
    {
        return new Rect(rect.X + Left, rect.Y + Top, rect.Width, rect.Height);
    }

    /// <summary>
    /// Converts a <see cref="Point"/> that is relative to the origin to one that is relative to the viewport.
    /// </summary>
    /// <returns></returns>
    public Point OriginToViewport(Point point)
    {
        return new Point(point.X + Left, point.Y + Top);
    }
}
