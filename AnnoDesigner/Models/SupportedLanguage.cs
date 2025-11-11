using AnnoDesigner.Core.Models;
using System.Diagnostics;

namespace AnnoDesigner.Models;

[DebuggerDisplay("{" + nameof(Name) + ",nq} ({" + nameof(IsSelected) + "})")]
public class SupportedLanguage : Notify
{
    public SupportedLanguage(string nameToUse)
    {
        Name = nameToUse;
    }

    public string Name
    {
        get;
        private set => UpdateProperty(ref field, value);
    }

    public string FlagPath
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public bool IsSelected
    {
        get;
        set => UpdateProperty(ref field, value);
    }
}


