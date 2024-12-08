using System.Diagnostics;
using AnnoDesigner.Core.Models;

namespace AnnoDesigner.Models
{
    [DebuggerDisplay("{" + nameof(Type) + ",nq} - {" + nameof(Name) + "}")]
    public class BuildingInfluence : Notify
    {
        private BuildingInfluenceType _type;
        private string _name;

        public BuildingInfluenceType Type
        {
            get { return _type; }
            set { _ = UpdateProperty(ref _type, value); }
        }

        public string Name
        {
            get { return _name; }
            set { _ = UpdateProperty(ref _name, value); }
        }
    }
}
