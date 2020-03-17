using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace NCoreUtils
{
    public unsafe sealed class UnmanagedMemoryManager<T> : MemoryManager<T>
        where T : unmanaged
    {
        readonly IntPtr _ptr;

        int _isDisposed;

        public int Size { get; }

        public UnmanagedMemoryManager(int size)
        {
            Size = size;
            _ptr = Marshal.AllocHGlobal(Marshal.SizeOf<T>() * size);
        }

        ~UnmanagedMemoryManager()
            => Dispose(false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ThrowIfDisposed()
        {
            if (0 != Interlocked.CompareExchange(ref _isDisposed, 0, 0))
            {
                throw new ObjectDisposedException(nameof(UnmanagedMemoryManager<T>));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                Marshal.FreeHGlobal(_ptr);
            }
        }

        public override Span<T> GetSpan()
        {
            ThrowIfDisposed();
            return new Span<T>((void*)_ptr, Size);
        }

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            if (elementIndex < 0 || elementIndex >= Size)
            {
                throw new ArgumentOutOfRangeException(nameof(elementIndex));
            }
            return new MemoryHandle((void*)(_ptr + Marshal.SizeOf<T>() * elementIndex));
        }

        public override void Unpin() { }
    }
}