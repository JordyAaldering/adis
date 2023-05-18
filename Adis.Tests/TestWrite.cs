using Xunit;

namespace Adis.Tests;

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

    [Theory]
    [InlineData(null, "????????")]
    [InlineData("", "        ")]
    [InlineData("abcd", "abcd    ")]
    [InlineData("abcdefgh", "abcdefgh")]
    [InlineData("abcdefghij", "abcdefgh")]
    [InlineData(0, "00000000")]
    [InlineData(1234, "00001234")]
    [InlineData(12345678, "12345678")]
    [InlineData(1234567890, "12345678")]
    [InlineData(0.0, "00000000", 1)]
    [InlineData(1234.5, "00012345", 1)]
    [InlineData(1234.56, "01234560", 3)]
    [InlineData(1234.5678, "12345678", 4)]
    [InlineData(1234.567890, "12345678", 4)]
    public void TestAdisEventFormat(object? actual, string expected, int resolution = 0)
    {
        var def = new AdisDefinition(1234);
        def.AddColumnDefinition(111, expected.Length, resolution);

        var adisEvent = def.CreateAdisEvent();
        adisEvent[111] = actual;

        Assert.Equal($"VN001234{expected}", adisEvent.ToString());
    }
}
