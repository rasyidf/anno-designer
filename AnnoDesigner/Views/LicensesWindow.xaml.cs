using System.Windows;
using AnnoDesigner.ViewModels;

namespace AnnoDesigner
{
    /// <summary>
    /// Interaction logic for LicensesWindow.xaml
    /// </summary>
    public partial class LicensesWindow : Window
    {
        public LicensesWindow()
        {
            InitializeComponent();
            DataContext = new LicensesViewModel();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
}
