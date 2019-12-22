using System;

namespace NCoreUtils.Memory
{
    public sealed class Int8Emplacer : IEmplacer<sbyte>
    {
        public static Int8Emplacer Instance { get; } = new Int8Emplacer();

        Int8Emplacer() { }

        public int Emplace(sbyte value, Span<char> span)
            => Int32Emplacer.Instance.Emplace(value, span);
    }
}