using AnnoDesigner.Models;
using System;
using System.Diagnostics;
using AnnoDesigner.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AnnoDesigner.PreferencesPages;

/// <summary>
/// Interaction logic for ManageKeybindings.xaml
/// </summary>
public partial class ManageKeybindingsPage : Page, INavigatedTo
{
    private NotifyCollectionChangedEventHandler _observableCollectionChangedHandler;
    private NotifyCollectionChangedEventHandler _managerCollectionChangedHandler;
    private ObservableCollection<Hotkey> _subscribedObservableCollection;

    public void NavigatedTo(object viewModel)
    {
        ViewModel = (ManageKeybindingsViewModel)viewModel;
        DataContext = ViewModel;

        // Ensure ItemsControl is bound to the concrete ObservableCollection so the UI
        // updates reliably even if the DataContext changes or the binding happens later.
        if (ViewModel?.HotkeyCommandManager?.ObservableCollection != null)
        {
            // Wire ItemsSource directly to the observable collection so WPF will update automatically
            // when hotkeys are added/removed.
            HotkeyActions.ItemsSource = ViewModel.HotkeyCommandManager.ObservableCollection;

            // Unsubscribe previous collection handler if any
            if (_subscribedObservableCollection != null && _observableCollectionChangedHandler != null)
            {
                _subscribedObservableCollection.CollectionChanged -= _observableCollectionChangedHandler;
            }

            // Prepare handler (capture 'this')
            _observableCollectionChangedHandler = (s, e) =>
            {
                // Ensure runs on UI thread
                _ = Dispatcher?.InvokeAsync(() =>
                {
                    Debug.WriteLine($"[ManageKeybindingsPage] ObservableCollection changed: Action={e.Action}");
                    if (HotkeyActions.ItemsSource != null)
                    {
                        CollectionViewSource.GetDefaultView(HotkeyActions.ItemsSource).Refresh();
                        // Log current count for diagnostics
                        if (HotkeyActions.ItemsSource is System.Collections.ICollection col)
                        {
                            Debug.WriteLine($"[ManageKeybindingsPage] ItemsSource count after change: {col.Count}");
                        }
                    }
                });
            };

            _subscribedObservableCollection = ViewModel.HotkeyCommandManager.ObservableCollection;
            _subscribedObservableCollection.CollectionChanged += _observableCollectionChangedHandler;
        }

        // Subscribe to manager-level notifications as well (property changes inside Hotkey items)
        ViewModel.HotkeyCommandManager.CollectionChanged -= Manager_CollectionChanged;
        ViewModel.HotkeyCommandManager.CollectionChanged += Manager_CollectionChanged;

        // Diagnostic output: log manager instance and current collection contents
        try
        {
            var manager = ViewModel.HotkeyCommandManager;
            Debug.WriteLine($"[ManageKeybindingsPage] NavigatedTo: HotkeyCommandManager instance={manager?.GetHashCode()}");
            var obs = manager?.ObservableCollection;
            Debug.WriteLine($"[ManageKeybindingsPage] ObservableCollection count={obs?.Count}");
            if (obs != null)
            {
                foreach (var hk in obs)
                {
                    try
                    {
                        var info = hk.GetHotkeyInformation();
                        Debug.WriteLine($"[ManageKeybindingsPage] Hotkey: Id={hk.HotkeyId}, Desc={hk.Description}, Type={info.Type}, Key={info.Key}, Modifiers={info.Modifiers}, MouseAction={info.MouseAction}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[ManageKeybindingsPage] Hotkey (partial): Id={hk?.HotkeyId}, Desc={hk?.Description}, error reading info: {ex.Message}");
                    }
                }
            }
        }
        catch { }

        // Also keep a reference so we can unsubscribe on Unloaded
        _managerCollectionChangedHandler = Manager_CollectionChanged;

        // Unload cleanup
        Unloaded -= ManageKeybindingsPage_Unloaded;
        Unloaded += ManageKeybindingsPage_Unloaded;

        // Try an initial refresh if the ItemsSource is already set.
        if (HotkeyActions.ItemsSource != null)
        {
            CollectionViewSource.GetDefaultView(HotkeyActions.ItemsSource).Refresh();
        }
    }

    private void Manager_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Manually refresh the entire item source only when it's available.
        if (HotkeyActions?.ItemsSource != null)
        {
            _ = Dispatcher?.InvokeAsync(() => CollectionViewSource.GetDefaultView(HotkeyActions.ItemsSource).Refresh());
        }
    }

    private void ManageKeybindingsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        // Clean up subscriptions
        try
        {
            if (_subscribedObservableCollection != null && _observableCollectionChangedHandler != null)
            {
                _subscribedObservableCollection.CollectionChanged -= _observableCollectionChangedHandler;
                _subscribedObservableCollection = null;
            }

            if (ViewModel?.HotkeyCommandManager != null && _managerCollectionChangedHandler != null)
            {
                ViewModel.HotkeyCommandManager.CollectionChanged -= _managerCollectionChangedHandler;
            }
        }
        catch { }
    }

    public ManageKeybindingsViewModel ViewModel;
}
