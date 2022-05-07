using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Collections
{
    [Obsolete("Error prone and must be reevaluated")]
    public unsafe struct TinyImmutableArray<T> : IReadOnlyList<T>
    {
        public ref struct Enumerator
        {
            int _index;

            readonly int _count;

            readonly void* _ptrSource;

            ref TinyImmutableArray<T> Source => ref Unsafe.AsRef<TinyImmutableArray<T>>(_ptrSource);

            public T Current
            {
                get
                {
                    if (-1 == _index)
                    {
                        return default!;
                    }
                    return Source[_index];
                }
            }

            internal Enumerator(int index, int count, void* ptrSource)
            {
                _index = index;
                _count = count;
                _ptrSource = ptrSource;
            }

            public bool MoveNext()
            {
                if (_index + 1 < _count)
                {
                    ++_index;
                    return true;
                }
                return false;
            }
        }

        public ref struct Builder
        {
            bool _hasFirst;

            T _first;

            bool _hasSecond;

            T _second;

            bool _hasThird;

            T _third;

            List<T>? _list;

            public void Add(T value)
            {
                if (_hasFirst)
                {
                    if (_hasSecond)
                    {
                        if (_hasThird)
                        {
                            (_list ??= new List<T>()).Add(value);
                        }
                        else
                        {
                            _hasThird = true;
                            _third = value;
                        }
                    }
                    else
                    {
                        _hasSecond = true;
                        _second = value;
                    }
                }
                else
                {
                    _hasFirst = true;
                    _first = value;
                }
            }

            public TinyImmutableArray<T> Build()
                => new(_hasFirst, _first, _hasSecond, _second, _hasThird, _third, _list?.ToArray());
        }

        readonly bool _hasFirst;

        readonly T _first;

        readonly bool _hasSecond;

        readonly T _second;

        readonly bool _hasThird;

        readonly T _third;

        readonly T[]? _array;

        public int Count
        {
            get
            {
                if (!_hasFirst)
                {
                    return 0;
                }
                if (!_hasSecond)
                {
                    return 1;
                }
                if (!_hasThird)
                {
                    return 2;
                }
                if (_array is null)
                {
                    return 3;
                }
                return 3 + _array.Length;
            }
        }

        public T this[int index] => index switch
        {
            0 => _hasFirst ? _first : throw new IndexOutOfRangeException(),
            1 => _hasSecond ? _second : throw new IndexOutOfRangeException(),
            2 => _hasThird ? _third : throw new IndexOutOfRangeException(),
            _ => _array is null ? throw new IndexOutOfRangeException() : _array[index - 3]
        };

        internal TinyImmutableArray(
            bool hasFirst,
            T first,
            bool hasSecond,
            T second,
            bool hasThird,
            T third,
            T[]? array)
        {
            _hasFirst = hasFirst;
            _first = first;
            _hasSecond = hasSecond;
            _second = second;
            _hasThird = hasThird;
            _third = third;
            _array = array;
        }

        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => Enumerate();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Enumerate();

        private IEnumerator<T> Enumerate()
        {
            if (!_hasFirst)
            {
                yield break;
            }
            yield return _first;
            if (!_hasSecond)
            {
                yield break;
            }
            yield return _second;
            if (!_hasThird)
            {
                yield break;
            }
            yield return _third;
            if (_array is null)
            {
                yield break;
            }
            foreach (var item in _array)
            {
                yield return item;
            }
        }

        public Enumerator GetEnumerator() => new(-1, Count, Unsafe.AsPointer(ref this));
    }
}