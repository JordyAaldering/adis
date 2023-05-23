using System;
using Xunit;

namespace Adis.Tests;

public class TestFormatter
{
    private readonly DefaultFormatProvider _formatProvider = new();

    [Theory]
    [InlineData("????????", null)]
    [InlineData("00000000", 0)]
    [InlineData("00001234", 1234)]
    [InlineData("12345678", 12345678)]
    [InlineData("12345678", 1234567890)]
    [InlineData("00123400", 1234, 2)]
    [InlineData("12345000", 12345, 3)]
    public void TestAdisIntFormat(string expected, int? value, int resolution = 0)
    {
        string actual = _formatProvider.Serialize(value, expected.Length, resolution);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("????????", null)]
    [InlineData("00000000", 0.0)]
    [InlineData("00001000", 1.0, 3)]
    [InlineData("00012345", 1234.5, 1)]
    [InlineData("00123450", 1234.5, 2)]
    [InlineData("01234560", 1234.56, 3)]
    [InlineData("12345678", 1234.5678, 4)]
    [InlineData("12345678", 1234.567890, 4)]
    public void TestAdisDoubleFormat(string expected, double? value, int resolution = 0)
    {
        string actual = _formatProvider.Serialize(value, expected.Length, resolution);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("????????", null)]
    [InlineData("        ", "")]
    [InlineData("abcd    ", "abcd")]
    [InlineData("abcdefgh", "abcdefgh")]
    [InlineData("abcdefgh", "abcdefghijklm")]
    public void TestAdisStringFormat(string expected, string? value, int resolution = 0)
    {
        string actual = _formatProvider.Serialize(value, expected.Length, resolution);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("183204", 2023, 5, 12, 18, 32, 4)]
    [InlineData("20230512", 2023, 5, 12, 0, 0, 0)]
    [InlineData("20230512183204", 2023, 5, 12, 18, 32, 4)]
    public void TestAdisDateTimeFormat(string expected, int year, int month, int day, int hour, int minute, int second)
    {
        var dt = new DateTime(year, month, day, hour, minute, second);
        string actual = _formatProvider.Serialize(dt, expected.Length);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("000000", 2023, 5, 12)]
    [InlineData("20230512", 2023, 5, 12)]
    [InlineData("20230512000000", 2023, 5, 12)]
    public void TestAdisDateOnlyFormat(string expected, int year, int month, int day)
    {
        var dt = new DateOnly(year, month, day);
        string actual = _formatProvider.Serialize(dt, expected.Length);
        Assert.Equal(expected, actual);
    }
}
