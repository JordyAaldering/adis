using System;
using System.Collections.Generic;
using System.IO;
using TechTalk.SpecFlow;

namespace Adis.Specifications.StepDefinitions
{
    [Binding]
    public sealed class AdisStepDefinitions
    {
        private AdisFile? _adisFile;

        [Given("the adis file:")]
        public void GivenAdisFile(IEnumerable<string> lines)
        {
            string file = string.Join(Environment.NewLine, lines);
            using var reader = new StringReader(file);
            _adisFile = AdisFile.FromReader(reader);
        }
    }
}
