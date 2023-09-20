using System;

namespace Adis;

internal readonly struct ColumnDefinition : IEquatable<ColumnDefinition>
{
    /// <summary>
    /// Unique identifier of this column.
    /// </summary>
    /// <example>01234567</example>
    public readonly int Ddi;

    /// <summary>
    /// Length of the field.
    /// </summary>
    /// <example>01</example>
    public readonly int Length;

    /// <summary>
    /// Number of decimal places.
    /// </summary>
    /// <example>0</example>
    public readonly int Resolution;

    public ColumnDefinition(int ddi, int length, int resolution = 0)
    {
        Ddi = ddi;
        Length = length;
        Resolution = resolution;
    }

    public bool Equals(ColumnDefinition other)
    {
        return Ddi == other.Ddi && Length == other.Length && Resolution == other.Resolution;
    }

    public override string ToString()
    {
        return $"{Ddi:D8}{Length:D2}{Resolution:D1}";
    }
}
