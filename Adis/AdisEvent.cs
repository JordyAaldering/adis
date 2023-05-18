using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Adis;

/// <summary>
/// A line of adis data
/// </summary>
public class AdisEvent
{
    private readonly int eventNumber;
    private readonly LineStatus lineStatus;
    private readonly AdisDefinition def;

    private readonly Dictionary<int, object?> data = new();

    public object? this[int ddi]
    {
        get => data[ddi];
        set => data[ddi] = value;
    }

    public AdisEvent(AdisDefinition def, int eventNumber, LineStatus lineStatus)
    {
        this.eventNumber = eventNumber;
        this.lineStatus = lineStatus;
        this.def = def;
    }

    public void Read(string line)
    {
        Debug.Assert(line[0] == (char)LineType.Value);

        var lineStatus = (LineStatus)line[1];
        int eventNumber = int.Parse(line.AsSpan(2, 6));

        int i = 8;
        foreach (var def in def.ColumnDefinitions)
        {
            // Make sure we don't read past the line
            if (i > line.Length)
            {
                break;
            }

            string value = line.Substring(i, def.Length);
            data[def.Ddi] = value;
            i += def.Length;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append((char)LineType.Value);
        sb.Append((char)lineStatus);
        sb.Append($"{eventNumber:D6}");

        foreach (var def in def.ColumnDefinitions)
        {
            if (data.TryGetValue(def.Ddi, out var value) && value != null)
            {
                sb.Append(FormatObject(def, value));
            }
            else
            {
                // The value is not set or is null
                sb.Append('?', def.Length);
            }
        }

        return sb.ToString();
    }

    private static string FormatObject(ColumnDefinition columnDefinition, object value)
    {
        if (value is int v)
        {
            return FormatInt(columnDefinition, v);
        }
        
        if (value is float f)
        {
            int intValue = (int)(f * Math.Pow(10, columnDefinition.Resolution));
            return FormatInt(columnDefinition, intValue);
        }
        
        if (value is double d)
        {
            int intValue = (int)(d * Math.Pow(10, columnDefinition.Resolution));
            return FormatInt(columnDefinition, intValue);
        }
        
        string result = value.ToString() ?? "";
        result = result.PadRight(columnDefinition.Length);
        result = result.Substring(0, columnDefinition.Length);
        return result;
    }

    private static string FormatInt(ColumnDefinition columnDefinition, int intValue)
    {
        string result = intValue.ToString();

        // If the string of the given int is equal or smaller than the resolution, zero pad it to always match the given resolution
        if (result.Length <= columnDefinition.Resolution)
        {
            string zeroPadding = new('0', columnDefinition.Resolution + 1);
            result = intValue.ToString(zeroPadding);
        }

        result = result.PadLeft(columnDefinition.Length, '0');
        result = result.Substring(0, columnDefinition.Length);
        return result;
    }
}
