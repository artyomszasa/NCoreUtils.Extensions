using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NCoreUtils.Collections.Internal;

namespace NCoreUtils.Collections
{
    public struct ImmutableHeadArray<T>
        where T : unmanaged
    {
        public delegate TResult Mapping<TResult>(in T value) where TResult : unmanaged;

        public delegate TResult IndexedMapping<TResult>(in T value, int index) where TResult : unmanaged;

        public unsafe ref struct Enumerator
        {
            private static readonly T _dummy;

            private readonly void* _ref;

            /// <summary>
            /// Current popition:
            /// <para><c>null</c> - uninitialized;</para>
            /// <para><c>-2</c> - end reached;</para>
            /// <para><c>-1</c> - not started;</para>
            /// <para>positive number - valid enumeration point.</para>
            /// </summary>
            private int? _position;

            public ref readonly T Current
            {
                get
                {
                    if (_position.HasValue)
                    {
                        var pos = _position.Value;
                        if (pos >= 0)
                        {
                            return ref Unsafe.AsRef<ImmutableHeadArray<T>>(_ref)[pos];
                        }
                    }
                    return ref _dummy;
                }
            }

            internal Enumerator(void* @ref)
            {
                _ref = @ref;
                _position = -1;
            }

            public bool MoveNext()
            {
                if (_position.HasValue)
                {
                    var pos = _position.Value;
                    if (-2 == pos)
                    {
                        return false;
                    }
                    var posNext = pos + 1;
                    if (posNext < Unsafe.AsRef<ImmutableHeadArray<T>>(_ref).Length)
                    {
                        _position = posNext;
                        return true;
                    }
                    _position = -2;
                    return false;
                }
                return false;
            }
        }

        private static T[] ArrayAppend(T[] source, in T value)
        {
            var result = new T[source.Length + 1];
            for (var i = 0; i < source.Length; ++i)
            {
                result[i] = source[i];
            }
            result[source.Length] = value;
            return result;
        }

        private static T[] ArrayAppend(T[] source, in ImmutableHeadArray<T> value)
        {
            var result = new T[source.Length + value.Length];
            for (var i = 0; i < source.Length; ++i)
            {
                result[i] = source[i];
            }
            if (value._head.HasValue)
            {
                result[source.Length] = value._head.Value;
            }
            if (value._tail is not null)
            {
                for (var i = 0; i < value._tail.Length; ++i)
                {
                    result[source.Length + 1 + i] = value._tail[i];
                }
            }
            return result;
        }

        private static T[] ArrayPop(T[] source, out T value)
        {
            var result = new T[source.Length - 1];
#if NETFRAMEWORK
            value = source[source.Length - 1];
#else
            value = source[^1];
#endif
            for (var i = 0; i < source.Length - 1; ++i)
            {
                result[i] = source[i];
            }
            return result;
        }

        private static T[] ArrayPrepend(T[] source, in T value)
        {
            var result = new T[source.Length + 1];
            result[0] = value;
            for (var i = 0; i < source.Length; ++i)
            {
                result[i + 1] = source[i];
            }
            return result;
        }

        private static T[] ArrayUnshift(T[] source, out T value)
        {
            value = source[0];
            var result = new T[source.Length - 1];
            for (var i = 1; i < source.Length; ++i)
            {
                result[i - 1] = source[i];
            }
            return result;
        }

        private static TResult[] ArrayMap<TResult>(T[] source, Mapping<TResult> mapping)
            where TResult : unmanaged
        {
            var result = new TResult[source.Length];
            for (var i = 0; i < source.Length; ++i)
            {
                result[i] = mapping(in source[i]);
            }
            return result;
        }

        private static TResult[] ArrayMapIndexed<TResult>(T[] source, IndexedMapping<TResult> mapping)
            where TResult : unmanaged
        {
            var result = new TResult[source.Length];
            for (var i = 0; i < source.Length; ++i)
            {
                result[i] = mapping(in source[i], i + 1);
            }
            return result;
        }

        private ImmutableNullable<T> _head;

        private readonly T[]? _tail;

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _head.HasValue
                ? _tail is null ? 1 : _tail.Length + 1
                : 0;
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !_head.HasValue;
        }

        public ref readonly T this[int index]
        {
            get
            {
                if (0 == index)
                {
                    if (_head.HasValue)
                    {
                        return ref _head.Value;
                    }
                    throw new IndexOutOfRangeException();
                }
                if (_tail is null)
                {
                    throw new IndexOutOfRangeException();
                }
                return ref _tail[index - 1];
            }
        }

        public ImmutableHeadArray(in T head, T[]? tail = default)
        {
            _head = new ImmutableNullable<T>(head);
            _tail = tail;
        }

        public ImmutableHeadArray(ReadOnlySpan<T> source)
        {
            if (source.Length > 0)
            {
                _head = new ImmutableNullable<T>(source[0]);
                if (source.Length > 1)
                {
                    _tail = new T[source.Length - 1];
#if NETFRAMEWORK
                    source.Slice(1).CopyTo(_tail.AsSpan());
#else
                    source[1..].CopyTo(_tail.AsSpan());
#endif
                }
                else
                {
                    _tail = default;
                }
            }
            else
            {
                _head = default;
                _tail = default;
            }
        }

