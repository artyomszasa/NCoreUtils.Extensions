using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils
{
    sealed class DelayedAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        readonly LazyAsync<IAsyncEnumerable<T>> _lazySource;

        readonly CancellationToken _cancellationToken;

        IAsyncEnumerator<T>? _enumerator;

        public T Current => _enumerator is null ? default! : _enumerator.Current;

        public DelayedAsyncEnumerator(LazyAsync<IAsyncEnumerable<T>> lazySource, CancellationToken cancellationToken)
        {
            _lazySource = lazySource;
            _cancellationToken = cancellationToken;
        }

        async Task<bool> ContinueMoveNextAsync(ValueTask<IAsyncEnumerable<T>> source)
        {
            _enumerator = (await source.ConfigureAwait(false)).GetAsyncEnumerator(_cancellationToken);
            return await _enumerator.MoveNextAsync();
        }

        public ValueTask DisposeAsync() => _enumerator?.DisposeAsync() ?? default;

        public ValueTask<bool> MoveNextAsync()
        {
            if (_enumerator is null)
            {
                // _enumerator = (await _lazySource.GetResultAsync(_cancellationToken)).GetAsyncEnumerator(_cancellationToken);
                var enumerable = _lazySource.GetResultAsync(_cancellationToken);
                if (enumerable.IsCompletedSuccessfully)
                {
                    _enumerator = enumerable.Result.GetAsyncEnumerator();
                }
                else
                {
                    // continue asynchronously.
                    return new ValueTask<bool>(ContinueMoveNextAsync(enumerable));
                }
            }
            return _enumerator.MoveNextAsync();
        }
    }
}