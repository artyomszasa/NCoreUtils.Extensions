using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils;

public static class AsyncEnumerable
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncEnumerable<T> Delay<T>(Func<CancellationToken, ValueTask<IAsyncEnumerable<T>>> factory)
        => new DelayedAsyncEnumerable<T>(factory);
}