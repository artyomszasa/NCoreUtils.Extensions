using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using NCoreUtils.Collections;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public abstract class MultiDictionaryTests<TKey, TValue>
    {
        protected abstract TKey NextKey();

        protected abstract TValue NextValue();

        public virtual void EmptyTests()
        {
            var key = NextKey();
            var empty = new MultiValueDictionary<TKey, TValue>();
            var xvs = new List<TValue>();
            Assert.False(empty.ContainsKey(key));
            Assert.False(empty.TryGetValue(key, out var _));
            Assert.False(empty.TryGetValues(key, out var arr));
            Assert.Null(arr);
            Assert.False(empty.TryGetValues(key, xvs));
            Assert.Empty(xvs);
            var count = empty.Count;
            Assert.Equal(0, count);
            count = empty.Count();
            Assert.Equal(0, count);
            Assert.Empty(empty.Keys);
            Assert.Equal(0, empty.Remove(key));
            Assert.False(((ICollection<KeyValuePair<TKey, TValue>>)empty).Remove(new KeyValuePair<TKey, TValue>(key, default(TValue))));
            Assert.False(((ICollection<KeyValuePair<TKey, TValue>>)empty).IsReadOnly);
        }

        public virtual void BasicOperationsTest()
        {
            var data = new MultiValueDictionary<TKey, TValue>();
            var key0 = NextKey();
            var key1 = NextKey();
            var value00 = NextValue();
            var value01 = NextValue();
            var value10 = NextValue();
            var value11 = NextValue();
            TValue v;
            TValue[] vs;
            var xvs = new List<TValue>();
            // single key, single value
            data.Add(key0, value00);
            Assert.True(data.ContainsKey(key0));
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value00)));
            Assert.False(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value01)));
            Assert.True(data.TryGetValue(key0, out v));
            Assert.Equal(value00, v);
            Assert.True(data.TryGetValues(key0, out vs));
            Assert.NotNull(vs);
            Assert.Single(vs, v);
            xvs.Clear();
            Assert.True(data.TryGetValues(key0, xvs));
            Assert.Single(xvs, v);
            var count = data.Count;
            Assert.Equal(1, count);
            count = data.Count();
            Assert.Equal(1, count);
            Assert.Single(data.Keys, key0);
            data.Clear();
            Assert.Empty(data);
            // single key, two values (added separately)
            data.Add(key0, value00);
            data.Add(key0, value01);
            Assert.True(data.ContainsKey(key0));
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value00)));
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value01)));
            Assert.True(data.TryGetValue(key0, out v));
            Assert.True(EqualityComparer<TValue>.Default.Equals(value00, v) || EqualityComparer<TValue>.Default.Equals(value01, v));
            Assert.True(data.TryGetValues(key0, out vs));
            Assert.NotNull(vs);
            Assert.True(HashSet<TValue>.CreateSetComparer().Equals(new HashSet<TValue>(vs), new HashSet<TValue> { value00, value01 }));
            xvs.Clear();
            Assert.True(data.TryGetValues(key0, xvs));
            count = xvs.Count;
            Assert.Equal(2, count);
            Assert.Contains(value00, xvs);
            Assert.Contains(value01, xvs);
            count = data.Count;
            Assert.Equal(2, count);
            count = data.Count();
            Assert.Equal(2, count);
            Assert.Single(data.Keys, key0);
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Remove(new KeyValuePair<TKey, TValue>(key0, value00)));
            count = data.Count;
            Assert.Equal(1, count);
            count = data.Count();
            Assert.Equal(1, count);
            data.Add(key0, value00);
            Assert.Equal(2, data.Remove(key0));
            Assert.Empty(data);
            // single key, two values (added as enumerable)
            data.Add(key0, value00, value01);
            Assert.True(data.ContainsKey(key0));
            Assert.True(data.TryGetValue(key0, out v));
            Assert.True(EqualityComparer<TValue>.Default.Equals(value00, v) || EqualityComparer<TValue>.Default.Equals(value01, v));
            Assert.True(data.TryGetValues(key0, out vs));
            Assert.NotNull(vs);
            Assert.True(HashSet<TValue>.CreateSetComparer().Equals(new HashSet<TValue>(vs), new HashSet<TValue> { value00, value01 }));
            count = data.Count;
            Assert.Equal(2, count);
            count = data.Count();
            Assert.Equal(2, count);
            Assert.Single(data.Keys, key0);
            data.Set(key0, value01);
            count = data.Count;
            Assert.Equal(1, count);
            count = data.Count();
            Assert.Equal(1, count);
            data.Set(key0, value00, value01);
            Assert.Equal(2, data.Remove(key0));
            Assert.Empty(data);
            // single key, two values (added as keyvalue pairs)
            ((ICollection<KeyValuePair<TKey, TValue>>)data).Add(new KeyValuePair<TKey, TValue>(key0, value00));
            ((ICollection<KeyValuePair<TKey, TValue>>)data).Add(new KeyValuePair<TKey, TValue>(key0, value01));
            Assert.True(data.ContainsKey(key0));
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value00)));
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value01)));
            Assert.True(data.TryGetValue(key0, out v));
            Assert.True(EqualityComparer<TValue>.Default.Equals(value00, v) || EqualityComparer<TValue>.Default.Equals(value01, v));
            Assert.True(data.TryGetValues(key0, out vs));
            Assert.NotNull(vs);
            Assert.True(HashSet<TValue>.CreateSetComparer().Equals(new HashSet<TValue>(vs), new HashSet<TValue> { value00, value01 }));
            count = data.Count;
            Assert.Equal(2, count);
            count = data.Count();
            Assert.Equal(2, count);
            Assert.Single(data.Keys, key0);
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Remove(new KeyValuePair<TKey, TValue>(key0, value00)));
            count = data.Count;
            Assert.Equal(1, count);
            count = data.Count();
            Assert.Equal(1, count);
            data.Add(key0, value00);
            Assert.Equal(2, data.Remove(key0));
            Assert.Empty(data);
            // two keys, two values (added separately)
            data.Add(key0, value00);
            data.Add(key0, value01);
            data.Add(key1, value10);
            data.Add(key1, value11);
            Assert.True(data.ContainsKey(key0));
            Assert.True(data.ContainsKey(key1));
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value00)));
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value01)));
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key1, value10)));
            Assert.True(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key1, value11)));
            Assert.False(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key1, value00)));
            Assert.False(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key1, value01)));
            Assert.False(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value10)));
            Assert.False(((ICollection<KeyValuePair<TKey, TValue>>)data).Contains(new KeyValuePair<TKey, TValue>(key0, value11)));
            Assert.True(data.TryGetValue(key0, out v));
            Assert.True(EqualityComparer<TValue>.Default.Equals(value00, v) || EqualityComparer<TValue>.Default.Equals(value01, v));
            Assert.True(data.TryGetValues(key0, out vs));
            Assert.NotNull(vs);
            Assert.True(HashSet<TValue>.CreateSetComparer().Equals(new HashSet<TValue>(vs), new HashSet<TValue> { value00, value01 }));
            xvs.Clear();
            Assert.True(data.TryGetValues(key0, xvs));
            count = xvs.Count;
            Assert.Equal(2, count);
            Assert.Contains(value00, xvs);
            Assert.Contains(value01, xvs);
            Assert.True(data.TryGetValue(key1, out v));
            Assert.True(EqualityComparer<TValue>.Default.Equals(value10, v) || EqualityComparer<TValue>.Default.Equals(value11, v));
            Assert.True(data.TryGetValues(key1, out vs));
            Assert.NotNull(vs);
            Assert.True(HashSet<TValue>.CreateSetComparer().Equals(new HashSet<TValue>(vs), new HashSet<TValue> { value10, value11 }));
            xvs.Clear();
            Assert.True(data.TryGetValues(key1, xvs));
            count = xvs.Count;
            Assert.Equal(2, count);
            Assert.Contains(value10, xvs);
            Assert.Contains(value11, xvs);
            count = data.Count;
            Assert.Equal(4, count);
            count = data.Count();
            Assert.Equal(4, count);
            Assert.True(HashSet<TKey>.CreateSetComparer().Equals(new HashSet<TKey>(data.Keys), new HashSet<TKey> { key0, key1 }));
            count = data.Keys.Count();
            Assert.Equal(2, count);
            Assert.Equal(2, data.Remove(key0));
            count = data.Count;
            Assert.Equal(2, count);
            count = data.Count();
            Assert.Equal(2, count);
            data.Clear();
            Assert.Empty(data);
        }

        public virtual void ManyItems()
        {
            var items = Enumerable.Range(0, 100).Select(_ =>
            {
                var key = NextKey();
                var values = Enumerable.Range(0, 100).Select(__ => NextValue()).ToArray();
                return new KeyValuePair<TKey, TValue[]>(key, values);
            }).ToDictionary(kv => kv.Key, kv => kv.Value);
            var data = new MultiValueDictionary<TKey, TValue>();
            foreach (var kv in items)
            {
                data.Add(kv.Key, kv.Value);
            }
            var count = data.Count;
            Assert.Equal(100 * 100, count);
            Assert.True(HashSet<TKey>.CreateSetComparer().Equals(new HashSet<TKey>(data.Keys), new HashSet<TKey>(items.Keys)));
            foreach (var key in items.Keys)
            {
                Assert.True(data.ContainsKey(key));
                Assert.True(data.TryGetValue(key, out var v));
                Assert.Contains(v, items[key]);
                Assert.True(data.TryGetValues(key, out var vs));
                count = vs.Length;
                Assert.Equal(100, count);
                Assert.True(HashSet<TValue>.CreateSetComparer().Equals(new HashSet<TValue>(vs), new HashSet<TValue>(items[key])));
                var vsx = new List<TValue>();
                Assert.True(data.TryGetValues(key, vsx));
                count = vsx.Count;
                Assert.Equal(100, count);
                Assert.True(HashSet<TValue>.CreateSetComparer().Equals(new HashSet<TValue>(vsx), new HashSet<TValue>(items[key])));
            }
        }

        public virtual void Serialization()
        {
            var items = Enumerable.Range(0, 100).Select(_ =>
            {
                var key = NextKey();
                var values = Enumerable.Range(0, 100).Select(__ => NextValue()).ToArray();
                return new KeyValuePair<TKey, TValue[]>(key, values);
            }).ToDictionary(kv => kv.Key, kv => kv.Value);
            var data0 = new MultiValueDictionary<TKey, TValue>();
            foreach (var kv in items)
            {
                data0.Add(kv.Key, kv.Value);
            }
            var binaryFormatter = new BinaryFormatter();
            byte[] bin;
            using (var buffer = new MemoryStream())
            {
                binaryFormatter.Serialize(buffer, data0);
                bin = buffer.ToArray();
            }
            MultiValueDictionary<TKey, TValue> data1;
            using (var buffer = new MemoryStream(bin, false))
            {
                data1 = (MultiValueDictionary<TKey, TValue>)binaryFormatter.Deserialize(buffer);
            }
            Assert.Equal(data0, data1);
            Assert.Equal(data0.GetHashCode(), data1.GetHashCode());
            var data2 = data0.Clone();
            Assert.True(((object)data0).Equals(data2));
            Assert.False(((object)data0).Equals(null));
            Assert.False(((object)data0).Equals(2));
        }
    }

    [Serializable]
    public struct MyInt : IEquatable<MyInt>
    {
        readonly int _i;
        public MyInt(int i) => _i = i;

        public bool Equals(MyInt other) => _i == other._i;

        public override bool Equals(object obj) => obj is MyInt other && Equals(other);

        public override int GetHashCode() => _i / 2;
    }

    public class IntIntMultiDictionaryTests : MultiDictionaryTests<int, int>
    {

        int _keySeed;

        int _valueSeed;

        protected override int NextKey() => ++_keySeed;

        protected override int NextValue() => ++_valueSeed;

        [Fact]
        public override void EmptyTests() => base.EmptyTests();

        [Fact]
        public override void BasicOperationsTest() => base.BasicOperationsTest();

        [Fact]
        public override void ManyItems() => base.ManyItems();

        [Fact]
        public override void Serialization() => base.Serialization();
    }

    public class MyIntIntMultiDictionaryTests : MultiDictionaryTests<MyInt, int>
    {

        int _keySeed;

        int _valueSeed;

        protected override MyInt NextKey() => new MyInt(++_keySeed);

        protected override int NextValue() => ++_valueSeed;

        [Fact]
        public override void EmptyTests() => base.EmptyTests();

        [Fact]
        public override void BasicOperationsTest() => base.BasicOperationsTest();

        [Fact]
        public override void ManyItems() => base.ManyItems();

        [Fact]
        public override void Serialization() => base.Serialization();
    }

    public class StringStringMultiDictionaryTests : MultiDictionaryTests<string, string>
    {
        int _keySeed;

        int _valueSeed;

        protected override string NextKey() => (++_keySeed).ToString();

        protected override string NextValue() => (++_valueSeed).ToString();

        [Fact]
        public override void EmptyTests() => base.EmptyTests();

        [Fact]
        public override void BasicOperationsTest() => base.BasicOperationsTest();

        [Fact]
        public override void ManyItems() => base.ManyItems();

        [Fact]
        public override void Serialization() => base.Serialization();
    }
}