using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils.Collections
{
    internal struct InlineArray3<T>
    {
        public T First;

        public T Second;

        public T Third;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get
            {
                var span = index switch
                {
                    0 => UnsafeHelpers.AsSpan(First),
                    1 => UnsafeHelpers.AsSpan(Second),
                    2 => UnsafeHelpers.AsSpan(Third),
                    _ => throw new IndexOutOfRangeException()
                };
                return ref MemoryMarshal.GetReference(span);
            }
        }
    }
}