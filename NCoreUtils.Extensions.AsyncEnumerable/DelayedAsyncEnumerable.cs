using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils
{
    public class DelayedAsyncEnumerable<T> : IAsyncEnumerable<T>
    {
        readonly LazyAsync<IAsyncEnumerable<T>> _lazySource;

        public DelayedAsyncEnumerable(Func<CancellationToken, ValueTask<IAsyncEnumerable<T>>> factory)
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            _lazySource = new LazyAsync<IAsyncEnumerable<T>>(factory);
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new DelayedAsyncEnumerator<T>(_lazySource, cancellationToken);
    }
}