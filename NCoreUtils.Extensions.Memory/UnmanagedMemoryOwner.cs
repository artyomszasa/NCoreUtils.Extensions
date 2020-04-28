using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NCoreUtils
{
    internal sealed class UnmanagedMemoryOwner<T> : IMemoryOwner<T>
        where T : unmanaged
    {
        private int _isDisposed = 0;

        public UnmanagedMemoryManager<T> Manager { get; }

        public UnmanagedMemoryPool<T> Pool { get; }

        public bool IsDisposed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 0 != Interlocked.CompareExchange(ref _isDisposed, 0, 0);
        }

        public Memory<T> Memory
        {
            get
            {
                ThrowIfDisposed();
                return Manager.Memory;
            }
        }

        public UnmanagedMemoryOwner(UnmanagedMemoryPool<T> pool, UnmanagedMemoryManager<T> manager)
        {
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));
            Pool = pool ?? throw new ArgumentNullException(nameof(pool));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("UnmanagedMemoryOwner");
            }
        }

        public void Dispose()
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                Pool.Return(Manager);
            }
        }
    }
}