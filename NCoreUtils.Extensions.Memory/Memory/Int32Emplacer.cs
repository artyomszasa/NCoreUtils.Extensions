using System;

namespace NCoreUtils.Memory
{
    public sealed class Int32Emplacer : IEmplacer<int>
    {
        public static Int32Emplacer Instance { get; } = new Int32Emplacer();

        private Int32Emplacer() { }

        public int Emplace(int value, Span<char> span)
            => Emplacer.Emplace(value, span);

        public bool TryEmplace(int value, Span<char> span, out int used)
            => TryEmplace(value, span, out used);
    }
}