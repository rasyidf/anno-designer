using Xunit;
using Xunit.Sdk;

namespace PresetParser.Tests.CultureAware
{
    [XunitTestCaseDiscoverer("PresetParser.Tests.CultureAware.CulturedFactAttributeDiscoverer", "PresetParser.Tests")]
    internal sealed class CulturedFactAttribute : FactAttribute
    {
        public CulturedFactAttribute(params string[] cultures) { }

        public string[] Cultures { get; }
    }
}