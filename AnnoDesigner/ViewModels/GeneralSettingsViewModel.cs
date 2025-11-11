using AnnoDesigner.Core.Helper;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Helper;
using AnnoDesigner.Models;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace AnnoDesigner.ViewModels;

public class GeneralSettingsViewModel : Notify
{
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    private readonly IAppSettings _appSettings;
    private readonly ICommons _commons;
    private readonly IRecentFilesHelper _recentFilesHelper;
    private Color? _selectedCustomGridLineColor;
    private Color? _selectedCustomObjectBorderLineColor;

    public GeneralSettingsViewModel(IAppSettings appSettingsToUse, ICommons commonsToUse, IRecentFilesHelper recentFilesHelperToUse)
    {
        _appSettings = appSettingsToUse;
        _commons = commonsToUse;
        _commons.SelectedLanguageChanged += Commons_SelectedLanguageChanged;
        _recentFilesHelper = recentFilesHelperToUse;

        UseZoomToPoint = _appSettings.UseZoomToPoint;
        ZoomSensitivityPercentage = _appSettings.ZoomSensitivityPercentage;
        HideInfluenceOnSelection = _appSettings.HideInfluenceOnSelection;
        ShowScrollbars = _appSettings.ShowScrollbars;
        InvertPanningDirection = _appSettings.InvertPanningDirection;
        InvertScrollingDirection = _appSettings.InvertScrollingDirection;
        MaxRecentFiles = _appSettings.MaxRecentFiles;

        ResetZoomSensitivityCommand = new RelayCommand(ExecuteResetZoomSensitivity, CanExecuteResetZoomSensitivity);
        ResetMaxRecentFilesCommand = new RelayCommand(ExecuteResetMaxRecentFiles, CanExecuteResetMaxRecentFiles);
        ClearRecentFilesCommand = new RelayCommand(ExecuteClearRecentFiles, CanExecuteClearRecentFiles);

        GridLineColors = [];
        RefreshGridLineColors();
        UserDefinedColor savedGridLineColor = SerializationHelper.LoadFromJsonString<UserDefinedColor>(_appSettings.ColorGridLines);
        if (savedGridLineColor is null)
        {
            SelectedGridLineColor = GridLineColors.First();
            SelectedCustomGridLineColor = SelectedGridLineColor.Color;
        }
        else
        {
            SelectedGridLineColor = GridLineColors.SingleOrDefault(x => x.Type == savedGridLineColor.Type);
            SelectedCustomGridLineColor = savedGridLineColor.Color;
        }

        ObjectBorderLineColors = [];
        RefreshObjectBorderLineColors();
        UserDefinedColor savedObjectBorderLineColor = SerializationHelper.LoadFromJsonString<UserDefinedColor>(_appSettings.ColorObjectBorderLines);
        if (savedObjectBorderLineColor is null)
        {
            SelectedObjectBorderLineColor = ObjectBorderLineColors.First();
            SelectedCustomObjectBorderLineColor = SelectedObjectBorderLineColor.Color;
        }
        else
        {
            SelectedObjectBorderLineColor = ObjectBorderLineColors.SingleOrDefault(x => x.Type == savedObjectBorderLineColor.Type);
            SelectedCustomObjectBorderLineColor = savedObjectBorderLineColor.Color;
        }
    }

    private void Commons_SelectedLanguageChanged(object sender, EventArgs e)
    {
        UserDefinedColorType selectedGridLineColorType = SelectedGridLineColor.Type;
        GridLineColors.Clear();
        RefreshGridLineColors();
        SelectedGridLineColor = GridLineColors.SingleOrDefault(x => x.Type == selectedGridLineColorType);

        UserDefinedColorType selectedObjectBorderLineColorType = SelectedObjectBorderLineColor.Type;
        ObjectBorderLineColors.Clear();
        RefreshObjectBorderLineColors();
        SelectedObjectBorderLineColor = ObjectBorderLineColors.SingleOrDefault(x => x.Type == selectedObjectBorderLineColorType);
    }

