using System;

namespace NCoreUtils.Memory
{
    public sealed class UInt64Emplacer : IEmplacer<ulong>
    {
        static ulong DivRem(ulong a, ulong b, out ulong reminder)
        {
            reminder = a % b;
            return a / b;
        }

        public static UInt64Emplacer Instance { get; } = new UInt64Emplacer();

        UInt64Emplacer() { }

        public int Emplace(ulong value, Span<char> span)
        {
            int length;
            if (0L == value)
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
                length = (int)Math.Floor(Math.Log10(value)) + 1;
                if (span.Length < length)
                {
                    throw new InvalidOperationException($"Provided span must be at least {length} character(s) long.");
                }
                for (var offset = 0; value != 0; ++offset)
                {
                    value = DivRem(value, 10L, out var part);
                    unchecked
                    {
                        span[offset] = I((int)part);
                    }
                }
                span.Slice(0, length).Reverse();
            }
            return length;

            static char I(int value) => (char)('0' + value);
        }
    }
}