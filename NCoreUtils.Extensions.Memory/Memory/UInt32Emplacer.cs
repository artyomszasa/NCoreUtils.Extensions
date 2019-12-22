using System;

namespace NCoreUtils.Memory
{
    public sealed class UInt32Emplacer : IEmplacer<uint>
    {
        public static UInt32Emplacer Instance { get; } = new UInt32Emplacer();

        UInt32Emplacer() { }

        public int Emplace(uint value, Span<char> span)
            => Int64Emplacer.Instance.Emplace((long)value, span);
    }
}