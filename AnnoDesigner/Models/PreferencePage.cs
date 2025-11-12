using AnnoDesigner.Core.Models;
using Wpf.Ui.Controls;

namespace AnnoDesigner.Models;

public class PreferencePage : Notify
{
    public string HeaderKeyForTranslation
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public string Name
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public Notify ViewModel
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public SymbolRegular Icon
    {
        get;
        set => UpdateProperty(ref field, value);
    }
}
