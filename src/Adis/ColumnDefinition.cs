using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Adis;

internal readonly struct ColumnDefinition : IEquatable<ColumnDefinition>
{
	/// <summary>
	/// Unique identifier of this column.
	/// </summary>
	/// <example>12345678</example>
	[Range(0, 99999999)]
	public readonly int Ddi;

	/// <summary>
	/// Length of the field.
	/// </summary>
	/// <example>12</example>
	[Range(0, 99)]
	public readonly int Length;

    /// <summary>
    /// Number of decimal places.
    /// </summary>
    /// <example>1</example>
    [Range(0, 9)]
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
