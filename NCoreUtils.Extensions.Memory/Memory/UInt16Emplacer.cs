using System;

namespace NCoreUtils.Memory
{
    public sealed class UInt16Emplacer : IEmplacer<ushort>
    {
        public static UInt16Emplacer Instance { get; } = new UInt16Emplacer();

        UInt16Emplacer() { }

        public int Emplace(ushort value, Span<char> span)
            => Int32Emplacer.Instance.Emplace((int)value, span);
    }
}