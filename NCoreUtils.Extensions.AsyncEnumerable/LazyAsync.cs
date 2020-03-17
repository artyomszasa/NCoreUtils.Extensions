using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils
{
    sealed class LazyAsync<T>
    {
        int _sync = 0;

        Func<CancellationToken, ValueTask<T>>? _factory;

        volatile bool _isInitialized = false;

        T _value = default!;

        public LazyAsync(Func<CancellationToken, ValueTask<T>> factory)
        {
            _factory = factory;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void EmplaceNoRelease(T value)
        {
            _value = value;
            _isInitialized = true;
            _factory = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T EmplaceValue(T value)
        {
            EmplaceNoRelease(value);
            Interlocked.CompareExchange(ref _sync, 0, 1);
            return value;
        }

        async Task<T> ContinueAsync(ValueTask<T> source)
        {
            try
            {
                var result = await source.ConfigureAwait(false);
                EmplaceNoRelease(result);
                return result;
            }
            finally
            {
                // release lock.
                Interlocked.CompareExchange(ref _sync, 0, 1);
            }
        }

        public ValueTask<T> GetResultAsync(CancellationToken cancellationToken)
        {
            if (_isInitialized)
            {
                return new ValueTask<T>(_value);
            }
            // acquire lock.
            while (0 == Interlocked.CompareExchange(ref _sync, 1, 0)) { }
            if (_isInitialized)
            {
                // release lock.
                Interlocked.CompareExchange(ref _sync, 0, 1);
                return new ValueTask<T>(_value);
            }
            try
            {
                var asyncResult = _factory!(cancellationToken);
                if (asyncResult.IsCompletedSuccessfully)
                {
                    // lock released inside EmplaceValue.
                    return new ValueTask<T>(EmplaceValue(asyncResult.Result));
                }
                // switch to async, lock released once operation has finished.
                return new ValueTask<T>(ContinueAsync(asyncResult));
            }
            catch
            {
                // release lock.
                Interlocked.CompareExchange(ref _sync, 0, 1);
                throw;
            }
        }
    }
}