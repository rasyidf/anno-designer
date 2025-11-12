using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace PresetParser.Tests.CultureAware
{
    internal class CulturedFactAttributeDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink diagnosticMessageSink;

        public CulturedFactAttributeDiscoverer(IMessageSink diagnosticMessageSink)
        {
            this.diagnosticMessageSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            object[] ctorArgs = factAttribute.GetConstructorArguments().ToArray();
            string[] cultures = Reflector.ConvertArguments(ctorArgs, new[] { typeof(string[]) }).Cast<string[]>().Single();

            if (cultures == null || cultures.Length == 0)
            {
                cultures = new[] { "en-US", "de-DE" };
            }

            TestMethodDisplay methodDisplay = discoveryOptions.MethodDisplayOrDefault();
            TestMethodDisplayOptions methodDisplayOptions = discoveryOptions.MethodDisplayOptionsOrDefault();

            return cultures.Select(culture => new CulturedXunitTestCase(diagnosticMessageSink, methodDisplay, methodDisplayOptions, testMethod, culture)).ToList();
        }
    }
}