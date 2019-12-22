using System;

namespace NCoreUtils.Memory
{
    public sealed class CharEmplacer : IEmplacer<char>
    {
        public static CharEmplacer Instance { get; } = new CharEmplacer();

        CharEmplacer() { }

        public int Emplace(char value, Span<char> span)
        {
            if (span.Length < 1)
            {
                throw new InvalidOperationException($"Provided span must be at least 1 character long.");
            }
            span[0] = value;
            return 1;
        }
    }
}