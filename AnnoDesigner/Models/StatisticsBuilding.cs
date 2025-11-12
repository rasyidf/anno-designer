using AnnoDesigner.Core.Models;
using System.Diagnostics;

namespace AnnoDesigner.Models;

[DebuggerDisplay("{" + nameof(Count) + ",nq} x {" + nameof(Name) + "}")]
public class StatisticsBuilding : Notify
{
    public int Count
    {
        get;
        set => UpdateProperty(ref field, value);
    }

    public string Name
    {
        get;
        set => UpdateProperty(ref field, value);
    }
}
