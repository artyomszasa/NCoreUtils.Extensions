namespace NCoreUtils;

public readonly struct AsyncReduceStream<T>
{
    internal Func<Func<T, CancellationToken, ValueTask<bool>>, CancellationToken, ValueTask> Reduce { get; }

    public AsyncReduceStream(Func<Func<T, CancellationToken, ValueTask<bool>>, CancellationToken, ValueTask> reduce)
        => Reduce = reduce;
}

public static class AsyncReduceStream
{
    public static AsyncReduceStream<T> Singleton<T>(T value)
        => new(async (continuation, cancellationToken) => await continuation(value, cancellationToken));

    public static AsyncReduceStream<T> Prepend<T>(this AsyncReduceStream<T> source, T value)
        => new(async (continuation, cancellationToken) =>
        {
            if (await continuation(value, cancellationToken).ConfigureAwait(false))
            {
                await source.Reduce(continuation, cancellationToken).ConfigureAwait(false);
            }
        });

    public static AsyncReduceStream<int> Range(int start, int count)
        => new(async (continuation, cancellationToken) =>
        {
            var next = true;
            for (var i = 0; i < count && next; ++i)
            {
                cancellationToken.ThrowIfCancellationRequested();
                next = await continuation(i + start, cancellationToken).ConfigureAwait(false);
            }
        });

    public static AsyncReduceStream<TResult> Select<TSource, TResult>(this AsyncReduceStream<TSource> source, Func<TSource, TResult> selector)
        => new((continuation, cancellationToken) =>
        {
            return source.Reduce((value, cancellationToken) => continuation(selector(value), cancellationToken), cancellationToken);
        });

    public static AsyncReduceStream<TResult> SelectAwait<TSource, TResult>(this AsyncReduceStream<TSource> source, Func<TSource, ValueTask<TResult>> selector)
        => new((continuation, cancellationToken) =>
        {
            return source.Reduce(async (value, cancellationToken) =>
            {
                var value1 = await selector(value).ConfigureAwait(false);
                return await continuation(value1, cancellationToken).ConfigureAwait(false);
            }, cancellationToken);
        });

    #region reductions

    public static async ValueTask<bool> IsEmptyAsync<T>(this AsyncReduceStream<T> source, CancellationToken cancellationToken = default)
    {
        var isEmpty = true;
        await source.Reduce((_, _) =>
        {
            isEmpty = false;
            return new ValueTask<bool>(false);
        }, cancellationToken).ConfigureAwait(false);
        return isEmpty;
    }

    public static async ValueTask<bool> AllAsync<T>(this AsyncReduceStream<T> source, Func<T, bool> predicate, CancellationToken cancellationToken = default)
    {
        var result = true;
        await source.Reduce((item, _) =>
        {
            result = predicate(item);
            return new ValueTask<bool>(result);
        }, cancellationToken).ConfigureAwait(false);
        return result;
    }

    public static async ValueTask<bool> AllAwaitAsync<T>(this AsyncReduceStream<T> source, Func<T, CancellationToken, ValueTask<bool>> predicate, CancellationToken cancellationToken = default)
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }
        var result = true;
        await source.Reduce(async (item, cancellationToken) =>
        {
            result = await predicate(item, cancellationToken).ConfigureAwait(false);
            return result;
        }, cancellationToken).ConfigureAwait(false);
        return result;
    }

    public static async ValueTask<TAccumulate> AggregateAsync<TSource, TAccumulate>(
        this AsyncReduceStream<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, TAccumulate> func,
        CancellationToken cancellationToken = default)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }
        var result = seed;
        await source.Reduce((item, _) =>
        {
            result = func(result, item);
            return new ValueTask<bool>(true);
        }, cancellationToken);
        return result;
    }

    public static async ValueTask<TAccumulate> AggregateAwaitAsync<TSource, TAccumulate>(
        this AsyncReduceStream<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, CancellationToken, ValueTask<TAccumulate>> func,
        CancellationToken cancellationToken = default)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }
        var result = seed;
        await source.Reduce(async (item, cancellationToken) =>
        {
            result = await func(result, item, cancellationToken);
            return true;
        }, cancellationToken);
        return result;
    }

    public static async ValueTask<TAccumulate> AggregateAwaitAsync<TSource, TAccumulate>(
        this AsyncReduceStream<TSource> source,
        TAccumulate seed,
        Func<TAccumulate, TSource, ValueTask<TAccumulate>> func,
        CancellationToken cancellationToken = default)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }
        var result = seed;
        await source.Reduce(async (item, _) =>
        {
            result = await func(result, item);
            return true;
        }, cancellationToken);
        return result;
    }

    public static async ValueTask<T> AggregateAsync<T>(
        this AsyncReduceStream<T> source,
        Func<T, T, T> func,
        CancellationToken cancellationToken = default)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }
        Maybe<T> result = default;
        await source.Reduce((item, _) =>
        {
            result = (result.TryGetValue(out var prev) ? func(prev, item) : item).Just();
            return new ValueTask<bool>(true);
        }, cancellationToken);
        return result.TryGetValue(out var res)
            ? res
            : throw new InvalidOperationException("Async reduce stream is empty.");
    }

    public static async ValueTask<T> AggregateAwaitAsync<T>(
        this AsyncReduceStream<T> source,
        Func<T, T, CancellationToken, ValueTask<T>> func,
        CancellationToken cancellationToken = default)
    {
        if (func is null)
        {
            throw new ArgumentNullException(nameof(func));
        }
        Maybe<T> result = default;
        await source.Reduce(async (item, cancellationToken) =>
        {
            result = Maybe.Just(result.TryGetValue(out var prev)
                ? await func(prev, item, cancellationToken).ConfigureAwait(false)
                : item
            );
            return true;
        }, cancellationToken);
        return result.TryGetValue(out var res)
            ? res
            : throw new InvalidOperationException("Async reduce stream is empty.");
    }

    #endregion
}