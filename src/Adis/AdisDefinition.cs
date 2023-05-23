using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Adis;

public class AdisDefinition
{
    public int EventNumber { get; }
    public LineStatus LineStatus { get; }

    private readonly List<ColumnDefinition> columnDefinitions = new();

    internal IReadOnlyList<ColumnDefinition> ColumnDefinitions => columnDefinitions;

    public AdisDefinition(int eventNumber, LineStatus lineStatus = LineStatus.Normal)
    {
        EventNumber = eventNumber;
        LineStatus = lineStatus;
    }

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
        columnDefinitions.Add(new ColumnDefinition(ddi, length, resolution));
    }

    public AdisEvent CreateAdisEvent()
    {
        return new AdisEvent(this, EventNumber, LineStatus, new DefaultFormatProvider());
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append((char)LineType.Definition);
        sb.Append((char)LineStatus);
        sb.Append($"{EventNumber:D6}");

        foreach (var column in columnDefinitions)
        {
            sb.Append(column.ToString());
        }

        return sb.ToString();
    }
}