    #region Color for grid lines

    private void RefreshGridLineColors()
    {
        foreach (UserDefinedColorType curColorType in Enum.GetValues<UserDefinedColorType>())
        {
            GridLineColors.Add(new UserDefinedColor
            {
                Type = curColorType
            });
        }
    }

    public ObservableCollection<UserDefinedColor> GridLineColors
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public UserDefinedColor SelectedGridLineColor
    {
        get;
        set
        {
            if (UpdateProperty(ref field, value))
            {
                if (value != null)
                {
                    UpdateGridLineColorVisibility();
                    SaveSelectedGridLineColor();
                }
            }
        }
    }

    public Color? SelectedCustomGridLineColor
    {
        get => _selectedCustomGridLineColor;
        set
        {
            if (UpdateProperty(ref _selectedCustomGridLineColor, value))
            {
                if (value != null)
                {
                    SelectedGridLineColor.Color = value.Value;
                    SaveSelectedGridLineColor();
                }
            }
        }
    }

    public bool IsGridLineColorPickerVisible
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    private void UpdateGridLineColorVisibility()
    {
        if (SelectedGridLineColor is null)
        {
            IsGridLineColorPickerVisible = false;
            return;
        }

        IsGridLineColorPickerVisible = SelectedGridLineColor.Type switch
        {
            UserDefinedColorType.Custom => true,
            _ => false,
        };
    }

    private void SaveSelectedGridLineColor()
    {
        switch (SelectedGridLineColor.Type)
        {
            case UserDefinedColorType.Default:
                SelectedGridLineColor.Color = Colors.Black;
                break;
            case UserDefinedColorType.Light:
                SelectedGridLineColor.Color = Colors.LightGray;
                break;
            case UserDefinedColorType.Custom:
                SelectedGridLineColor.Color = SelectedCustomGridLineColor ?? Colors.Black;
                break;
            default:
                break;
        }

        string json = SerializationHelper.SaveToJsonString(SelectedGridLineColor);
        _appSettings.ColorGridLines = json;
        _appSettings.Save();
    }

    #endregion

    #region Color for object border lines

    private void RefreshObjectBorderLineColors()
    {
        foreach (UserDefinedColorType curColorType in Enum.GetValues<UserDefinedColorType>())
        {
            ObjectBorderLineColors.Add(new UserDefinedColor
            {
                Type = curColorType
            });
        }
    }

    public ObservableCollection<UserDefinedColor> ObjectBorderLineColors
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public UserDefinedColor SelectedObjectBorderLineColor
    {
        get;
        set
        {
            if (UpdateProperty(ref field, value))
            {
                if (value != null)
                {
                    UpdateObjectBorderLineVisibility();
                    SaveSelectedObjectBorderLine();
                }
            }
        }
    }

    public Color? SelectedCustomObjectBorderLineColor
    {
        get => _selectedCustomObjectBorderLineColor;
        set
        {
            if (UpdateProperty(ref _selectedCustomObjectBorderLineColor, value))
            {
                if (value != null)
                {
                    SelectedObjectBorderLineColor.Color = value.Value;
                    SaveSelectedObjectBorderLine();
                }
            }
        }
    }

    public bool IsObjectBorderLineColorPickerVisible
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    private void UpdateObjectBorderLineVisibility()
    {
        if (SelectedObjectBorderLineColor is null)
        {
            IsObjectBorderLineColorPickerVisible = false;
            return;
        }

        IsObjectBorderLineColorPickerVisible = SelectedObjectBorderLineColor.Type switch
        {
            UserDefinedColorType.Custom => true,
            _ => false,
        };
    }

