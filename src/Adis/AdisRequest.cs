﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Adis;

public class AdisRequest
{
    public int EventNumber { get; }

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
    /// <example>RN01234501234567899</example>
    public static AdisRequest FromLine(string line)
    {
        Debug.Assert(line[0] == (char)LineType.Request);

        var lineStatus = (LineStatus)line[1];
        int eventNumber = int.Parse(line.AsSpan(2, 6));
        var def = new AdisRequest(eventNumber, lineStatus);

        int i = 8;
        while (i < line.Length - 11)
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
