using Xunit;

namespace Adis.Tests.Unit;

public class TestWrite
{
    [Fact]
    public void TestAdisHeaderFormat()
    {
        var def = new AdisDefinition(1234);
        def.AddColumnDefinition(111, 2, 3);
        def.AddColumnDefinition(4444, 5, 6);
        def.AddColumnDefinition(77777, 88, 9);

        Assert.Equal("DN001234000001110230000444405600077777889", def.ToString());
    }
}
