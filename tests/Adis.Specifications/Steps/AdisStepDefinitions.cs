using System;
using System.Collections.Generic;
using System.IO;
using TechTalk.SpecFlow;
using Xunit;

namespace Adis.Specifications.Steps
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

        [Then("definition with event number (.*) has the following columns:")]
        public void ThenDefinitionHasColumns(int eventNumber, IEnumerable<ColumnDefinition> columns)
        {
            var def = _adisFile?.GetDefinition(eventNumber);
            Assert.NotNull(def);

            foreach (var column in columns)
            {
                Assert.Equal(column.Length, def.GetLength(column.Ddi));
                Assert.Equal(column.Resolution, def.GetResolution(column.Ddi));
            }
        }
    }
}