    private void SaveSelectedObjectBorderLine()
    {
        switch (SelectedObjectBorderLineColor.Type)
        {
            case UserDefinedColorType.Default:
                SelectedObjectBorderLineColor.Color = Colors.Black;
                break;
            case UserDefinedColorType.Light:
                SelectedObjectBorderLineColor.Color = Colors.LightGray;
                break;
            case UserDefinedColorType.Custom:
                SelectedObjectBorderLineColor.Color = SelectedCustomObjectBorderLineColor ?? Colors.Black;
                break;
            default:
                break;
        }

        string json = SerializationHelper.SaveToJsonString(SelectedObjectBorderLineColor);
        _appSettings.ColorObjectBorderLines = json;
        _appSettings.Save();
    }

    #endregion

    public bool HideInfluenceOnSelection
    {
        get;
        set
        {
            if (UpdateProperty(ref field, value))
            {
                _appSettings.HideInfluenceOnSelection = value;
                _appSettings.Save();
            }
        }
    }

    public double ZoomSensitivityPercentage
    {
        get;
        set
        {
            // Clamp value to valid range
            double clampedValue = Math.Max(1, Math.Min(value, Constants.ZoomSensitivitySliderMaximum));
            if (UpdateProperty(ref field, clampedValue))
            {
                _appSettings.ZoomSensitivityPercentage = clampedValue;
                _appSettings.Save();
            }
        }
    }

    public bool UseZoomToPoint
    {
        get;
        set
        {
            if (UpdateProperty(ref field, value))
            {
                _appSettings.UseZoomToPoint = value;
                _appSettings.Save();
            }
        }
    }

    public bool InvertScrollingDirection
    {
        get;
        set
        {
            if (UpdateProperty(ref field, value))
            {
                _appSettings.InvertScrollingDirection = value;
                _appSettings.Save();
            }
        }
    }

    public bool InvertPanningDirection
    {
        get;
        set
        {
            if (UpdateProperty(ref field, value))
            {
                _appSettings.InvertPanningDirection = value;
                _appSettings.Save();
            }
        }
    }

    public bool ShowScrollbars
    {
        get;
        set
        {
            if (UpdateProperty(ref field, value))
            {
                _appSettings.ShowScrollbars = value;
                _appSettings.Save();
            }
        }
    }

    public bool IncludeRoadsInStatisticCalculation
    {
        get;
        set
        {
            if (UpdateProperty(ref field, value))
            {
                _appSettings.IncludeRoadsInStatisticCalculation = value;
                _appSettings.Save();
            }
        }
    }

    public int MaxRecentFiles
    {
        get;
        set
        {
            // Clamp value to valid range
            int clampedValue = Math.Max(1, Math.Min(value, 100));
            if (UpdateProperty(ref field, clampedValue))
            {
                _appSettings.MaxRecentFiles = clampedValue;
                _appSettings.Save();

                _recentFilesHelper.MaximumItemCount = clampedValue;
            }
        }
    }

    public ICommand ResetZoomSensitivityCommand { get; private set; }

    private void ExecuteResetZoomSensitivity(object param)
    {
        ZoomSensitivityPercentage = Constants.ZoomSensitivityPercentageDefault;
    }

    private bool CanExecuteResetZoomSensitivity(object param)
    {
        return ZoomSensitivityPercentage != Constants.ZoomSensitivityPercentageDefault;
    }

    public ICommand ResetMaxRecentFilesCommand { get; private set; }

    private void ExecuteResetMaxRecentFiles(object param)
    {
        MaxRecentFiles = Constants.MaxRecentFiles;
    }

    private bool CanExecuteResetMaxRecentFiles(object param)
    {
        return MaxRecentFiles != Constants.MaxRecentFiles;
    }

    public ICommand ClearRecentFilesCommand { get; private set; }

    private void ExecuteClearRecentFiles(object param)
    {
        _recentFilesHelper.ClearRecentFiles();
    }

    private bool CanExecuteClearRecentFiles(object param)
    {
        return _recentFilesHelper.RecentFiles.Count > 0;
    }
}
