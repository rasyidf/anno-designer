using AnnoDesigner.Core.Models;
using AnnoDesigner.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Navigation;

namespace AnnoDesigner.ViewModels;

public class PreferencesViewModel : Notify
{
    private PreferencePage _selectedItem;

    public PreferencesViewModel()
    {
        Pages = [];
        CloseWindowCommand = new RelayCommand<ICloseable>(CloseWindow);
    }

    public NavigationService NavigationService { get; set; }

    public PreferencePage SelectedItem
    {
        get => _selectedItem;
        set
        {
            _ = UpdateProperty(ref _selectedItem, value);
            ShowPage(value.Name);
        }
    }

    public ObservableCollection<PreferencePage> Pages { get; set; }

    public void ShowFirstPage()
    {
        if (Pages.Count > 0)
        {
            SelectedItem = Pages.First();
        }
    }

    private void ShowPage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        PreferencePage foundPage = Pages.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (foundPage != null)
        {
            try
            {
                NavigationService?.Navigate(new Uri($@"pack://application:,,,/Views\PreferencesPages\{name}.xaml", UriKind.RelativeOrAbsolute), foundPage.ViewModel);
            }
            catch (Exception ex)
            {
                // Log navigation error but don't crash
                System.Diagnostics.Debug.WriteLine($"Failed to navigate to page {name}: {ex.Message}");
            }
        }
    }

    public ICommand CloseWindowCommand { get; private set; }
    private void CloseWindow(ICloseable window)
    {
        window?.Close();
    }
}
