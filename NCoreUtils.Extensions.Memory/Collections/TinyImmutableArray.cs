using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Collections
{
    public static class TinyImmutableArray
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TinyImmutableArray<T> Create<T>(T value)
            => new(true, value, false, default!, false, default!, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TinyImmutableArray<T> Create<T>(T value1, T value2)
            => new(true, value1, true, value2, false, default!, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TinyImmutableArray<T> Create<T>(T value1, T value2, T value3)
            => new(true, value1, true, value2, true, value3, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TinyImmutableArray<T> Create<T>(IEnumerable<T> source)
        {
            TinyImmutableArray<T>.Builder builder = default;
            foreach (var item in source)
            {
                builder.Add(item);
            }
            return builder.Build();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TinyImmutableArray<T>.Builder CreateBuilder<T>() => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TinyImmutableArray<T> Empty<T>() => default;
    }
}