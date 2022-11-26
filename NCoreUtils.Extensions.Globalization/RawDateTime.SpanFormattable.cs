using System;

namespace NCoreUtils;

public partial struct RawDateTime : ISpanFormattable
{
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        return new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond, DateTimeKind.Unspecified)
            .TryFormat(destination, out charsWritten, format, provider);
    }
}