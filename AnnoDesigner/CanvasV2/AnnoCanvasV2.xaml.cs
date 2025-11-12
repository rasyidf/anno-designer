using AnnoDesigner.CanvasV2.Rendering;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace AnnoDesigner.CanvasV2;

public partial class AnnoCanvasV2 : UserControl, IScrollInfo
{
    static AnnoCanvasV2()
    {
        // Register class command bindings so ApplicationCommands wired to the control (e.g. via Menu CommandTarget)
        // are handled by this new V2 control and forwarded to the ViewModel. The control itself is thin and
        // does not perform any file IO; it asks the ViewModel to request the host to do dialogs.
        CommandManager.RegisterClassCommandBinding(typeof(AnnoCanvasV2), new CommandBinding(ApplicationCommands.New, ExecuteCommand));
        CommandManager.RegisterClassCommandBinding(typeof(AnnoCanvasV2), new CommandBinding(ApplicationCommands.Open, ExecuteCommand));
        CommandManager.RegisterClassCommandBinding(typeof(AnnoCanvasV2), new CommandBinding(ApplicationCommands.Save, ExecuteCommand));
        CommandManager.RegisterClassCommandBinding(typeof(AnnoCanvasV2), new CommandBinding(ApplicationCommands.SaveAs, ExecuteCommand));
    }

    private static void ExecuteCommand(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is AnnoCanvasV2 canvas && canvas.DataContext is AnnoCanvasViewModel vm)
        {
            if (e.Command == ApplicationCommands.New)
            {
                vm.RequestNew();
                e.Handled = true;
                return;
            }

            if (e.Command == ApplicationCommands.Open)
            {
                vm.RequestOpen();
                e.Handled = true;
                return;
            }

            if (e.Command == ApplicationCommands.Save)
            {
                _ = vm.Save();
                e.Handled = true;
                return;
            }

            if (e.Command == ApplicationCommands.SaveAs)
            {
                _ = vm.SaveAs();
                e.Handled = true;
                return;
            }
        }
    }
    private readonly LayoutRenderer _renderer = new();
    public AnnoCanvasViewModel ViewModel => (AnnoCanvasViewModel)DataContext;

    public AnnoCanvasV2()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is AnnoCanvasViewModel vm)
        {
            vm.RenderInvalidated += InvalidateVisual;
            vm.ScrollInvalidated += () => ScrollOwner?.InvalidateScrollInfo();

            // Initialize viewport size
            UpdateViewportSize();
        }
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        UpdateViewportSize();
        InvalidateVisual();
    }

    private void UpdateViewportSize()
    {
        if (DataContext is AnnoCanvasViewModel vm)
        {
            // Update viewport size based on actual control size
            int grid = vm.GridSize <= 0 ? 1 : vm.GridSize;
            vm.ViewportWidth = ActualWidth / grid;
            vm.ViewportHeight = ActualHeight / grid;
        }
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
        if (DataContext is not AnnoCanvasViewModel vm)
        {
            return;
        }

        // Ensure viewport is up to date
        UpdateViewportSize();

        RenderState state = vm.CreateRenderState();
        _renderer.Render(drawingContext, state);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);
        ViewModel?.InputHandler.HandleMouseWheel(e, e.GetPosition(this));
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        _ = Focus(); // Ensure keyboard focus for hotkeys
        ViewModel?.InputHandler.HandleMouseDown(e, e.GetPosition(this));
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        ViewModel?.InputHandler.HandleMouseMove(e, e.GetPosition(this));
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);
        ViewModel?.InputHandler.HandleMouseUp(e, e.GetPosition(this));
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        ViewModel?.InputHandler.HandleKeyDown(e);
    }

    private void ContextMenu_Opened(object sender, RoutedEventArgs e)
    {
        // Update mouse position and refresh command states when context menu opens
        Point mousePos = Mouse.GetPosition(this);
        ViewModel?.ReportMousePosition(mousePos);
        CommandManager.InvalidateRequerySuggested();
    }

    #region IScrollInfo pass-through
    public ScrollViewer ScrollOwner { get => ViewModel.ScrollOwner; set => ViewModel.ScrollOwner = value; }
    public bool CanVerticallyScroll { get => ViewModel.CanVerticallyScroll; set => ViewModel.CanVerticallyScroll = value; }
    public bool CanHorizontallyScroll { get => ViewModel.CanHorizontallyScroll; set => ViewModel.CanHorizontallyScroll = value; }
    public double ExtentWidth => ViewModel.ExtentWidth;
    public double ExtentHeight => ViewModel.ExtentHeight;
    public double ViewportWidth => ViewModel.ViewportWidth;
    public double ViewportHeight => ViewModel.ViewportHeight;
    public double HorizontalOffset => ViewModel.HorizontalOffset;
    public double VerticalOffset => ViewModel.VerticalOffset;
    public void LineUp()
    {
        ViewModel.LineUp();
    }

    public void LineDown()
    {
        ViewModel.LineDown();
    }

    public void LineLeft()
    {
        ViewModel.LineLeft();
    }

    public void LineRight()
    {
        ViewModel.LineRight();
    }

    public void PageUp()
    {
        ViewModel.PageUp();
    }

    public void PageDown()
    {
        ViewModel.PageDown();
    }

    public void PageLeft()
    {
        ViewModel.PageLeft();
    }

    public void PageRight()
    {
        ViewModel.PageRight();
    }

    public void MouseWheelUp()
    {
        ViewModel.MouseWheelUp();
    }

    public void MouseWheelDown()
    {
        ViewModel.MouseWheelDown();
    }

    public void MouseWheelLeft()
    {
        ViewModel.MouseWheelLeft();
    }

    public void MouseWheelRight()
    {
        ViewModel.MouseWheelRight();
    }

    public void SetHorizontalOffset(double offset)
    {
        ViewModel.SetHorizontalOffset(offset);
    }

    public void SetVerticalOffset(double offset)
    {
        ViewModel.SetVerticalOffset(offset);
    }

    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
        return ViewModel.MakeVisible(visual, rectangle);
    }
    #endregion
}