        public ImmutableHeadArray(IReadOnlyList<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source.Count > 0)
            {
                _head = new ImmutableNullable<T>(source[0]);
                if (source.Count > 1)
                {
                    _tail = new T[source.Count - 1];
                    for (var i = 1; i < source.Count; ++i)
                    {
                        _tail[i - 1] = source[i];
                    }
                }
                else
                {
                    _tail = default;
                }
            }
            else
            {
                _head = default;
                _tail = default;
            }
        }

        public ImmutableHeadArray<T> Append(in T appendix)
        {
            if (_head.HasValue)
            {
                if (_tail is null)
                {
                    return new ImmutableHeadArray<T>(in _head.Value, new [] { appendix });
                }
                return new ImmutableHeadArray<T>(in _head.Value, ArrayAppend(_tail, in appendix));
            }
            return new ImmutableHeadArray<T>(in appendix);
        }

        public ImmutableHeadArray<T> Append(in ImmutableHeadArray<T> appendix)
        {
            if (appendix.IsEmpty)
            {
                return this;
            }
            if (_head.HasValue)
            {
                if (_tail is null)
                {
                    return new ImmutableHeadArray<T>(in _head.Value, appendix.ToArray());
                }
                return new ImmutableHeadArray<T>(in _head.Value, ArrayAppend(_tail, appendix));
            }
            return appendix;
        }

        public int CopyTo(Span<T> buffer)
        {
            var l = Length;
            if (l > buffer.Length)
            {
                throw new ArgumentException($"Unable to copy immutable head array contents to the provided buffer as array contains {l} elements and buffer size is {buffer.Length}.");
            }
            if (_head.HasValue)
            {
                buffer[0] = _head.Value;
            }
            if (!(_tail is null))
            {
                for (var i = 0; i < _tail.Length; ++i)
                {
                    buffer[i + 1] = _tail[i];
                }
            }
            return l;
        }

        public unsafe Enumerator GetEnumerator()
            => new Enumerator(Unsafe.AsPointer(ref this));

        public ImmutableHeadArray<TResult> Map<TResult>(Mapping<TResult> mapping)
            where TResult : unmanaged
        {
            if (_head.HasValue)
            {
                if (!(_tail is null))
                {
                    return new ImmutableHeadArray<TResult>(mapping(_head.Value), ArrayMap(_tail, mapping));
                }
                return new ImmutableHeadArray<TResult>(mapping(_head.Value));
            }
            return default;
        }

        public ImmutableHeadArray<TResult> Map<TResult>(IndexedMapping<TResult> mapping)
            where TResult : unmanaged
        {
            if (_head.HasValue)
            {
                if (!(_tail is null))
                {
                    return new ImmutableHeadArray<TResult>(mapping(_head.Value, 0), ArrayMapIndexed(_tail, mapping));
                }
                return new ImmutableHeadArray<TResult>(mapping(_head.Value, 0));
            }
            return default;
        }

        public ImmutableHeadArray<T> Pop(out T value)
        {
            if (_head.HasValue)
            {
                if (_tail is null)
                {
                    value = _head.Value;
                    return default;
                }
                if (_tail.Length == 1)
                {
                    value = _tail[0];
                    return new ImmutableHeadArray<T>(in _head.Value);
                }
                return new ImmutableHeadArray<T>(in _head.Value, ArrayPop(_tail, out value));
            }
            throw new InvalidOperationException("Unable to pop item from empty array.");
        }

        public ImmutableHeadArray<T> Pop()
            => Pop(out var _);

        public ImmutableHeadArray<T> Prepend(in T prefix)
            => new ImmutableHeadArray<T>(in prefix).Append(in this);

        public ImmutableHeadArray<T> Prepend(in ImmutableHeadArray<T> prefix)
            => prefix.Append(in this);

        public bool SequenceEqual(in ImmutableHeadArray<T> other, IEqualityComparer<T>? equalityComparer = default)
        {
            if (Length != other.Length)
            {
                return false;
            }
            var eq = equalityComparer ?? EqualityComparer<T>.Default;
            if (_head.HasValue)
            {
                if (!eq.Equals(_head.Value, other._head.Value))
                {
                    return false;
                }
                if (_tail is null)
                {
                    return other._tail is null;
                }
                for (var i = 0; i < _tail.Length; ++i)
                {
                    if (!eq.Equals(_tail[i], other._tail![i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool SequenceEqual(IEnumerable<T> other, IEqualityComparer<T>? equalityComparer = default)
        {
            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            var eq = equalityComparer ?? EqualityComparer<T>.Default;
            var selfEnumerator = GetEnumerator();
            var otherEnumerator = other.GetEnumerator();
            while (true)
            {
                var selfNext = selfEnumerator.MoveNext();
                var otherNext = otherEnumerator.MoveNext();
                if (!selfNext)
                {
                    return !otherNext;
                }
                if (!otherNext)
                {
                    return false;
                }
                if (!eq.Equals(selfEnumerator.Current, otherEnumerator.Current))
                {
                    return false;
                }
            }
        }

        public ImmutableHeadArray<T> Unshift(out T value)
        {
            if (_head.HasValue)
            {
                value = _head.Value;
                if (_tail is null)
                {
                    return default;
                }
                if (_tail.Length == 1)
                {
                    return new ImmutableHeadArray<T>(in _tail[0]);
                }
                var newTail = ArrayUnshift(_tail, out var newHead);
                return new ImmutableHeadArray<T>(in newHead, newTail);
            }
            throw new InvalidOperationException("Unable to unshift item from empty array.");
        }

        public ImmutableHeadArray<T> Unshift()
            => Unshift(out var _);

        public T[] ToArray()
        {
            if (_head.HasValue)
            {
                if (_tail is null)
                {
                    return new T[] { _head.Value };
                }
                return ArrayPrepend(_tail, in _head.Value);
            }
            return new T[0];
        }
    }
}