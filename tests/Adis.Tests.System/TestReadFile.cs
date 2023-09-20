using System.IO;
using Xunit;

namespace Adis.Tests.System;

public class TestReadFile
{
    [Fact]
    public void TestAdisFile()
    {
        using var reader = OpenFile("AdisFile.txt");
        var adisFile = AdisFile.FromReader(reader);
        Assert.NotNull(adisFile);
    }

    [Fact]
    public void TestEmptyFile()
    {
        using var reader = OpenFile("EmptyFile.txt");
        var adisFile = AdisFile.FromReader(reader);
        Assert.NotNull(adisFile);
    }

    [Fact]
    public void TestEmptyAdisFile()
    {
        using var reader = OpenFile("EmptyAdisFile.txt");
        var adisFile = AdisFile.FromReader(reader);
        Assert.NotNull(adisFile);
    }

    [Fact]
    public void TestInvalidAdisFile()
    {
        using var reader = OpenFile("InvalidAdisFile.txt");
        var exception = Record.Exception(() => AdisFile.FromReader(reader));
        Assert.NotNull(exception);
    }

    private static StreamReader OpenFile(string inputFileName)
    {
        var inputFile = new FileInfo(Path.Join(".", "Resources", inputFileName));
        return inputFile.OpenText();
    }
}