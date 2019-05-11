using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NCoreUtils.Collections;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class EnumerableTests
    {
        sealed class HiddenArrayEnumerator<T> : IEnumerator<T>
        {
            readonly T[] _source;

            int _index = -1;

            object IEnumerator.Current => Current;

            public T Current
            {
                get
                {
                    if (_index < 0 || _index >= _source.Length)
                    {
                        return default(T);
                    }
                    return _source[_index];
                }
            }

            public HiddenArrayEnumerator(T[] source) => _source = source ?? throw new ArgumentNullException(nameof(source));

            public bool MoveNext()
            {
                if (_index > _source.Length)
                {
                    return false;
                }
                ++_index;
                return _index < _source.Length;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose() { }
        }

        sealed class HiddenArray<T> : IEnumerable<T>
        {
            public T[] Source { get; }

            public HiddenArray(T[] source) => Source = source ?? throw new ArgumentNullException(nameof(source));

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public IEnumerator<T> GetEnumerator() => new HiddenArrayEnumerator<T>(Source);
        }

        sealed class AsCollection<T> : ICollection<T>
        {
            readonly List<T> _list;

            public int Count => _list.Count;

            public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

            public AsCollection(IEnumerable<T> values) => _list = new List<T>(values);

            public AsCollection() => _list = new List<T>();

            public void Add(T item) => _list.Add(item);

            public void Clear() => _list.Clear();

            public bool Contains(T item) => _list.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

            public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();

            public bool Remove(T item) => _list.Remove(item);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        sealed class IndexAccessOnlyList<T> : IList<T>
        {
            readonly List<T> _list;

            public T this[int index] { get => _list[index]; set => _list[index] = value; }

            public int Count => _list.Count;

            public bool IsReadOnly => ((IList<T>)_list).IsReadOnly;

            public IndexAccessOnlyList(IEnumerable<T> values) => _list = new List<T>(values);

            public IndexAccessOnlyList() => _list = new List<T>();

            public void Add(T item) => _list.Add(item);

            public void Clear() => _list.Clear();

            public bool Contains(T item) => _list.Contains(item);

            public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

            public IEnumerator<T> GetEnumerator() => throw new InvalidOperationException("GetEnumerator should not be called!");

            public int IndexOf(T item) => _list.IndexOf(item);

            public void Insert(int index, T item) => _list.Insert(index, item);

            public bool Remove(T item) => _list.Remove(item);

            public void RemoveAt(int index) => _list.RemoveAt(index);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        sealed class IndexAccessOnlyReadOnlyList<T> : IReadOnlyList<T>
        {
            readonly List<T> _list;

            public T this[int index] => _list[index];

            public int Count => _list.Count;

            public IndexAccessOnlyReadOnlyList() : this(System.Linq.Enumerable.Empty<T>()) { }

            public IndexAccessOnlyReadOnlyList(IEnumerable<T> values) => _list = new List<T>(values);

            public IEnumerator<T> GetEnumerator() => throw new InvalidOperationException("GetEnumerator should not be called!");

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        sealed class Box<T> : IEquatable<Box<T>>
        {
            static readonly IEqualityComparer<T> _eqComparer = EqualityComparer<T>.Default;

            public T Value { get; }

            public Box(T value) => Value = value;

            public bool Equals(Box<T> other) => other != null && _eqComparer.Equals(Value, other.Value);

            public override bool Equals(object obj) => Equals(obj as Box<T>);

            public override int GetHashCode() => _eqComparer.GetHashCode(Value);
        }

        [Fact]
        public void TryGetFirst()
        {
            int value;
            // null parameter case
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.TryGetFirst(null, out value));

            // empty seq case
            value = 10; // non default
            Assert.False(new int[0].TryGetFirst(out value));
            Assert.Equal(default(int), value);
            value = 10; // non default
            Assert.False(new HiddenArray<int>(new int[0]).TryGetFirst(out value));
            Assert.Equal(default(int), value);

            // non-empty case
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var seq = new HiddenArray<int>(array);
            Assert.True(array.TryGetFirst(out value));
            Assert.Equal(1, value);
            Assert.True(seq.TryGetFirst(out value));
            Assert.Equal(1, value);

            // list value case
            {
                var list0 = new IndexAccessOnlyList<int>();
                var list = new IndexAccessOnlyList<int>{ 1, 2, 3, 4 };
                value = 10; // non default
                Assert.False(list0.TryGetFirst(out value));
                Assert.Equal(default(int), value);
                Assert.True(list.TryGetFirst(out value));
                Assert.Equal(1, value);
            }

            // read-only list value case
            {
                var list0 = new IndexAccessOnlyReadOnlyList<int>();
                var list = new IndexAccessOnlyReadOnlyList<int>(new [] { 1, 2, 3, 4 });
                value = 10; // non default
                Assert.False(list0.TryGetFirst(out value));
                Assert.Equal(default(int), value);
                Assert.True(list.TryGetFirst(out value));
                Assert.Equal(1, value);
            }
        }

        [Fact]
        public void TryGetFirstWithPredicate()
        {
            int value;
            var array0 = new int[0];
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var seq0 = new HiddenArray<int>(array0);
            var seq = new HiddenArray<int>(array);
            var list0 = new IndexAccessOnlyList<int>();
            var list = new IndexAccessOnlyList<int>{ 1, 2, 3, 4, 5, 6 };
            var rolist0 = new IndexAccessOnlyReadOnlyList<int>();
            var rolist = new IndexAccessOnlyReadOnlyList<int>(array);

            Func<int, bool> predicate2 = i => i % 2 == 0;
            Func<int, bool> predicate5 = i => i % 5 == 0;
            Func<int, bool> predicate9 = i => i % 9 == 0;

            // null parameter case
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.TryGetFirst(null, predicate5, out value));
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.TryGetFirst(array, null, out value));

            // empty case
            value = 10;
            Assert.False(array0.TryGetFirst(predicate5, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(seq0.TryGetFirst(predicate5, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(list0.TryGetFirst(predicate5, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(rolist0.TryGetFirst(predicate5, out value));
            Assert.Equal(default(int), value);

            // non-matching case
            value = 10;
            Assert.False(array.TryGetFirst(predicate9, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(seq.TryGetFirst(predicate9, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(list.TryGetFirst(predicate9, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(rolist.TryGetFirst(predicate9, out value));
            Assert.Equal(default(int), value);

            // matching case
            Assert.True(array.TryGetFirst(predicate5, out value));
            Assert.Equal(5, value);
            Assert.True(seq.TryGetFirst(predicate5, out value));
            Assert.Equal(5, value);
            Assert.True(list.TryGetFirst(predicate5, out value));
            Assert.Equal(5, value);
            Assert.True(rolist.TryGetFirst(predicate5, out value));
            Assert.Equal(5, value);

            // multiple match case
            Assert.True(array.TryGetFirst(predicate2, out value));
            Assert.Equal(2, value);
            Assert.True(seq.TryGetFirst(predicate2, out value));
            Assert.Equal(2, value);
            Assert.True(list.TryGetFirst(predicate2, out value));
            Assert.Equal(2, value);
            Assert.True(rolist.TryGetFirst(predicate2, out value));
            Assert.Equal(2, value);

        }

        [Fact]
        public void TryGetSingle()
        {
            int value;
            var array0 = new int[0];
            var array1 = new int[] { 1 };
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var seq0 = new HiddenArray<int>(array0);
            var seq1 = new HiddenArray<int>(array1);
            var seq = new HiddenArray<int>(array);
            var list0 = new IndexAccessOnlyList<int>();
            var list1 = new IndexAccessOnlyList<int>{ 1 };
            var list = new IndexAccessOnlyList<int>{ 1, 2, 3, 4, 5, 6 };
            var rolist0 = new IndexAccessOnlyReadOnlyList<int>();
            var rolist1 = new IndexAccessOnlyReadOnlyList<int>(array1);
            var rolist = new IndexAccessOnlyReadOnlyList<int>(array);

            // null parameter case
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.TryGetSingle(null, out value));

            // empty case
            value = 10;
            Assert.False(array0.TryGetSingle(out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(seq0.TryGetSingle(out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(list0.TryGetSingle(out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(rolist0.TryGetSingle(out value));
            Assert.Equal(default(int), value);

            // single element case
            Assert.True(array1.TryGetSingle(out value));
            Assert.Equal(1, value);
            Assert.True(seq1.TryGetSingle(out value));
            Assert.Equal(1, value);
            Assert.True(list1.TryGetSingle(out value));
            Assert.Equal(1, value);
            Assert.True(rolist1.TryGetSingle(out value));
            Assert.Equal(1, value);

            // multiple element case
            Assert.Throws<InvalidOperationException>(() => array.TryGetSingle(out value));
            Assert.Throws<InvalidOperationException>(() => seq.TryGetSingle(out value));
            Assert.Throws<InvalidOperationException>(() => list.TryGetSingle(out value));
            Assert.Throws<InvalidOperationException>(() => rolist.TryGetSingle(out value));
        }

        [Fact]
        public void TryGetSingleWithPredicate()
        {
            int value;
            var array0 = new int[0];
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var seq0 = new HiddenArray<int>(array0);
            var seq = new HiddenArray<int>(array);
            var list0 = new IndexAccessOnlyList<int>();
            var list = new IndexAccessOnlyList<int>{ 1, 2, 3, 4, 5, 6 };
            var rolist0 = new IndexAccessOnlyReadOnlyList<int>();
            var rolist = new IndexAccessOnlyReadOnlyList<int>(array);

            Func<int, bool> predicate2 = i => i % 2 == 0;
            Func<int, bool> predicate5 = i => i % 5 == 0;
            Func<int, bool> predicate9 = i => i % 9 == 0;

            // null parameter case
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.TryGetSingle(null, predicate5, out value));
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.TryGetSingle(array, null, out value));

            // empty case
            value = 10;
            Assert.False(array0.TryGetSingle(predicate5, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(seq0.TryGetSingle(predicate5, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(list0.TryGetSingle(predicate5, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(rolist0.TryGetSingle(predicate5, out value));
            Assert.Equal(default(int), value);

            // non-matching case
            value = 10;
            Assert.False(array.TryGetSingle(predicate9, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(seq.TryGetSingle(predicate9, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(list.TryGetSingle(predicate9, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(rolist.TryGetSingle(predicate9, out value));
            Assert.Equal(default(int), value);

            // single match case
            Assert.True(array.TryGetSingle(predicate5, out value));
            Assert.Equal(5, value);
            Assert.True(seq.TryGetSingle(predicate5, out value));
            Assert.Equal(5, value);
            Assert.True(list.TryGetSingle(predicate5, out value));
            Assert.Equal(5, value);
            Assert.True(rolist.TryGetSingle(predicate5, out value));
            Assert.Equal(5, value);

            // multiple match case
            Assert.Throws<InvalidOperationException>(() => array.TryGetSingle(predicate2, out value));
            Assert.Throws<InvalidOperationException>(() => seq.TryGetSingle(predicate2, out value));
            Assert.Throws<InvalidOperationException>(() => list.TryGetSingle(predicate2, out value));
            Assert.Throws<InvalidOperationException>(() => rolist.TryGetSingle(predicate2, out value));
        }

        [Fact]
        public void TryGetLast()
        {
            int value;
            // null parameter case
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.TryGetLast(null, out value));

            // empty seq case
            value = 10; // non default
            Assert.False(new int[0].TryGetLast(out value));
            Assert.Equal(default(int), value);
            value = 10; // non default
            Assert.False(new HiddenArray<int>(new int[0]).TryGetLast(out value));
            Assert.Equal(default(int), value);

            // non-empty case
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var seq = new HiddenArray<int>(array);
            Assert.True(array.TryGetLast(out value));
            Assert.Equal(6, value);
            Assert.True(seq.TryGetLast(out value));
            Assert.Equal(6, value);

            // list value case
            {
                var list0 = new IndexAccessOnlyList<int>();
                var list = new IndexAccessOnlyList<int>{ 1, 2, 3, 4 };
                value = 10; // non default
                Assert.False(list0.TryGetLast(out value));
                Assert.Equal(default(int), value);
                Assert.True(list.TryGetLast(out value));
                Assert.Equal(4, value);
            }

            // read-only list value case
            {
                var list0 = new IndexAccessOnlyReadOnlyList<int>();
                var list = new IndexAccessOnlyReadOnlyList<int>(new [] { 1, 2, 3, 4 });
                value = 10; // non default
                Assert.False(list0.TryGetLast(out value));
                Assert.Equal(default(int), value);
                Assert.True(list.TryGetLast(out value));
                Assert.Equal(4, value);
            }
        }

        [Fact]
        public void TryGetLastWithPredicate()
        {
            int value;
            var array0 = new int[0];
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var seq0 = new HiddenArray<int>(array0);
            var seq = new HiddenArray<int>(array);
            var list0 = new IndexAccessOnlyList<int>();
            var list = new IndexAccessOnlyList<int>{ 1, 2, 3, 4, 5, 6 };
            var rolist0 = new IndexAccessOnlyReadOnlyList<int>();
            var rolist = new IndexAccessOnlyReadOnlyList<int>(array);

            Func<int, bool> predicate2 = i => i % 2 == 0;
            Func<int, bool> predicate5 = i => i % 5 == 0;
            Func<int, bool> predicate9 = i => i % 9 == 0;

            // null parameter case
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.TryGetLast(null, predicate5, out value));
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.TryGetLast(array, null, out value));

            // empty case
            value = 10;
            Assert.False(array0.TryGetLast(predicate5, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(seq0.TryGetLast(predicate5, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(list0.TryGetLast(predicate5, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(rolist0.TryGetLast(predicate5, out value));
            Assert.Equal(default(int), value);

            // non-matching case
            value = 10;
            Assert.False(array.TryGetLast(predicate9, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(seq.TryGetLast(predicate9, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(list.TryGetLast(predicate9, out value));
            Assert.Equal(default(int), value);
            value = 10;
            Assert.False(rolist.TryGetLast(predicate9, out value));
            Assert.Equal(default(int), value);

            // matching case
            Assert.True(array.TryGetLast(predicate5, out value));
            Assert.Equal(5, value);
            Assert.True(seq.TryGetLast(predicate5, out value));
            Assert.Equal(5, value);
            Assert.True(list.TryGetLast(predicate5, out value));
            Assert.Equal(5, value);
            Assert.True(rolist.TryGetLast(predicate5, out value));
            Assert.Equal(5, value);

            // multiple match case
            Assert.True(array.TryGetLast(predicate2, out value));
            Assert.Equal(6, value);
            Assert.True(seq.TryGetLast(predicate2, out value));
            Assert.Equal(6, value);
            Assert.True(list.TryGetLast(predicate2, out value));
            Assert.Equal(6, value);
            Assert.True(rolist.TryGetLast(predicate2, out value));
            Assert.Equal(6, value);
        }

        [Fact]
        public void MapToArray()
        {
            var array = new int[] { 1, 2, 3, 4, 5, 6 };
            var seq = new HiddenArray<int>(array);
            var list = new IndexAccessOnlyList<int>(array);
            var rolist = new IndexAccessOnlyReadOnlyList<int>(array);
            var col = new AsCollection<int>(array);

            Func<int, int> selector = i => i * 2;

            // null parameter case
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.MapToArray(null, selector));
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.MapToArray<int, int>(array, null));

            var expected = new int[] { 2, 4, 6, 8, 10, 12 };

            Assert.Equal(expected, array.MapToArray(selector));
            Assert.Equal(expected, seq.MapToArray(selector));
            Assert.Equal(expected, list.MapToArray(selector));
            Assert.Equal(expected, rolist.MapToArray(selector));
            Assert.Equal(expected, col.MapToArray(selector));
        }

        [Fact]
        public void MinBy()
        {
            var array0 = new int[0];
            var array = new int[] { 2, 1, 3, 10, 4, 5, 6 };
            var seq0 = new HiddenArray<int>(array0);
            var seq = new HiddenArray<int>(array);
            var list0 = new IndexAccessOnlyList<int>();
            var list = new IndexAccessOnlyList<int>(array);
            var rolist0 = new IndexAccessOnlyReadOnlyList<int>();
            var rolist = new IndexAccessOnlyReadOnlyList<int>(array);

            var comparer = Comparer<int>.Default;

            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.MinBy(null, comparer));

            Assert.Throws<InvalidOperationException>(() => array0.MinBy());
            Assert.Throws<InvalidOperationException>(() => seq0.MinBy());
            Assert.Throws<InvalidOperationException>(() => list0.MinBy());
            Assert.Throws<InvalidOperationException>(() => rolist0.MinBy());

            Assert.Equal(1, array.MinBy());
            Assert.Equal(1, seq.MinBy());
            Assert.Equal(1, list.MinBy());
            Assert.Equal(1, rolist.MinBy());

            Assert.Equal(1, array.MinBy(comparer));
            Assert.Equal(1, seq.MinBy(comparer));
            Assert.Equal(1, list.MinBy(comparer));
            Assert.Equal(1, rolist.MinBy(comparer));
        }

        [Fact]
        public void MinByWithSelector()
        {
            var array0 = new Box<int>[0];
            var array = new int[] { 2, 1, 3, 10, 4, 5, 6 }.Map(i => new Box<int>(i));
            var seq0 = new HiddenArray<Box<int>>(array0);
            var seq = new HiddenArray<Box<int>>(array);
            var list0 = new IndexAccessOnlyList<Box<int>>();
            var list = new IndexAccessOnlyList<Box<int>>(array);
            var rolist0 = new IndexAccessOnlyReadOnlyList<Box<int>>();
            var rolist = new IndexAccessOnlyReadOnlyList<Box<int>>(array);

            var comparer = Comparer<int>.Default;
            Func<Box<int>, int> selector = b => b.Value;

            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.MinBy(null, selector, comparer));
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.MinBy(seq, null, comparer));

            Assert.Throws<InvalidOperationException>(() => array0.MinBy(selector));
            Assert.Throws<InvalidOperationException>(() => seq0.MinBy(selector));
            Assert.Throws<InvalidOperationException>(() => list0.MinBy(selector));
            Assert.Throws<InvalidOperationException>(() => rolist0.MinBy(selector));

            Assert.Equal(new Box<int>(1), array.MinBy(selector));
            Assert.Equal(new Box<int>(1), seq.MinBy(selector));
            Assert.Equal(new Box<int>(1), list.MinBy(selector));
            Assert.Equal(new Box<int>(1), rolist.MinBy(selector));

            Assert.Equal(new Box<int>(1), array.MinBy(selector, comparer));
            Assert.Equal(new Box<int>(1), seq.MinBy(selector, comparer));
            Assert.Equal(new Box<int>(1), list.MinBy(selector, comparer));
            Assert.Equal(new Box<int>(1), rolist.MinBy(selector, comparer));
        }

        [Fact]
        public void MaxBy()
        {
            var array0 = new int[0];
            var array = new int[] { 1, 2, 3, 10, 4, 5, 6 };
            var seq0 = new HiddenArray<int>(array0);
            var seq = new HiddenArray<int>(array);
            var list0 = new IndexAccessOnlyList<int>();
            var list = new IndexAccessOnlyList<int>(array);
            var rolist0 = new IndexAccessOnlyReadOnlyList<int>();
            var rolist = new IndexAccessOnlyReadOnlyList<int>(array);

            var comparer = Comparer<int>.Default;

            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.MaxBy(null, comparer));

            Assert.Throws<InvalidOperationException>(() => array0.MaxBy());
            Assert.Throws<InvalidOperationException>(() => seq0.MaxBy());
            Assert.Throws<InvalidOperationException>(() => list0.MaxBy());
            Assert.Throws<InvalidOperationException>(() => rolist0.MaxBy());

            Assert.Equal(10, array.MaxBy());
            Assert.Equal(10, seq.MaxBy());
            Assert.Equal(10, list.MaxBy());
            Assert.Equal(10, rolist.MaxBy());

            Assert.Equal(10, array.MaxBy(comparer));
            Assert.Equal(10, seq.MaxBy(comparer));
            Assert.Equal(10, list.MaxBy(comparer));
            Assert.Equal(10, rolist.MaxBy(comparer));
        }

        [Fact]
        public void MaxByWithSelector()
        {
            var array0 = new Box<int>[0];
            var array = new int[] { 1, 2, 3, 10, 4, 5, 6 }.Map(i => new Box<int>(i));
            var seq0 = new HiddenArray<Box<int>>(array0);
            var seq = new HiddenArray<Box<int>>(array);
            var list0 = new IndexAccessOnlyList<Box<int>>();
            var list = new IndexAccessOnlyList<Box<int>>(array);
            var rolist0 = new IndexAccessOnlyReadOnlyList<Box<int>>();
            var rolist = new IndexAccessOnlyReadOnlyList<Box<int>>(array);

            var comparer = Comparer<int>.Default;
            Func<Box<int>, int> selector = b => b.Value;

            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.MaxBy(null, selector, comparer));
            Assert.Throws<ArgumentNullException>(() => EnumerableExtensions.MaxBy(seq, null, comparer));


            Assert.Throws<InvalidOperationException>(() => array0.MaxBy(selector));
            Assert.Throws<InvalidOperationException>(() => seq0.MaxBy(selector));
            Assert.Throws<InvalidOperationException>(() => list0.MaxBy(selector));
            Assert.Throws<InvalidOperationException>(() => rolist0.MaxBy(selector));

            Assert.Equal(new Box<int>(10), array.MaxBy(selector));
            Assert.Equal(new Box<int>(10), seq.MaxBy(selector));
            Assert.Equal(new Box<int>(10), list.MaxBy(selector));
            Assert.Equal(new Box<int>(10), rolist.MaxBy(selector));

            Assert.Equal(new Box<int>(10), array.MaxBy(selector, comparer));
            Assert.Equal(new Box<int>(10), seq.MaxBy(selector, comparer));
            Assert.Equal(new Box<int>(10), list.MaxBy(selector, comparer));
            Assert.Equal(new Box<int>(10), rolist.MaxBy(selector, comparer));
        }

        [Fact]
        public void EnumeratorExtensionTests()
        {
            var source = (IReadOnlyList<int>)new int[] { 0,1,2,3,4,5,6,7,8,9 };
            List<int> l;
            using (var e = source.GetEnumerator().Select(x => x * 2))
            {
                l = e.ToList();
            }
            Assert.True(Enumerable.SequenceEqual(new int[] { 0,2,4,6,8,10,12,14,16,18 }, l));
            using (var e = source.GetEnumerator().Select((x, i) => x * i))
            {
                l = e.ToList();
            }
            Assert.Equal((IEnumerable<int>)new int[] { 0,1,4,9,16,25,36,49,64,81 }, (IEnumerable<int>)l);
            using (var e = source.GetEnumerator().Where(x => x % 2 == 0))
            {
                l = e.ToList();
            }
            Assert.True(Enumerable.SequenceEqual(new int[] { 0,2,4,6,8 }, l));
            using (var e = source.GetEnumerator().Where((_, i) => i % 2 == 0))
            {
                l = e.ToList();
            }
            Assert.True(Enumerable.SequenceEqual(new int[] { 0,2,4,6,8 }, l));

            Assert.Throws<ArgumentNullException>(() => EnumeratorExtensions.Select<int, int>(null, i => i));
            Assert.Throws<ArgumentNullException>(() => EnumeratorExtensions.Select<int, int>(null, (i, _) => i));
            Assert.Throws<ArgumentNullException>(() => EnumeratorExtensions.Select<int, int>(l.GetEnumerator(), (Func<int, int>)null));
            Assert.Throws<ArgumentNullException>(() => EnumeratorExtensions.Select<int, int>(l.GetEnumerator(), (Func<int, int, int>)null));
            Assert.Throws<ArgumentNullException>(() => EnumeratorExtensions.Where<int>(null, i => true));
            Assert.Throws<ArgumentNullException>(() => EnumeratorExtensions.Where<int>(null, (i, _) => true));
            Assert.Throws<ArgumentNullException>(() => EnumeratorExtensions.Where<int>(l.GetEnumerator(), (Func<int, bool>)null));
            Assert.Throws<ArgumentNullException>(() => EnumeratorExtensions.Where<int>(l.GetEnumerator(), (Func<int, int, bool>)null));
            Assert.Throws<ArgumentNullException>(() => EnumeratorExtensions.ToList<int>(null));
        }

        [Fact]
        public void MappingReadOnlyListTests()
        {
            var l = new MappingReadOnlyList<int, int>(new [] { 0,1,2,3,4,5,6,7,8,9 }, i => i * 2);
            Assert.Equal(2, l[1]);
            Assert.Equal(10, l[5]);
            var lcount = l.Count;
            Assert.Equal(10, lcount);
            Assert.Equal((IEnumerable<int>)new int[] { 0,2,4,6,8,10,12,14,16,18 }, (IEnumerable<int>)l);
            Assert.Throws<ArgumentNullException>(() => new MappingReadOnlyList<int, int>(null, i => i * 2));
            Assert.Throws<ArgumentNullException>(() => new MappingReadOnlyList<int, int>(new int[0], null));
        }

        [Fact]
        public void MappingReadOnlyDictionaryTests()
        {
            var d = new MappingReadOnlyDictionary<int, int, int>(new Dictionary<int, int>
            {
                { 0, 0 },
                { 1, 1 },
                { 2, 2 },
                { 3, 3 },
                { 4, 4 },
                { 5, 5 },
                { 6, 6 },
                { 7, 7 },
                { 8, 8 },
                { 9, 9 }
            }, i => i * 2);
            Assert.Equal(2, d[1]);
            Assert.Equal(10, d[5]);
            var lcount = d.Count;
            Assert.Equal(10, lcount);
            Assert.True(d.ContainsKey(1));
            Assert.True(d.TryGetValue(1, out var v));
            Assert.Equal(2, v);
            Assert.False(d.ContainsKey(11));
            Assert.False(d.TryGetValue(11, out v));
            Assert.Equal((IEnumerable<int>)new int[] { 0,1,2,3,4,5,6,7,8,9 }, d.Keys);
            Assert.Equal((IEnumerable<int>)new int[] { 0,2,4,6,8,10,12,14,16,18 }, d.Values);
            Assert.True(HashSet<int>.CreateSetComparer().Equals(new HashSet<int> { 0,1,2,3,4,5,6,7,8,9 }, new HashSet<int>(d.Select(kv => kv.Key))));
            Assert.True(HashSet<int>.CreateSetComparer().Equals(new HashSet<int> { 0,2,4,6,8,10,12,14,16,18 }, new HashSet<int>(d.Select(kv => kv.Value))));
            Assert.Throws<ArgumentNullException>(() => new MappingReadOnlyDictionary<int, int, int>(null, i => i * 2));
            Assert.Throws<ArgumentNullException>(() => new MappingReadOnlyDictionary<int, int, int>(new Dictionary<int, int>(), null));
        }
    }
}
