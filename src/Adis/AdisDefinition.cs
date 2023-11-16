using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Adis;

public class AdisDefinition
{
	/// <summary>
	/// The event number of this request.
	/// </summary>
	/// <example>123456</example>
	[Range(0, 999999)]
	public int EventNumber { get; }

	/// <summary>
	/// The line status of this request.
	/// </summary>
	/// <example>N</example>
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
    /// <example>DN12345612345678999...</example>
    public static AdisDefinition FromLine(string line)
    {
        var lineType = String.Pop(ref line);
        Debug.Assert(lineType == (char)LineType.Definition);

        var lineStatus = (LineStatus)String.Pop(ref line);
        int eventNumber = int.Parse(String.Pop(ref line, 6));
        var def = new AdisDefinition(eventNumber, lineStatus);

        while (line.Length >= 11)
        {
            int ddi = int.Parse(String.Pop(ref line, 8));
            int len = int.Parse(String.Pop(ref line, 2));
            int res = int.Parse(String.Pop(ref line, 1));
            def.AddColumnDefinition(ddi, len, res);
        }

        Debug.Assert(line.Length == 0, $"Line has remaining characters: {line}");

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
