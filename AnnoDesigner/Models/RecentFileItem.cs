using AnnoDesigner.Core.Models;
using System;
using System.Diagnostics;

namespace AnnoDesigner.Models;

[DebuggerDisplay("{" + nameof(Path) + ",nq}")]
public class RecentFileItem : Notify
{
    public RecentFileItem(string pathToUse)
    {
        Path = pathToUse;
    }

    public string Path
    {
        get;
        private set => UpdateProperty(ref field, value);
    }

    /// <summary>
    /// The last time the file was loaded/used.
    /// </summary>
    public DateTime LastUsed { get; set; }
}
