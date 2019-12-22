using System;

namespace NCoreUtils.Memory
{
    public sealed class Int16Emplacer : IEmplacer<short>
    {
        public static Int16Emplacer Instance { get; } = new Int16Emplacer();

        Int16Emplacer() { }

        public int Emplace(short value, Span<char> span)
            => Int32Emplacer.Instance.Emplace(value, span);
    }
}