using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Adis;

/// <summary>
/// A line of adis data.
/// </summary>
public class AdisEvent
{
    public int EventNumber { get; }
    public LineStatus LineStatus { get; }

    private readonly Dictionary<int, string> data = new();
    private readonly IReadOnlyList<ColumnDefinition> columnDefinitions;
    private readonly IFormatProvider formatProvider;

    public AdisEvent(AdisDefinition definition, int eventNumber, LineStatus lineStatus = LineStatus.Normal)
    {
        EventNumber = eventNumber;
        LineStatus = lineStatus;
        columnDefinitions = definition.Columns;
        formatProvider = new DefaultFormatProvider();
    }

    public AdisEvent(AdisDefinition definition, int eventNumber, LineStatus lineStatus, IFormatProvider formatProvider)
    {
        EventNumber = eventNumber;
        LineStatus = lineStatus;
        columnDefinitions = definition.Columns;
        this.formatProvider = formatProvider;
    }

    public T? GetData<T>(int ddi)
    {
        var col = columnDefinitions.First(d => d.Ddi == ddi);
        return formatProvider.Deserialize<T>(data[ddi], col.Length, col.Resolution);
    }

    public void SetData<T>(int ddi, T? value)
    {
        var col = columnDefinitions.First(d => d.Ddi == ddi);
        data[ddi] = formatProvider.Serialize(value, col.Length, col.Resolution);
    }

    public static AdisEvent FromLine(string line, AdisDefinition definition, IFormatProvider formatProvider)
    {
        Debug.Assert(line[0] == (char)LineType.Value);
        var lineStatus = (LineStatus)line[1];
        int eventNumber = int.Parse(line.AsSpan(2, 6));
        Debug.Assert(eventNumber == definition.EventNumber);

        var adisEvent = new AdisEvent(definition, eventNumber, lineStatus, formatProvider);

        int i = 8;
        foreach (var column in definition.Columns)
        {
            string value = line.Substring(i, column.Length);
            adisEvent.data[column.Ddi] = value;
            i += column.Length;
        }

        return adisEvent;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append((char)LineType.Value);
        sb.Append((char)LineStatus);
        sb.Append($"{EventNumber:D6}");

        foreach (var def in columnDefinitions)
        {
            sb.Append(data[def.Ddi]);
        }

        return sb.ToString();
    }
}
