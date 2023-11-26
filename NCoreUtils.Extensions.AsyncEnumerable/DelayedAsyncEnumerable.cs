using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils;

public class DelayedAsyncEnumerable<T>(Func<CancellationToken, ValueTask<IAsyncEnumerable<T>>> factory) : IAsyncEnumerable<T>
{
    private const int StateInitial = 0;

    private const int StatePending = 1;

    private const int StateInitialized = 2;

    private int _state = StateInitial;

    private IAsyncEnumerable<T>? _source;

    private Task<IAsyncEnumerable<T>>? _pendingSource;

    private Func<CancellationToken, ValueTask<IAsyncEnumerable<T>>> Factory { get; } = factory ?? throw new ArgumentNullException(nameof(factory));

    private async Task<IAsyncEnumerable<T>> DoGetSourceAsync(Task<IAsyncEnumerable<T>> pending)
    {
        var source = await pending.ConfigureAwait(false);
        _source = source;
        _state = StateInitialized; // relaxed write --> instant visbility not required
        return source;
    }

    internal ValueTask<IAsyncEnumerable<T>> GetSourceAsync(CancellationToken cancellationToken)
    {
        // relaxed access (fast path)
        if (_state == StateInitialized)
        {
            return new(_source!);
        }
        // try enter initializing state..
        var origState = Interlocked.CompareExchange(ref _state, StatePending, StateInitial);
        if (origState == StateInitial)
        {
            // ..entered successfully --> safe to init
            var vt = Factory(cancellationToken);
            if (vt.IsCompletedSuccessfully)
            {
                var source = _source = vt.Result;
                _state = StateInitialized; // relaxed write --> instant visbility not required
                return new(source);
            }
            var pendingSource = _pendingSource = DoGetSourceAsync(vt.AsTask());
            return new(pendingSource);
        }
        // ..failed to enter
        if (origState == StateInitialized)
        {
            // already initialized by other thread --> recheck as initialization may be pending
            var source = _source;
            if (source is null)
            {
                // rerun
                return GetSourceAsync(cancellationToken);
            }
            return new(source);
        }
        // already started by another thread --> recheck as initialization may be pending
        {
            var pendingSource = _pendingSource;
            if (pendingSource is null)
            {
                // rerun
                return GetSourceAsync(cancellationToken);
            }
            return new(pendingSource);
        }
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new DelayedAsyncEnumerator<T>(this, cancellationToken);
}