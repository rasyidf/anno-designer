using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AnnoDesigner.Core.Presets.Models
{
    [DebuggerDisplay("{" + nameof(Version) + "}")]
    [DataContract]
    public class IconMappingPresets
    {
        public IconMappingPresets()
        {
            Version = string.Empty;
            IconNameMappings = [];
        }

        [DataMember(Order = 0)]
        public string Version { get; set; }

        [DataMember(Order = 1)]
        public List<IconNameMap> IconNameMappings { get; set; }
    }
}
