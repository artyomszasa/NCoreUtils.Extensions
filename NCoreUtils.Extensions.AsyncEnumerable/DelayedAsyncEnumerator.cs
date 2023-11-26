using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils;

internal sealed class DelayedAsyncEnumerator<T>(DelayedAsyncEnumerable<T> parent, CancellationToken cancellationToken) : IAsyncEnumerator<T>
{
    private readonly DelayedAsyncEnumerable<T> _parent = parent;

    private readonly CancellationToken _cancellationToken = cancellationToken;

    private IAsyncEnumerator<T>? _enumerator;

    public T Current => _enumerator is null ? default! : _enumerator.Current;

    private async Task<bool> ContinueMoveNextAsync(ValueTask<IAsyncEnumerable<T>> source)
    {
        _enumerator = (await source.ConfigureAwait(false)).GetAsyncEnumerator(_cancellationToken);
        return await _enumerator.MoveNextAsync();
    }

    public ValueTask DisposeAsync() => _enumerator?.DisposeAsync() ?? default;

    public ValueTask<bool> MoveNextAsync()
    {
        if (_enumerator is null)
        {
            var enumerable = _parent.GetSourceAsync(_cancellationToken);
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