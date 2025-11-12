using System.Diagnostics;
using static AnnoDesigner.Core.CoreConstants;

namespace AnnoDesigner.Models.PresetsTree;

[DebuggerDisplay("{" + nameof(Header) + ",nq}")]
public class GameHeaderTreeItem : GenericTreeItem
{
    public GameHeaderTreeItem() : base(null)
    {
        GameVersion = GameVersion.Unknown;
    }

    public GameVersion GameVersion
    {
        get;
        set => UpdateProperty(ref field, value);
    }
}
