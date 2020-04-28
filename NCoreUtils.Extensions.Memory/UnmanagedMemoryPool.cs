using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NCoreUtils
{
    public class UnmanagedMemoryPool<T> : MemoryPool<T>
        where T : unmanaged
    {
        private static int[] _pow2 = { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024 };

        private static UnmanagedMemoryPool<T>? _shared;

        public new static UnmanagedMemoryPool<T> Shared
        {
            get
            {
                var instance = Interlocked.CompareExchange(ref _shared, default, default);
                if (instance is null)
                {
                    var newInstance = new UnmanagedMemoryPool<T>();
                    instance = Interlocked.CompareExchange(ref _shared, newInstance, default);
                    if (instance is null)
                    {
                        return newInstance;
                    }
                    else
                    {
                        newInstance.Dispose();
                        return instance;
                    }
                }
                return instance;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureValidSize(int value, string parameterName)
        {
            if (value % 1024 != 0)
            {
                throw new ArgumentException("Buffer size must be divisible by 1024.", parameterName);
            }
            if (-1 == Array.BinarySearch(_pow2, value / 1024))
            {
                throw new ArgumentException("Buffer must be 1024 * power of 2.", parameterName);
            }
        }

        private int _isDisposed = 0;

        private (int MaxSize, ConcurrentQueue<UnmanagedMemoryManager<T>> Queue)[] _store;

        public int DefaultBufferSize { get; }

        public bool IsDisposed
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 0 != Interlocked.CompareExchange(ref _isDisposed, 0, 0);
        }

        public override int MaxBufferSize { get; }

        public UnmanagedMemoryPool(int minBufferSize = 4 * 1024, int maxBufferSize = 1024 * 1024, int defaultBufferSize = 32 * 1024)
        {
            EnsureValidSize(minBufferSize, nameof(minBufferSize));
            EnsureValidSize(maxBufferSize, nameof(maxBufferSize));
            EnsureValidSize(defaultBufferSize, nameof(defaultBufferSize));
            if (minBufferSize > maxBufferSize)
            {
                throw new InvalidOperationException("Minimum buffer size must be less than or equal to the maximum buffer size.");
            }
            if (defaultBufferSize > maxBufferSize)
            {
                throw new InvalidOperationException("Default buffer size must be less than or equal to the maximum buffer size.");
            }
            if (defaultBufferSize > minBufferSize)
            {
                throw new InvalidOperationException("Default buffer size must be grater than or equal to the minimum buffer size.");
            }
            var store = new List<(int MaxSize, ConcurrentQueue<UnmanagedMemoryManager<T>> Queue)>();
            var maxPow = maxBufferSize / 1024;
            for (var pow = minBufferSize / 1024; pow <= maxPow; pow *= 2)
            {
                store.Add((1024 * pow, new ConcurrentQueue<UnmanagedMemoryManager<T>>()));
            }
            _store = store.ToArray();
            MaxBufferSize = maxBufferSize;
            DefaultBufferSize = defaultBufferSize;
        }

        private ConcurrentQueue<UnmanagedMemoryManager<T>> GetQueueExact(int size)
        {
            foreach (var (s, q) in _store)
            {
                if (s == size)
                {
                    return q;
                }
            }
            throw new InvalidOperationException("Invalid buffer size.");
        }

        private ConcurrentQueue<UnmanagedMemoryManager<T>> GetQueue(int size, out int bufferSize)
        {
            foreach (var (s, q) in _store)
            {
                if (s > size)
                {
                    bufferSize = s;
                    return q;
                }
            }
            throw new InvalidOperationException("Invalid buffer size.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("UnmanagedMemoryOwner");
            }
        }

        internal void Return(UnmanagedMemoryManager<T> manager)
        {
            if (IsDisposed)
            {
                ((IDisposable)manager).Dispose();
            }
            else
            {
                GetQueueExact(manager.Size).Enqueue(manager);
            }
        }

        public void Free()
        {
            foreach (var (_, queue) in _store)
            {
                while (queue.TryDequeue(out var item))
                {
                    ((IDisposable)item).Dispose();
                }
            }
        }

        public override IMemoryOwner<T> Rent(int minBufferSize = -1)
        {
            ThrowIfDisposed();
            var queue = GetQueue(minBufferSize == -1 ? DefaultBufferSize : minBufferSize, out var bufferSize);
            return new UnmanagedMemoryOwner<T>(
                pool: this,
                manager: queue.TryDequeue(out var manager) ? manager : new UnmanagedMemoryManager<T>(bufferSize)
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
            {
                if (disposing)
                {
                    Free();
                }
            }
        }
    }
}