using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System;

namespace AnnoDesigner.Views
{
    public partial class BuildingPresetsPanel : UserControl
    {
        public BuildingPresetsPanel()
        {
            InitializeComponent();
        }

        private void TextBoxSearchPresetsKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                //used to fix issue with misplaced caret in TextBox
                var tb = sender as System.Windows.Controls.Control;
                tb?.UpdateLayout();
                _ = tb?.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
            }
        }
    }
}