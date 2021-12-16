using System;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Collections.Internal
{
    public struct ImmutableNullable<T>
        where T : unmanaged
    {
        private T _value;

        public bool HasValue { get; }

        public unsafe ref readonly T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("Trying to access empty value.");
                }
                return ref Unsafe.AsRef<T>(Unsafe.AsPointer(ref _value));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ImmutableNullable(in T value)
        {
            _value = value;
            HasValue = true;
        }
    }
}