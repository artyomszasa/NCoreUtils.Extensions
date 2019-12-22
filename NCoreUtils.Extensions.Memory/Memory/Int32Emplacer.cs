using System;

namespace NCoreUtils.Memory
{
    public sealed class Int32Emplacer : IEmplacer<int>
    {
        public static Int32Emplacer Instance { get; } = new Int32Emplacer();

        Int32Emplacer() { }

        public int Emplace(int value, Span<char> span)
        {
            if (value == Int32.MinValue)
            {
                return StringEmplacer.Instance.Emplace("-2147483648", span);
            }
            int length;
            if (0 == value)
            {
                if (span.Length < 1)
                {
                    throw new InvalidOperationException("Provided span must be at least 1 character long.");
                }
                length = 1;
                span[0] = '0';
            }
            else
            {
                var isSigned = value < 0 ? 1 : 0;
                value = Math.Abs(value);
                length = (int)Math.Floor(Math.Log10(value)) + 1 + isSigned;
                if (span.Length < length)
                {
                    throw new InvalidOperationException($"Provided span must be at least {length} character(s) long.");
                }
                if (0 != isSigned)
                {
                    span[0] = '-';
                }
                for (var offset = isSigned; value > 0; ++offset)
                {
                    value = Math.DivRem(value, 10, out var part);
                    span[offset] = I(part);
                }
                if (0 != isSigned)
                {
                    span.Slice(1, length -1).Reverse();
                }
                else
                {
                    span.Slice(0, length).Reverse();
                }
            }
            return length;

            static char I(int value) => (char)('0' + value);
        }
    }
}