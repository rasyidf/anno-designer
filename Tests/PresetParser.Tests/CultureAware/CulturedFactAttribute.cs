using Xunit;
using Xunit.Sdk;

namespace PresetParser.Tests.CultureAware
{
    [XunitTestCaseDiscoverer("PresetParser.Tests.CultureAware.CulturedFactAttributeDiscoverer", "PresetParser.Tests")]
    public sealed class CulturedFactAttribute : FactAttribute
    {
        public CulturedFactAttribute(params string[] cultures) { }
    }
}