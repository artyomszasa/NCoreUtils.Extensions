using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NCoreUtils.Collections;

public sealed class ArrayPoolList<T> : IList<T>, IList, IReadOnlyList<T>, IDisposable
{
    internal const int MaxArrayLength = 0X7FEFFFFF;

    private static bool IsCompatibleObject(object? value)
    {
        // Non-null values are fine.  Only accept nulls if T is a class or Nullable<U>.
        // Note that default(T) is not equal to null for value types except when T is Nullable<U>.
        return (value is T) || (value == null && default(T) == null);
    }

    private static bool IsReferenceOrContainsReferences()
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        return true;
#else
        return RuntimeHelpers.IsReferenceOrContainsReferences<T>();
#endif
    }

    private const int DefaultCapacity = 4;

    private readonly ArrayPool<T> _pool;

    private int _isDisposed;

    private int _version;

    internal T[] _items;

    internal int _size;

    private static readonly T[] _emptyArray = [];

    public ArrayPoolList(ArrayPool<T>? pool = default)
    {
        _pool = pool ?? ArrayPool<T>.Shared;
        _items = _emptyArray;
    }

    public ArrayPoolList(int capacity, ArrayPool<T>? pool = default)
    {
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }
        _pool = pool ?? ArrayPool<T>.Shared;
        _items = capacity == 0
            ? _emptyArray
            : _pool.Rent(capacity);
    }

    public ArrayPoolList(IEnumerable<T> collection, ArrayPool<T>? pool = default)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }
        _pool = pool ?? ArrayPool<T>.Shared;

        if (collection is ICollection<T> c)
        {
            int count = c.Count;
            if (count == 0)
            {
                _items = _emptyArray;
            }
            else
            {
                _items = _pool.Rent(count);
                c.CopyTo(_items, 0);
                _size = count;
            }
        }
        else if (collection is IReadOnlyList<T> l)
        {
            int count = l.Count;
            if (count == 0)
            {
                _items = _emptyArray;
            }
            else
            {
                _items = _pool.Rent(count);
                for (var i = 0; i < count; ++i)
                {
                    _items[i] = l[i];
                }
                _size = count;
            }
        }
        else
        {
            _items = _emptyArray;
            foreach (var item in collection)
            {
                Add(item);
            }
        }
    }

    public ArrayPool<T> Pool
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _pool;
    }

    public int Capacity
    {
        get => _items.Length;
        set
        {
            if (value < _size)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (value != _items.Length)
            {
                var currentItems = _items;
                if (value > 0)
                {
                    T[] newItems = _pool.Rent(value);
                    if (_size > 0)
                    {
                        Array.Copy(_items, newItems, _size);
                    }
                    _items = newItems;
                }
                else
                {
                    _items = _emptyArray;
                }
                if (currentItems.Length != 0)
                {
                    _pool.Return(currentItems);
                }
            }
        }
    }

    public int Count => _size;

    public bool IsDisposed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 0 != Interlocked.CompareExchange(ref _isDisposed, 0, 0);
    }

    bool IList.IsFixedSize => false;

    bool ICollection<T>.IsReadOnly => false;

    bool IList.IsReadOnly => false;

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => this;

    public T this[int index]
    {
        get
        {
            if ((uint)index >= (uint)_size)
            {
                throw new IndexOutOfRangeException();
            }
            return _items[index];
        }

        set
        {
            if ((uint)index >= (uint)_size)
            {
                throw new IndexOutOfRangeException();
            }
            _items[index] = value;
            _version++;
        }
    }

    object? IList.this[int index]
    {
        get => this[index];
        set
        {
            this[index] = (T)value!;
        }
    }

    private void ThrowIfDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(ArrayPoolList<T>));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        ThrowIfDisposed();
        _version++;
        T[] array = _items;
        int size = _size;
        if ((uint)size < (uint)array.Length)
        {
            _size = size + 1;
            array[size] = item;
        }
        else
        {
            AddWithResize(item);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddWithResize(T item)
    {
        int size = _size;
        EnsureCapacityCore(size + 1);
        _size = size + 1;
        _items[size] = item;
    }

    int IList.Add(object? item)
    {
        Add((T)item!);
        return Count - 1;
    }

    public void AddRange(IEnumerable<T> collection)
        => InsertRange(_size, collection);

    public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (_size - index < count)
        {
            throw new ArgumentException($"'{nameof(index)}' and/or '{nameof(count)}' are out of range.");
        }
        return Array.BinarySearch(_items, index, count, item, comparer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int BinarySearch(T item)
        => BinarySearch(0, Count, item, null);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int BinarySearch(T item, IComparer<T>? comparer)
        => BinarySearch(0, Count, item, comparer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearNoCheck()
    {
        _version++;
        if (IsReferenceOrContainsReferences())
        {
            int size = _size;
            _size = 0;
            if (size > 0)
            {
                Array.Clear(_items, 0, size); // Clear the elements so that the gc can reclaim the references.
            }
        }
        else
        {
            _size = 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        ThrowIfDisposed();
        ClearNoCheck();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(T item)
        => _size != 0 && IndexOf(item) != -1;

    bool IList.Contains(object? item)
    {
        if (IsCompatibleObject(item))
        {
            return Contains((T)item!);
        }
        return false;
    }

    public ArrayPoolList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter, ArrayPool<TOutput>? pool = default)
    {
        if (converter is null)
        {
            throw new ArgumentNullException(nameof(converter));
        }
        var list = new ArrayPoolList<TOutput>(_size, pool);
        for (int i = 0; i < _size; i++)
        {
            list._items[i] = converter(_items[i]);
        }
        list._size = _size;
        return list;
    }

    public void CopyTo(T[] array)
        => CopyTo(array, 0);

    void ICollection.CopyTo(Array array, int arrayIndex)
    {
        if ((array != null) && (array.Rank != 1))
        {
            throw new ArgumentException($"Parameter '{nameof(array)}' must be non-null single dimension array.");
        }

        // Array.Copy will check for NULL.
        Array.Copy(_items, 0, array!, arrayIndex, _size);
    }

    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        if (_size - index < count)
        {
            throw new ArgumentException($"'{nameof(index)}' and/or '{nameof(count)}' are out of range.");
        }
        Array.Copy(_items, index, array, arrayIndex, count);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(_items, 0, array, arrayIndex, _size);
    }

    /// <summary>
    /// Disposes the object but instead of returning the underlying array to the pool returns it to the caller.
    /// Caller takes responsibility for returning array to the pool.
    /// </summary>
    public ArraySegment<T> Disown()
    {
        if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
        {
            var items = _items;
            var size = _size;
            _size = 0;
            _items = _emptyArray;
            return new ArraySegment<T>(items, 0, size);
        }
        throw new ObjectDisposedException(nameof(ArrayPool<T>));
    }

    public void Dispose()
    {
        if (0 == Interlocked.CompareExchange(ref _isDisposed, 1, 0))
        {
            ClearNoCheck();
            Capacity = 0;
        }
    }

    /// <summary>
    /// Ensures that the capacity of this list is at least the specified <paramref name="capacity"/>.
    /// If the current capacity of the list is less than specified <paramref name="capacity"/>,
    /// the capacity is increased by continuously twice current capacity until it is at least the specified <paramref name="capacity"/>.
    /// </summary>
    /// <param name="capacity">The minimum capacity to ensure.</param>
    public int EnsureCapacity(int capacity)
    {
        ThrowIfDisposed();
        if (capacity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }
        if (_items.Length < capacity)
        {
            EnsureCapacityCore(capacity);
            _version++;
        }

        return _items.Length;
    }

    /// <summary>
    /// Increase the capacity of this list to at least the specified <paramref name="capacity"/> by continuously twice current capacity.
    /// </summary>
    /// <param name="capacity">The minimum capacity to ensure.</param>
    private void EnsureCapacityCore(int capacity)
    {
        if (_items.Length < capacity)
        {
            int newcapacity = _items.Length == 0 ? DefaultCapacity : 2 * _items.Length;

            // Allow the list to grow to maximum possible capacity (~2G elements) before encountering overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newcapacity > MaxArrayLength)
            {
                newcapacity = MaxArrayLength;
            }

            // If the computed capacity is still less than specified, set to the original argument.
            // Capacities exceeding MaxArrayLength will be surfaced as OutOfMemoryException by Array.Resize.
            if (newcapacity < capacity)
            {
                newcapacity = capacity;
            }

            Capacity = newcapacity;
        }
    }

    public bool Exists(Predicate<T> match)
        => FindIndex(match) != -1;

    [return: MaybeNull]
    public T Find(Predicate<T> match)
    {
        if (match is null)
        {
            throw new ArgumentNullException(nameof(match));
        }

        for (int i = 0; i < _size; i++)
        {
            if (match(_items[i]))
            {
                return _items[i];
            }
        }
        return default!;
    }

    public int FindIndex(Predicate<T> match)
        => FindIndex(0, _size, match);

    public int FindIndex(int startIndex, Predicate<T> match)
        => FindIndex(startIndex, _size - startIndex, match);

    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        if ((uint)startIndex > (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }
        if (count < 0 || startIndex > _size - count)
        {
            throw new ArgumentException($"'{nameof(startIndex)}' and/or '{nameof(count)}' are out of range.");
        }
        if (match is null)
        {
            throw new ArgumentNullException(nameof(match));
        }
        int endIndex = startIndex + count;
        for (int i = startIndex; i < endIndex; i++)
        {
            if (match(_items[i])) return i;
        }
        return -1;
    }

    [return: MaybeNull]
    public T FindLast(Predicate<T> match)
    {
        if (match is null)
        {
            throw new ArgumentNullException(nameof(match));
        }
        for (int i = _size - 1; i >= 0; i--)
        {
            if (match(_items[i]))
            {
                return _items[i];
            }
        }
        return default;
    }

    public int FindLastIndex(Predicate<T> match)
        => FindLastIndex(_size - 1, _size, match);

    public int FindLastIndex(int startIndex, Predicate<T> match)
        => FindLastIndex(startIndex, startIndex + 1, match);

    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        if (match is null)
        {
            throw new ArgumentNullException(nameof(match));
        }
        if (_size == 0)
        {
            // Special case for 0 length List
            if (startIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
        }
        else
        {
            // Make sure we're not out of range
            if ((uint)startIndex >= (uint)_size)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
        }
        // 2nd have of this also catches when startIndex == MAXINT, so MAXINT - 0 + 1 == -1, which is < 0.
        if (count < 0 || startIndex - count + 1 < 0)
        {
            throw new ArgumentException($"'{nameof(startIndex)}' and/or '{nameof(count)}' are out of range.");
        }
        int endIndex = startIndex - count;
        for (int i = startIndex; i > endIndex; i--)
        {
            if (match(_items[i]))
            {
                return i;
            }
        }
        return -1;
    }

    public void ForEach(Action<T> action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        int version = _version;

        for (int i = 0; i < _size; i++)
        {
            if (version != _version)
            {
                break;
            }
            action(_items[i]);
        }

        if (version != _version)
        {
            throw new InvalidOperationException("Collection has changed.");
        }
    }

    public Enumerator GetEnumerator()
        => new(this);

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
        => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public ArrayPoolList<T> GetRange(int index, int count)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (_size - index < count)
        {
            throw new ArgumentException($"'{nameof(index)}' and/or '{nameof(count)}' are out of range.");
        }

        ArrayPoolList<T> list = new(count, _pool);
        Array.Copy(_items, index, list._items, 0, count);
        list._size = count;
        return list;
    }

    public int IndexOf(T item)
        => Array.IndexOf(_items, item, 0, _size);

    int IList.IndexOf(object? item)
    {
        if (IsCompatibleObject(item))
        {
            return IndexOf((T)item!);
        }
        return -1;
    }

    public int IndexOf(T item, int index)
    {
        if (index > _size)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        return Array.IndexOf(_items, item, index, _size - index);
    }

    public int IndexOf(T item, int index, int count)
    {
        if (index > _size)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0 || index > _size - count)
        {
            throw new ArgumentException($"'{nameof(index)}' and/or '{nameof(count)}' are out of range.");
        }
        return Array.IndexOf(_items, item, index, count);
    }

    public void Insert(int index, T item)
    {
        ThrowIfDisposed();
        if ((uint)index > (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (_size == _items.Length)
        {
            EnsureCapacityCore(_size + 1);
        }
        if (index < _size)
        {
            Array.Copy(_items, index, _items, index + 1, _size - index);
        }
        _items[index] = item;
        _size++;
        _version++;
    }

    void IList.Insert(int index, object? item)
    {
        Insert(index, (T)item!);
    }

    public void InsertRange(int index, IEnumerable<T> collection)
    {
        ThrowIfDisposed();
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }
        if ((uint)index > (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (collection is ICollection<T> c)
        {
            int count = c.Count;
            if (count > 0)
            {
                EnsureCapacityCore(_size + count);
                if (index < _size)
                {
                    Array.Copy(_items, index, _items, index + count, _size - index);
                }

                // If we're inserting a List into itself, we want to be able to deal with that.
                if (this == c)
                {
                    // Copy first part of _items to insert location
                    Array.Copy(_items, 0, _items, index, index);
                    // Copy last part of _items back to inserted location
                    Array.Copy(_items, index + count, _items, index * 2, _size - index);
                }
                else
                {
                    c.CopyTo(_items, index);
                }
                _size += count;
            }
        }
        else
        {
            foreach (var item in collection)
            {
                Insert(index++, item);
            }
        }
        _version++;
    }

    public int LastIndexOf(T item)
    {
        if (_size == 0)
        {
            return -1;
        }
        else
        {
            return LastIndexOf(item, _size - 1, _size);
        }
    }

    public int LastIndexOf(T item, int index)
    {
        if (index >= _size)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        return LastIndexOf(item, index, index + 1);
    }

    public int LastIndexOf(T item, int index, int count)
    {
        if ((Count != 0) && (index < 0))
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if ((Count != 0) && (count < 0))
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (_size == 0)
        {
            return -1;
        }
        if (index >= _size)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count > index + 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        return Array.LastIndexOf(_items, item, index, count);
    }

    public bool Remove(T item)
    {
        ThrowIfDisposed();
        int index = IndexOf(item);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    void IList.Remove(object? item)
    {
        if (IsCompatibleObject(item))
        {
            Remove((T)item!);
        }
    }

    public int RemoveAll(Predicate<T> match)
    {
        ThrowIfDisposed();
        if (match is null)
        {
            throw new ArgumentNullException(nameof(match));
        }
        int freeIndex = 0;
        while (freeIndex < _size && !match(_items[freeIndex])) freeIndex++;
        if (freeIndex >= _size)
        {
            return 0;
        }
        int current = freeIndex + 1;
        while (current < _size)
        {
            while (current < _size && match(_items[current])) current++;
            if (current < _size)
            {
                _items[freeIndex++] = _items[current++];
            }
        }
        if (IsReferenceOrContainsReferences())
        {
            Array.Clear(_items, freeIndex, _size - freeIndex);
        }
        int result = _size - freeIndex;
        _size = freeIndex;
        _version++;
        return result;
    }

    public void RemoveAt(int index)
    {
        ThrowIfDisposed();
        if ((uint)index >= (uint)_size)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        _size--;
        if (index < _size)
        {
            Array.Copy(_items, index + 1, _items, index, _size - index);
        }
        if (IsReferenceOrContainsReferences())
        {
            _items[_size] = default!;
        }
        _version++;
    }

    public void RemoveRange(int index, int count)
    {
        ThrowIfDisposed();
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (_size - index < count)
        {
            throw new ArgumentException($"'{nameof(index)}' and/or '{nameof(count)}' are out of range.");
        }
        if (count > 0)
        {
            _size -= count;
            if (index < _size)
            {
                Array.Copy(_items, index + count, _items, index, _size - index);
            }

            _version++;
            if (IsReferenceOrContainsReferences())
            {
                Array.Clear(_items, _size, count);
            }
        }
    }

    public void Reverse()
        => Reverse(0, Count);

    public void Reverse(int index, int count)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (_size - index < count)
        {
            throw new ArgumentException($"'{nameof(index)}' and/or '{nameof(count)}' are out of range.");
        }
        if (count > 1)
        {
            Array.Reverse(_items, index, count);
        }
        _version++;
    }

    public void Sort(IComparer<T>? comparer = default)
        => Sort(0, Count, comparer);

    public void Sort(int index, int count, IComparer<T>? comparer = default)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (_size - index < count)
        {
            throw new ArgumentException($"'{nameof(index)}' and/or '{nameof(count)}' are out of range.");
        }
        if (count > 1)
        {
            Array.Sort(_items, index, count, comparer);
        }
        _version++;
    }

    public T[] ToArray()
    {
        if (_size == 0)
        {
            return _emptyArray;
        }
        T[] array = new T[_size];
        Array.Copy(_items, array, _size);
        return array;
    }

    public void TrimExcess()
    {
        ThrowIfDisposed();
        int threshold = (int)(_items.Length * 0.9);
        if (_size < threshold)
        {
            Capacity = _size;
        }
    }

    public bool TrueForAll(Predicate<T> match)
    {
        if (match is null)
        {
            throw new ArgumentNullException(nameof(match));
        }
        for (int i = 0; i < _size; i++)
        {
            if (!match(_items[i]))
            {
                return false;
            }
        }
        return true;
    }

    public struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private readonly ArrayPoolList<T> _list;

        private int _index;

        private readonly int _version;

        private T _current;

        internal Enumerator(ArrayPoolList<T> list)
        {
            _list = list;
            _index = 0;
            _version = list._version;
            _current = default!;
        }

        public readonly void Dispose() { }

        public bool MoveNext()
        {
            ArrayPoolList<T> localList = _list;
            if (_version == localList._version && ((uint)_index < (uint)localList._size))
            {
                _current = localList._items[_index];
                _index++;
                return true;
            }
            return MoveNextRare();
        }

        private bool MoveNextRare()
        {
            if (_version != _list._version)
            {
                throw new InvalidOperationException("Collection has changed.");
            }
            _index = _list._size + 1;
            _current = default!;
            return false;
        }

        public readonly T Current => _current!;

        readonly object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _list._size + 1)
                {
                    throw new InvalidOperationException("Should never happen");
                }
                return Current;
            }
        }

        void IEnumerator.Reset()
        {
            if (_version != _list._version)
            {
                throw new InvalidOperationException("Should never happen");
            }
            _index = 0;
            _current = default!;
        }
    }
}