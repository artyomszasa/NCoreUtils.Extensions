using System;

namespace NCoreUtils.Memory
{
    public interface IEmplacer<in T>
    {
        int Emplace(T value, Span<char> span);

        bool TryEmplace(T value, Span<char> span, out int used);
    }
}