using System;

namespace NCoreUtils.Memory
{
    public interface IEmplaceable<T>
    {
        int Emplace(Span<char> span);

        bool TryEmplace(Span<char> span, out int used);
    }
}