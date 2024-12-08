using System;
using System.Diagnostics;
using AnnoDesigner.Core.Models;

namespace AnnoDesigner.Models
{
    [DebuggerDisplay("{" + nameof(Path) + ",nq}")]
    public class RecentFileItem : Notify
    {
        private string _path;

        public RecentFileItem(string pathToUse)
        {
            Path = pathToUse;
        }

        public string Path
        {
            get { return _path; }
            private set { _ = UpdateProperty(ref _path, value); }
        }

        /// <summary>
        /// The last time the file was loaded/used.
        /// </summary>
        public DateTime LastUsed { get; set; }
    }
}
