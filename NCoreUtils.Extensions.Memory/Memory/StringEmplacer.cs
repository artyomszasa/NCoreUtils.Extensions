using System;

namespace NCoreUtils.Memory
{
    public sealed class StringEmplacer : IEmplacer<string>
    {
        public static StringEmplacer Instance { get; } = new StringEmplacer();

        StringEmplacer() { }

        public int Emplace(string value, Span<char> span)
        {
            if (value is null)
            {
                return 0;
            }
            if (value.Length > span.Length)
            {
                throw new InvalidOperationException($"Provided span must be at least {value.Length} long.");
            }
            value.AsSpan().CopyTo(span);
            return value.Length;
        }
    }
}