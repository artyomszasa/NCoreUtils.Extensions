using System;

namespace NCoreUtils;

public interface ISpanEmplaceable : ISpanFormattable
{
    int Emplace(Span<char> span)
    {
        if (TryEmplace(span, out var used))
        {
            return used;
        }
        throw new InsufficientBufferSizeException(span);
    }

    bool TryGetEmplaceBufferSize(out int minimumBufferSize)
    {
        minimumBufferSize = default;
        return false;
    }

    bool TryEmplace(Span<char> span, out int used);

    bool ISpanFormattable.TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => TryEmplace(destination, out charsWritten);
}