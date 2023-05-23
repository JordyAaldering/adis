using System.Linq;
using Xunit;

namespace Adis.Tests;

public class TestRead
{
    [Fact]
    public void TestAdisHeaderRead()
    {
        const string header = "DN001234000001110230000444405600077777889";

        var def = AdisDefinition.FromLine(header);
        var columns = def.ColumnDefinitions.ToList();

        Assert.Equal(1234, def.EventNumber);

        Assert.Equal(new ColumnDefinition(111, 2, 3), columns[0]);
        Assert.Equal(new ColumnDefinition(4444, 5, 6), columns[1]);
        Assert.Equal(new ColumnDefinition(77777, 88, 9), columns[2]);
    }
}
