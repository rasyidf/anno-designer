using AnnoDesigner.Core.Models;
using System.Diagnostics;
using static AnnoDesigner.Core.CoreConstants;

namespace AnnoDesigner.Models;

[DebuggerDisplay("{" + nameof(Name) + ",nq} ({" + nameof(IsSelected) + "})")]
public class GameVersionFilter : Notify
{
    public GameVersion Type
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public string Name
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public bool IsSelected
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public int Order
    {
        get;
        set => UpdateProperty(ref field, value);
    }
}
