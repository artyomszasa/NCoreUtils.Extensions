using System;

namespace NCoreUtils.Memory
{
    public class DefaultEmplacer<T> : IEmplacer<T>
    {
        public int Emplace(T value, Span<char> span)
        {
            var stringValue = value?.ToString();
            if (stringValue is null)
            {
                return 0;
            }
            if (stringValue.Length > span.Length)
            {
                throw new InvalidOperationException($"Provided span must be at least {stringValue.Length} character(s) long.");
            }
            stringValue.AsSpan().CopyTo(span);
            return stringValue.Length;
        }
    }
}