using System;

namespace Adis;

internal readonly struct ColumnDefinition : IEquatable<ColumnDefinition>
{
    /// <summary>
    /// Item number
    /// </summary>
    public readonly int Ddi;

    /// <summary>
    /// Length of the field
    /// </summary>
    public readonly int Length;

    /// <summary>
    /// Decimal places
    /// </summary>
    public readonly int Resolution;

    public ColumnDefinition(int ddi, int length, int resolution)
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
