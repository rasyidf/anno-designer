using AnnoDesigner.Core.Models;
using System.Diagnostics;

namespace AnnoDesigner.Models;

[DebuggerDisplay("{" + nameof(Type) + ",nq} - {" + nameof(Name) + "}")]
public class BuildingInfluence : Notify
{
    public BuildingInfluenceType Type
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
