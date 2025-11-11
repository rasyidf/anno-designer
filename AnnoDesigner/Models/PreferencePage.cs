using AnnoDesigner.Core.Models;
using Wpf.Ui.Controls;

namespace AnnoDesigner.Models;

public class PreferencePage : Notify
{
    private string _headerKeyForTranslation;
    private string _name;
    private Notify _viewModel;
    private SymbolRegular _icon;

    public string HeaderKeyForTranslation
    {
        get => _headerKeyForTranslation;
        set => UpdateProperty(ref _headerKeyForTranslation, value);
    }

    public string Name
    {
        get => _name;
        set => UpdateProperty(ref _name, value);
    }

    public Notify ViewModel
    {
        get => _viewModel;
        set => UpdateProperty(ref _viewModel, value);
    }

    public SymbolRegular Icon
    {
        get => _icon;
        set => UpdateProperty(ref _icon, value);
    }
}
