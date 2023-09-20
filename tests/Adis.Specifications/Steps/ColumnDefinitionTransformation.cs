using System.Collections.Generic;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Adis.Specifications.Steps;

[Binding]
public class ColumnDefinitionTransformation
{
    [StepArgumentTransformation]
    public static IEnumerable<ColumnDefinition> ColumnDefinitions(Table table)
    {
        return table.CreateSet<ColumnDefinition>();
    }
}
