using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Adis;

internal static class String
{
	internal static char Pop([MinLength(1)] ref string s)
	{
		char pop = s[0];
		s = s.Remove(0, 1);
		return pop;
	}

	internal static ReadOnlySpan<char> Pop(ref string s, [Range(1, int.MaxValue)] int amount)
	{
		Debug.Assert(s.Length >= amount);
		var pop = s.AsSpan(0, amount);
		s = s.Remove(0, amount);
		return pop;
	}
}
