using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils.Collections
{
    internal static class UnsafeHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Span<T> AsSpan<T>(in T directReference)
        {
            ref T r0 = ref Unsafe.AsRef(directReference);
            return MemoryMarshal.CreateSpan(ref r0, 1);
        }
    }
}