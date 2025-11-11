using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using static AnnoDesigner.Core.CoreConstants;

namespace AnnoDesigner.ViewModels;

public class PresetsTreeSearchViewModel : Notify
{
    private string _searchText;
    private bool _hasFocus;
    private ObservableCollection<GameVersionFilter> _gameVersionFilters;
    private bool _isUpdatingGameVersionFilter;

    public PresetsTreeSearchViewModel()
    {
        ClearSearchTextCommand = new RelayCommand(ClearSearchText);
        GotFocusCommand = new RelayCommand(GotFocus);
        LostFocusCommand = new RelayCommand(LostFocus);
        GameVersionFilterChangedCommand = new RelayCommand(GameVersionFilterChanged);
        SelectAllGameVersionsCommand = new RelayCommand(SelectAllGameVersions);
        ClearAllGameVersionsCommand = new RelayCommand(ClearAllGameVersions);

        HasFocus = false;
        SearchText = string.Empty;
        GameVersionFilters = [];
        InitGameVersionFilters();
        HookGameVersionFilters();
    }

    private void InitGameVersionFilters()
    {
        int order = 0;
        foreach (GameVersion curGameVersion in Enum.GetValues<GameVersion>())
        {
            if (curGameVersion is GameVersion.Unknown or GameVersion.All)
            {
                continue;
            }

            GameVersionFilters.Add(new GameVersionFilter
            {
                Name = curGameVersion.ToString().Replace("Anno", "Anno "),
                Type = curGameVersion,
                Order = ++order
            });
        }
    }

    public string SearchText
    {
        get => _searchText;
        set => UpdateProperty(ref _searchText, value);
    }

    public bool HasFocus
    {
        get => _hasFocus;
        set => UpdateProperty(ref _hasFocus, value);
    }

    public ObservableCollection<GameVersionFilter> GameVersionFilters
    {
        get => _gameVersionFilters;
        set
        {
            if (UpdateProperty(ref _gameVersionFilters, value))
            {
                HookGameVersionFilters();
            }
        }
    }

    public ObservableCollection<GameVersionFilter> SelectedGameVersionFilters => new(GameVersionFilters.Where(x => x.IsSelected));

    public GameVersion SelectedGameVersions
    {
        set
        {
            try
            {
                _isUpdatingGameVersionFilter = true;

                foreach (GameVersionFilter curFilter in GameVersionFilters)
                {
                    curFilter.IsSelected = value.HasFlag(curFilter.Type);
                }
            }
            finally
            {
                _isUpdatingGameVersionFilter = false;

                OnPropertyChanged(nameof(SelectedGameVersionFilters));
            }
        }
    }

    #region commands

    public ICommand ClearSearchTextCommand { get; private set; }

    //TODO: refactor to use interface ICanUpdateLayout -> currently TextBox does not implement it (create own control?)
    private void ClearSearchText(object param)
    {
        SearchText = string.Empty;

        if (param is ICanUpdateLayout updateable)
        {
            updateable.UpdateLayout();
        }
        else if (param is TextBox textBox)
        {
            //Debug.WriteLine($"+ IsFocused: {textBox.IsFocused} | IsKeyboardFocused: {textBox.IsKeyboardFocused} | IsKeyboardFocusWithin: {textBox.IsKeyboardFocusWithin} | CaretIndex: {textBox.CaretIndex}");

            //SearchText = string.Empty;

            //Debug.WriteLine($"++ IsFocused: {textBox.IsFocused} | IsKeyboardFocused: {textBox.IsKeyboardFocused} | IsKeyboardFocusWithin: {textBox.IsKeyboardFocusWithin} | CaretIndex: {textBox.CaretIndex}");

            _ = textBox.Focus();
            textBox.UpdateLayout();

            //Debug.WriteLine($"+++ IsFocused: {textBox.IsFocused} | IsKeyboardFocused: {textBox.IsKeyboardFocused} | IsKeyboardFocusWithin: {textBox.IsKeyboardFocusWithin} | CaretIndex: {textBox.CaretIndex}");
        }
    }

    public ICommand GotFocusCommand { get; private set; }

    private void GotFocus(object param)
    {
        HasFocus = true;
    }

    public ICommand LostFocusCommand { get; private set; }

    private void LostFocus(object param)
    {
        HasFocus = false;
    }

    public ICommand GameVersionFilterChangedCommand { get; private set; }
    public ICommand SelectAllGameVersionsCommand { get; private set; }
    public ICommand ClearAllGameVersionsCommand { get; private set; }

    private void GameVersionFilterChanged(object param)
    {
        if (_isUpdatingGameVersionFilter)
        {
            return;
        }

        try
        {
            _isUpdatingGameVersionFilter = true;

            if (param is GameVersionFilter x)
            {
                x.IsSelected = !x.IsSelected;
            }
        }
        finally
        {
            _isUpdatingGameVersionFilter = false;

            OnPropertyChanged(nameof(SelectedGameVersionFilters));
        }
    }

    private void SelectAllGameVersions(object param)
    {
        try
        {
            _isUpdatingGameVersionFilter = true;
            foreach (var filter in GameVersionFilters)
            {
                filter.IsSelected = true;
            }
        }
        finally
        {
            _isUpdatingGameVersionFilter = false;
            OnPropertyChanged(nameof(SelectedGameVersionFilters));
        }
    }

    private void ClearAllGameVersions(object param)
    {
        try
        {
            _isUpdatingGameVersionFilter = true;
            foreach (var filter in GameVersionFilters)
            {
                filter.IsSelected = false;
            }
        }
        finally
        {
            _isUpdatingGameVersionFilter = false;
            OnPropertyChanged(nameof(SelectedGameVersionFilters));
        }
    }

    private void HookGameVersionFilters()
    {
        if (GameVersionFilters == null)
        {
            return;
        }

        GameVersionFilters.CollectionChanged -= GameVersionFilters_CollectionChanged;
        GameVersionFilters.CollectionChanged += GameVersionFilters_CollectionChanged;

        foreach (var item in GameVersionFilters)
        {
            item.PropertyChanged -= GameVersionFilter_PropertyChanged;
            item.PropertyChanged += GameVersionFilter_PropertyChanged;
        }
    }

    private void GameVersionFilters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (GameVersionFilter oldItem in e.OldItems)
            {
                oldItem.PropertyChanged -= GameVersionFilter_PropertyChanged;
            }
        }
        if (e.NewItems != null)
        {
            foreach (GameVersionFilter newItem in e.NewItems)
            {
                newItem.PropertyChanged -= GameVersionFilter_PropertyChanged;
                newItem.PropertyChanged += GameVersionFilter_PropertyChanged;
            }
        }
        OnPropertyChanged(nameof(SelectedGameVersionFilters));
    }

    private void GameVersionFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GameVersionFilter.IsSelected) && !_isUpdatingGameVersionFilter)
        {
            OnPropertyChanged(nameof(SelectedGameVersionFilters));
        }
    }

    #endregion      
}
