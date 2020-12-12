using System;

namespace NCoreUtils.Memory
{
    public interface IEmplaceable<T>
    {
#if NETSTANDARD2_1
        int Emplace(Span<char> span)
        {
            if (TryEmplace(span, out var used))
            {
                return used;
            }
            throw new InsufficientBufferSizeException(span);
        }
#else
        int Emplace(Span<char> span);
#endif

        bool TryEmplace(Span<char> span, out int used);
    }
}