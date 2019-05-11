using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NCoreUtils
{
    /// <summary>
    /// Contains enumerator extensions.
    /// </summary>
    public static class EnumeratorExtensions
    {

        sealed class SelectEnumerator<TSource, TTarget> : IEnumerator<TTarget>
        {
            readonly IEnumerator<TSource> _source;

            readonly Func<TSource, TTarget> _selector;

            public TTarget Current { get; private set; } = default(TTarget);

            object IEnumerator.Current
            {
                [ExcludeFromCodeCoverage]
                get => Current;
            }

            public SelectEnumerator(IEnumerator<TSource> source, Func<TSource, TTarget> selector)
            {
                _source = source;
                _selector = selector;
            }

            public void Dispose() => _source.Dispose();

            public bool MoveNext()
            {
                if (_source.MoveNext())
                {
                    Current = _selector(_source.Current);
                    return true;
                }
                Current = default(TTarget);
                return false;
            }

            [ExcludeFromCodeCoverage]
            public void Reset()
            {
                _source.Reset();
                Current = default(TTarget);
            }
        }

        sealed class IndexedSelectEnumerator<TSource, TTarget> : IEnumerator<TTarget>
        {
            readonly IEnumerator<TSource> _source;

            readonly Func<TSource, int, TTarget> _selector;

            int _index = -1;

            public TTarget Current { get; private set; } = default(TTarget);

            object IEnumerator.Current
            {
                [ExcludeFromCodeCoverage]
                get => Current;
            }

            public IndexedSelectEnumerator(IEnumerator<TSource> source, Func<TSource, int, TTarget> selector)
            {
                _source = source;
                _selector = selector;
            }

            public void Dispose() => _source.Dispose();

            public bool MoveNext()
            {
                if (_source.MoveNext())
                {
                    Current = _selector(_source.Current, ++_index);
                    return true;
                }
                Current = default(TTarget);
                return false;
            }

            [ExcludeFromCodeCoverage]
            public void Reset()
            {
                _source.Reset();
                _index = -1;
                Current = default(TTarget);
            }
        }

        sealed class WhereEnumerator<T> : IEnumerator<T>
        {
            readonly IEnumerator<T> _source;

            readonly Func<T, bool> _predicate;

            public T Current { get; private set; } = default(T);

            object IEnumerator.Current
            {
                [ExcludeFromCodeCoverage]
                get => Current;
            }

            public WhereEnumerator(IEnumerator<T> source, Func<T, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            public void Dispose() => _source.Dispose();

            public bool MoveNext()
            {
                var found = false;
                var done = false;
                do
                {
                    if (!_source.MoveNext())
                    {
                        done = true;
                        Current = default(T);
                    }
                    else
                    {
                        var current = _source.Current;
                        if (_predicate(current))
                        {
                            found = true;
                            done = true;
                            Current = current;
                        }
                    }
                }
                while (!done);
                return found;
            }

            [ExcludeFromCodeCoverage]
            public void Reset()
            {
                _source.Reset();
                Current = default(T);
            }
        }

        sealed class IndexedWhereEnumerator<T> : IEnumerator<T>
        {
            readonly IEnumerator<T> _source;

            readonly Func<T, int, bool> _predicate;

            int _index = -1;

            public T Current { get; private set; } = default(T);

            object IEnumerator.Current
            {
                [ExcludeFromCodeCoverage]
                get => Current;
            }

            public IndexedWhereEnumerator(IEnumerator<T> source, Func<T, int, bool> predicate)
            {
                _source = source;
                _predicate = predicate;
            }

            public void Dispose() => _source.Dispose();

            public bool MoveNext()
            {
                var found = false;
                var done = false;
                do
                {
                    if (!_source.MoveNext())
                    {
                        done = true;
                        Current = default(T);
                    }
                    else
                    {
                        var current = _source.Current;
                        if (_predicate(current, ++_index))
                        {
                            found = true;
                            done = true;
                            Current = current;
                        }
                    }
                }
                while (!done);
                return found;
            }

            [ExcludeFromCodeCoverage]
            public void Reset()
            {
                _source.Reset();
                _index = -1;
                Current = default(T);
            }
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <param name="source">Source sequence enumerator.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" /></typeparam>
        /// <typeparam name="TTarget">The type of the value returned by <paramref name="selector" />.</typeparam>
        /// <returns>
        /// Enumerator whose elements are the result of invoking the transform function on each element of
        /// <paramref name="source" />.
        /// </returns>
        public static IEnumerator<TTarget> Select<TSource, TTarget>(this IEnumerator<TSource> source, Func<TSource, TTarget> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            return new SelectEnumerator<TSource, TTarget>(source, selector);
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <param name="source">Source sequence enumerator.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" /></typeparam>
        /// <typeparam name="TTarget">The type of the value returned by <paramref name="selector" />.</typeparam>
        /// <returns>
        /// Enumerator whose elements are the result of invoking the transform function on each element of
        /// <paramref name="source" />.
        /// </returns>
        public static IEnumerator<TTarget> Select<TSource, TTarget>(this IEnumerator<TSource> source, Func<TSource, int, TTarget> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            return new IndexedSelectEnumerator<TSource, TTarget>(source, selector);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <param name="source">Source sequence enumerator.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>
        /// Enumerator that contains elements from the input sequence enumerator that satisfy the condition.
        /// </returns>
        public static IEnumerator<T> Where<T>(this IEnumerator<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new WhereEnumerator<T>(source, predicate);
        }

        /// <summary>
        /// Filters a sequence of values based on a predicate. Each element's index is used in the logic of the
        /// predicate function.
        /// </summary>
        /// <param name="source">Source sequence enumerator.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>
        /// Enumerator that contains elements from the input sequence enumerator that satisfy the condition.
        /// </returns>
        public static IEnumerator<T> Where<T>(this IEnumerator<T> source, Func<T, int, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }
            return new IndexedWhereEnumerator<T>(source, predicate);
        }

        /// <summary>
        /// Creates a list from the enumerator.
        /// </summary>
        /// <param name="source">Sequence enumerator.</param>
        /// <typeparam name="T">The type of the elements of <paramref name="source" /></typeparam>
        /// <returns>A list that contains elements from the input sequence enumerator.</returns>
        public static List<T> ToList<T>(this IEnumerator<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var result = new List<T>();
            while (source.MoveNext())
            {
                result.Add(source.Current);
            }
            return result;
        }
    }
}