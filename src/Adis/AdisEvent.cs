using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Adis;

/// <summary>
/// A line of adis data.
/// </summary>
public class AdisEvent
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

	/// <summary>
	/// Create a new ADIS event from a string, which should be formatted as follows:
	///  - 1 character 'V'
	///  - 1 character that describes the line status
	///  - 6 characters that describe the event number
	///  - N characters for each column in the definition
	/// </summary>
	/// <example>VN1234561231234...</example>
	public static AdisEvent FromLine(string line, AdisDefinition definition, IFormatProvider formatProvider)
    {
        char lineType = String.Pop(ref line);
        Debug.Assert(lineType == (char)LineType.Value);

        var lineStatus = (LineStatus)String.Pop(ref line);
        int eventNumber = int.Parse(String.Pop(ref line, 6));
        Debug.Assert(eventNumber == definition.EventNumber);

        var adisEvent = new AdisEvent(definition, eventNumber, lineStatus, formatProvider);

        foreach (var column in definition.Columns)
        {
            var value = String.Pop(ref line, column.Length);
			adisEvent.data[column.Ddi] = value.ToString();
        }

		Debug.Assert(line.Length == 0, $"Line has remaining characters: {line}");

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
