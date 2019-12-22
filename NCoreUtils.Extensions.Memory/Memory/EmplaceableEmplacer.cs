using System;

namespace NCoreUtils.Memory
{
    public class EmplaceableEmplacer<T> : IEmplacer<T>
        where T : IEmplaceable<T>
    {
        public int Emplace(T value, Span<char> span) => value.Emplace(span);
    }
}