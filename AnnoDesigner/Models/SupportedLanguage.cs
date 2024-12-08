using System.Diagnostics;
using AnnoDesigner.Core.Models;

namespace AnnoDesigner.Models
{
    [DebuggerDisplay("{" + nameof(Name) + ",nq} ({" + nameof(IsSelected) + "})")]
    public class SupportedLanguage : Notify
    {
        private string _name;
        private string _flagPath;
        private bool _isSelected;

        public SupportedLanguage(string nameToUse)
        {
            Name = nameToUse;
        }

        public string Name
        {
            get { return _name; }
            private set { _ = UpdateProperty(ref _name, value); }
        }

        public string FlagPath
        {
            get { return _flagPath; }
            set { _ = UpdateProperty(ref _flagPath, value); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _ = UpdateProperty(ref _isSelected, value); }
        }
    }
}


