using System;
using System.Globalization;

namespace Adis;

public class DefaultFormatProvider : IFormatProvider
{
    public string Serialize<T>(T? value, int length, int resolution = 0)
    {
        if (value == null)
        {
            return NullString(length);
        }

        if (value is int or long or short)
        {
            int i = Convert.ToInt32(value);
            i = (int)(i * Math.Pow(10, resolution));
            return FormatInt(i, length, resolution);
        }

        if (value is float or double or decimal)
        {
            double d = Convert.ToDouble(value);
            int i = (int)(d * Math.Pow(10, resolution));
            return FormatInt(i, length, resolution);
        }

        if (value is DateTime dateTime)
        {
            string format = GetDateFormat(length);
            return dateTime.ToString(format);
        }

        if (value is DateOnly dateOnly)
        {
            string format = GetDateFormat(length);
            dateTime = dateOnly.ToDateTime(new TimeOnly());
            return dateTime.ToString(format);
        }

        string result = value.ToString() ?? "";
        result = result.PadRight(length);
        result = result.Substring(0, length);
        return result;
    }

    public T? Deserialize<T>(string value, int length, int resolution = 0)
    {
        object? res = null;

        if (value == NullString(length))
        {
            return default;
        }

        Type type = typeof(T);

        if (type == typeof(string))
        {
            res = value.Trim();
        }
        else if (type == typeof(int) || type == typeof(long) || type == typeof(short)
            || type == typeof(float) || type == typeof(double) || type == typeof(decimal))
        {
            double i = int.Parse(value);
            res = i / Math.Pow(10, resolution);
            res = Convert.ChangeType(res, type);
        }
        else if (type == typeof(DateTime))
        {
            string format = GetDateFormat(length);
            res = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        }
        else if (type == typeof(DateOnly))
        {
            string format = GetDateFormat(length);
            var dateTime = DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
            res = DateOnly.FromDateTime(dateTime);
        }

        return res == null ? default : (T)res;
    }

    private static string NullString(int length)
    {
        return new string('?', length);
    }

    private static string FormatInt(int value, int length, int resolution)
    {
        string result = value.ToString();

        // If the string of the given int is equal or smaller than the resolution, zero pad it to always match the given resolution
        if (result.Length <= resolution)
        {
            string zeroPadding = new('0', resolution + 1);
            result = value.ToString(zeroPadding);
        }

        result = result.PadLeft(length, '0');
        result = result.Substring(0, length);
        return result;
    }

    private static string GetDateFormat(int length)
    {
        switch (length)
        {
            case 6: return "HHmmss";
            case 8: return "yyyyMMdd";
            case 14: return "yyyyMMddHHmmss";
        }

        throw new ArgumentException($"DateTime does not support length: {length}", nameof(length));
    }
}
