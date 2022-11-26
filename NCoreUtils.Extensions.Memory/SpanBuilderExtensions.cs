using System;
using System.Runtime.CompilerServices;

namespace NCoreUtils
{
    /// <summary>
    /// Span/ReadOnlySpan can only be handled in static methods, see: https://github.com/dotnet/roslyn/issues/23433
    /// </summary>
    public static class SpanBuilderExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("Use member method instead.")]
        public static void Append(in SpanBuilder builder, in Span<char> value)
        {
            value.CopyTo(builder._span[builder.Length..]);
            Unsafe.AsRef(builder._length) += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("Use member method instead.")]
        public static void Append(in SpanBuilder builder, in ReadOnlySpan<char> value)
        {
            value.CopyTo(builder._span[builder.Length..]);
            Unsafe.AsRef(builder._length) += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("Use member method instead.")]
        public static bool TryAppend(in SpanBuilder builder, in Span<char> value)
        {
            if (value.TryCopyTo(builder._span[builder.Length..]))
            {
                Unsafe.AsRef(builder._length) += value.Length;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Obsolete("Use member method instead.")]
        public static bool TryAppend(in SpanBuilder builder, in ReadOnlySpan<char> value)
        {
            if (value.TryCopyTo(builder._span[builder.Length..]))
            {
                Unsafe.AsRef(builder._length) += value.Length;
                return true;
            }
            return false;
        }
    }
}