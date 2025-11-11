using AnnoDesigner.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace AnnoDesigner.CanvasV2;

/// <summary>
/// ViewModel for the floating toolbox that provides quick access to canvas tools.
/// Manages tool selection state and provides metadata for each tool.
/// </summary>
public class FloatingToolboxViewModel : INotifyPropertyChanged
{
    private readonly Dictionary<CanvasTool, ToolMetadata> _toolMetadata;

    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<CanvasTool> SelectedToolChanged;

    /// <summary>
    /// Gets or sets the currently selected tool.
    /// </summary>
    public CanvasTool SelectedTool
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                RaisePropertyChanged();
                SelectedToolChanged?.Invoke(this, value);

                // Refresh all tool selection states
                foreach (ToolMetadata tool in _toolMetadata.Values)
                {
                    tool.RefreshIsSelected();
                }
            }
        }
    } = CanvasTool.Select;

    /// <summary>
    /// Gets the collection of available tools with their metadata.
    /// </summary>
    public IEnumerable<ToolMetadata> Tools => _toolMetadata.Values;

    /// <summary>
    /// Command to select a specific tool.
    /// </summary>
    public ICommand SelectToolCommand { get; }

    public FloatingToolboxViewModel()
    {
        SelectToolCommand = new RelayCommand(
            parameter => SelectedTool = (CanvasTool)parameter,
            parameter => parameter is CanvasTool tool && IsToolEnabled(tool)
        );

        // Initialize tool metadata
        _toolMetadata = new Dictionary<CanvasTool, ToolMetadata>
        {
            [CanvasTool.Select] = new ToolMetadata(
                this,
                CanvasTool.Select,
                "Select",
                "Select and manipulate objects",
                "\uE8B3", // Segoe MDL2: Pointer
                "Esc",
                isEnabled: true
            ),
            [CanvasTool.Draw] = new ToolMetadata(
                this,
                CanvasTool.Draw,
                "Draw",
                "Place objects continuously",
                "\uE70F", // Segoe MDL2: Edit/Pencil
                "D",
                isEnabled: true
            ),
            [CanvasTool.Move] = new ToolMetadata(
                this,
                CanvasTool.Move,
                "Move",
                "Pan the canvas view",
                "\uE7C2", // Segoe MDL2: HandGrab
                "Space",
                isEnabled: true
            ),
            [CanvasTool.Duplicate] = new ToolMetadata(
                this,
                CanvasTool.Duplicate,
                "Duplicate",
                "Duplicate objects quickly",
                "\uE8C8", // Segoe MDL2: Copy
                "Ctrl+D",
                isEnabled: true
            ),
            [CanvasTool.Delete] = new ToolMetadata(
                this,
                CanvasTool.Delete,
                "Delete",
                "Delete objects by clicking",
                "\uE74D", // Segoe MDL2: Delete
                "Del",
                isEnabled: true
            ),
            [CanvasTool.Measure] = new ToolMetadata(
                this,
                CanvasTool.Measure,
                "Measure",
                "Measure distances and areas",
                "\uE7C8", // Segoe MDL2: Ruler
                "M",
                isEnabled: false // Future implementation
            ),
            [CanvasTool.Eyedropper] = new ToolMetadata(
                this,
                CanvasTool.Eyedropper,
                "Eyedropper",
                "Copy object properties",
                "\uE7C3", // Segoe MDL2: Eyedropper
                "I",
                isEnabled: false // Future implementation
            )
        };
    }

    /// <summary>
    /// Gets the metadata for a specific tool.
    /// </summary>
    public ToolMetadata GetToolMetadata(CanvasTool tool)
    {
        return _toolMetadata.TryGetValue(tool, out ToolMetadata metadata) ? metadata : null;
    }

    /// <summary>
    /// Checks if a tool is currently enabled.
    /// </summary>
    private bool IsToolEnabled(CanvasTool tool)
    {
        return _toolMetadata.TryGetValue(tool, out ToolMetadata metadata) && metadata.IsEnabled;
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Metadata for a single tool in the toolbox.
    /// </summary>
    public class ToolMetadata : INotifyPropertyChanged
    {
        private readonly FloatingToolboxViewModel _parent;

        public event PropertyChangedEventHandler PropertyChanged;

        public CanvasTool Tool { get; }
        public string Name { get; }
        public string Description { get; }
        public string Icon { get; }
        public string Hotkey { get; }
        public bool IsEnabled { get; }

        /// <summary>
        /// Gets whether this tool is currently selected.
        /// </summary>
        public bool IsSelected => _parent.SelectedTool == Tool;

        /// <summary>
        /// Gets the tooltip text combining description and hotkey.
        /// </summary>
        public string Tooltip => $"{Description} ({Hotkey})";

        public ToolMetadata(
            FloatingToolboxViewModel parent,
            CanvasTool tool,
            string name,
            string description,
            string icon,
            string hotkey,
            bool isEnabled)
        {
            _parent = parent;
            Tool = tool;
            Name = name;
            Description = description;
            Icon = icon;
            Hotkey = hotkey;
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Notifies that the IsSelected property may have changed.
        /// Called when the parent's SelectedTool changes.
        /// </summary>
        internal void RefreshIsSelected()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }
}
