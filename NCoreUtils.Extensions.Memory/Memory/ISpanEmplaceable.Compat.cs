using System;

namespace NCoreUtils;

public interface ISpanEmplaceable
{
    int Emplace(Span<char> span);

    bool TryGetEmplaceBufferSize(out int minimumBufferSize);

    bool TryEmplace(Span<char> span, out int used);

    bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider);
}