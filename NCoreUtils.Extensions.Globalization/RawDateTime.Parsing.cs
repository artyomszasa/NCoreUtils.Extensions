using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace NCoreUtils;

public partial struct RawDateTime
#if NET7_0_OR_GREATER
    : IParsable<RawDateTime>
    , ISpanParsable<RawDateTime>
#endif
{
    public static RawDateTime Parse(string input, IFormatProvider? provider)
    {
        if (TryParse(input, provider, out var result))
        {
            return result;
        }
        throw new FormatException("Specified value is not a valid date/time.");
    }

    public static RawDateTime Parse(ReadOnlySpan<char> input, IFormatProvider? provider)
    {
        if (TryParse(input, provider, out var result))
        {
            return result;
        }
        throw new FormatException("Specified value is not a valid date/time.");
    }

    public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? provider, DateTimeStyles styles, out RawDateTime result)
    {
        if (DateTime.TryParse(input, provider, styles, out var dt))
        {
            result = new(dt);
            return true;
        }
        result = default;
        return false;
    }

    public static bool TryParse(string? input, IFormatProvider? provider, DateTimeStyles styles, out RawDateTime result)
        => TryParse(input.AsSpan(), provider, styles, out result);

    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, out RawDateTime result)
        => TryParse(input, provider, DateTimeStyles.AllowWhiteSpaces, out result);

    public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? provider, out RawDateTime result)
        => TryParse(input, provider, DateTimeStyles.AllowWhiteSpaces, out result);
}