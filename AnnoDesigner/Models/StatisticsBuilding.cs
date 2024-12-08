using System.Diagnostics;
using AnnoDesigner.Core.Models;

namespace AnnoDesigner.Models
{
    [DebuggerDisplay("{" + nameof(Count) + ",nq} x {" + nameof(Name) + "}")]
    public class StatisticsBuilding : Notify
    {
        private int _count;
        private string _name;

        public int Count
        {
            get { return _count; }
            set { _ = UpdateProperty(ref _count, value); }
        }

        public string Name
        {
            get { return _name; }
            set { _ = UpdateProperty(ref _name, value); }
        }
    }
}
