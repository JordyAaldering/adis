using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace Adis;

public class AdisRequest
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

    private readonly List<ColumnDefinition> columnDefinitions = new();

    public AdisRequest(int eventNumber, LineStatus lineStatus = LineStatus.Normal)
    {
        EventNumber = eventNumber;
        LineStatus = lineStatus;
    }

    /// <summary>
    /// Create a new ADIS request from a string, which should be formatted as follows:
    ///  - 1 character 'R'
    ///  - 1 character that describes the line status
    ///  - 6 characters that describe the event number
    ///  - 11 characters for each column definition
    /// </summary>
    /// <example>RN12345612345678999...</example>
    public static AdisRequest FromLine(string line)
    {
        var lineType = String.Pop(ref line);
        Debug.Assert(lineType == (char)LineType.Request);

        var lineStatus = (LineStatus)String.Pop(ref line);
        int eventNumber = int.Parse(String.Pop(ref line, 6));
        var request = new AdisRequest(eventNumber, lineStatus);

        while (line.Length >= 11)
        {
            int ddi = int.Parse(String.Pop(ref line, 8));
            int len = int.Parse(String.Pop(ref line, 2));
            int res = int.Parse(String.Pop(ref line, 1));
            request.AddColumnDefinition(ddi, len, res);
        }

        Debug.Assert(line.Length == 0, $"Line has remaining characters: {line}");

        return request;
    }

    public void AddColumnDefinition(int ddi, int length, int resolution = 0)
    {
        columnDefinitions.Add(new ColumnDefinition(ddi, length, resolution));
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append((char)LineType.Request);
        sb.Append((char)LineStatus);
        sb.Append($"{EventNumber:D6}");

        foreach (var column in columnDefinitions)
        {
            sb.Append(column.ToString());
        }

        return sb.ToString();
    }
}
