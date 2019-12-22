using System;

namespace NCoreUtils.Memory
{
    public sealed class UInt8Emplacer : IEmplacer<byte>
    {
        public static UInt8Emplacer Instance { get; } = new UInt8Emplacer();

        UInt8Emplacer() { }

        public int Emplace(byte value, Span<char> span)
            => Int32Emplacer.Instance.Emplace((int)value, span);
    }
}