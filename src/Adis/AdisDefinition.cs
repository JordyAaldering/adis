using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Adis;

public class AdisDefinition
{
    public int EventNumber { get; }

    public LineStatus LineStatus { get; }

    internal IReadOnlyList<ColumnDefinition> Columns => columns;

    private readonly List<ColumnDefinition> columns = new();
    private readonly IFormatProvider formatProvider;

    public AdisDefinition(int eventNumber, LineStatus lineStatus = LineStatus.Normal)
    {
        EventNumber = eventNumber;
        LineStatus = lineStatus;
        formatProvider = new DefaultFormatProvider();
    }

    public AdisDefinition(int eventNumber, LineStatus lineStatus, IFormatProvider formatProvider)
    {
        EventNumber = eventNumber;
        LineStatus = lineStatus;
        this.formatProvider = formatProvider;
    }

    /// <summary>
    /// Create a new ADIS definition from a string, which should be formatted as follows:
    ///  - 1 character 'D'
    ///  - 1 character that describes the line status
    ///  - 6 characters that describe the event number
    ///  - 11 characters for each column definition
    /// </summary>
    /// <example>DN01234501234567899</example>
    public static AdisDefinition FromLine(string line)
    {
        Debug.Assert(line[0] == (char)LineType.Definition);

        var lineStatus = (LineStatus)line[1];
        int eventNumber = int.Parse(line.AsSpan(2, 6));
        var def = new AdisDefinition(eventNumber, lineStatus);

        int i = 8;
        while (i < line.Length)
        {
            int ddi = int.Parse(line.AsSpan(i, 8));
            int len = int.Parse(line.AsSpan(i + 8, 2));
            int res = int.Parse(line.AsSpan(i + 10, 1));
            def.AddColumnDefinition(ddi, len, res);
            i += 11;
        }

        return def;
    }

    public void AddColumnDefinition(int ddi, int length, int resolution = 0)
    {
        columns.Add(new ColumnDefinition(ddi, length, resolution));
    }

    public int GetLength(int ddi)
    {
        return columns.First(c => c.Ddi == ddi).Length;
    }

    public int GetResolution(int ddi)
    {
        return columns.First(c => c.Ddi == ddi).Resolution;
    }

    public AdisEvent CreateAdisEvent()
    {
        return new AdisEvent(this, EventNumber, LineStatus, formatProvider);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append((char)LineType.Definition);
        sb.Append((char)LineStatus);
        sb.Append($"{EventNumber:D6}");

        foreach (var column in columns)
        {
            sb.Append(column.ToString());
        }

        return sb.ToString();
    }
}
