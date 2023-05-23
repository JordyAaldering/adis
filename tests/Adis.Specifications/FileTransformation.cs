using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace Adis.Specifications;

[Binding]
public class FileTransformation
{
    [StepArgumentTransformation]
    public static IEnumerable<string> FileLines(Table table)
    {
        return table.Rows.Select(row => row[0]);
    }
}
