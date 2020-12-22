using System;
using System.Collections.Generic;
using System.Linq;
using NCoreUtils.Collections;
using NCoreUtils.Collections.Internal;
using Xunit;

namespace NCoreUtils
{
    public class ImmutableHeadArrayTests
    {
        [Fact]
        public void Default()
        {
            ImmutableHeadArray<int> array = default;
            Assert.Equal(0, array.Length);
            Assert.Equal(0, new ImmutableHeadArray<int>(Enumerable.Empty<int>().ToList()).Length);
            Assert.Equal(0, new ImmutableHeadArray<int>(new int[0].AsSpan()).Length);
            Assert.Throws<IndexOutOfRangeException>(() => array[0]);
            Assert.False(array.GetEnumerator().MoveNext());
            foreach (ref readonly int i in array)
            {
                Assert.True(false, "no iteration shuild occur.");
            }
            ImmutableHeadArray<int>.Enumerator enumerator = default;
            // should neither throw not segfault
            Assert.Equal(default, enumerator.Current);
            Assert.False(enumerator.MoveNext());

            // null arg
            Assert.Throws<ArgumentNullException>(() => new ImmutableHeadArray<int>(default(IReadOnlyList<int>)));
        }

        private void CheckHeadOnly(in ImmutableHeadArray<int> array)
        {
            Assert.False(array.SequenceEqual(new int[] { }));
            Assert.True(array.SequenceEqual(new int[] { 1 }));
            Assert.False(array.SequenceEqual(new int[] { 2 }));
            Assert.False(array.SequenceEqual(new int[] { 1, 2 }));
            Assert.Equal(1, array.Length);
            Assert.Equal(1, array[0]);
            var iterations = 0;
            var enumerator = array.GetEnumerator();
            while (enumerator.MoveNext()) { }
            Assert.False(enumerator.MoveNext());
            foreach (ref readonly int i in array)
            {
                ++iterations;
                Assert.Equal(1, i);
            }
            Assert.Equal(1, iterations);
        }

        [Fact]
        public void HeadOnly()
        {
            ImmutableHeadArray<int> empty = default;
            ImmutableHeadArray<int> array0 = new ImmutableHeadArray<int>(1);
            ImmutableHeadArray<int> array1 = default(ImmutableHeadArray<int>).Append(1);
            ImmutableHeadArray<int> array2 = default(ImmutableHeadArray<int>).Prepend(1);
            ImmutableHeadArray<int> array3 = new ImmutableHeadArray<int>(new int[] { 1 }.AsSpan());
            ImmutableHeadArray<int> array4 = new ImmutableHeadArray<int>((IReadOnlyList<int>)new int[] { 1 });
            Assert.Throws<ArgumentNullException>(() => array0.SequenceEqual(default(IEnumerable<int>)));
            Assert.True(array0.SequenceEqual(in array1));
            Assert.False(array0.SequenceEqual(in empty));
            Assert.Throws<IndexOutOfRangeException>(() => array0[1]);
            CheckHeadOnly(in array0);
            Assert.Throws<IndexOutOfRangeException>(() => array1[1]);
            CheckHeadOnly(in array1);
            Assert.Throws<IndexOutOfRangeException>(() => array2[1]);
            CheckHeadOnly(in array2);
            Assert.Throws<IndexOutOfRangeException>(() => array3[1]);
            CheckHeadOnly(in array3);
            Assert.Throws<IndexOutOfRangeException>(() => array4[1]);
            CheckHeadOnly(in array4);
        }

        [Fact]
        public void Tail()
        {
            ImmutableHeadArray<int> array0 = default;
            ImmutableHeadArray<int> array1 = new ImmutableHeadArray<int>(1);
            ImmutableHeadArray<int> array2 = new ImmutableHeadArray<int>((IReadOnlyList<int>)new int[] { 1, 2 });
            ImmutableHeadArray<int> array3 = new ImmutableHeadArray<int>(new int[] { 1, 2, 3 }.AsSpan());
            Assert.True(array0.SequenceEqual(in array0));
            Assert.Equal(new int[] {}, array0.ToArray());
            Assert.False(array0.SequenceEqual(in array1));
            Assert.False(array0.SequenceEqual(in array2));
            Assert.False(array0.SequenceEqual(in array3));

            Assert.Equal(new int[] { 1 }, array1.ToArray());
            Assert.False(array1.SequenceEqual(in array0));
            Assert.True(array1.SequenceEqual(in array1));
            Assert.False(array1.SequenceEqual(in array2));
            Assert.False(array1.SequenceEqual(in array3));
            Assert.False(array1.SequenceEqual(new ImmutableHeadArray<int>(2)));

            Assert.Equal(new int[] { 1, 2 }, array2.ToArray());
            Assert.False(array2.SequenceEqual(in array0));
            Assert.False(array2.SequenceEqual(in array1));
            Assert.True(array2.SequenceEqual(in array2));
            Assert.False(array2.SequenceEqual(in array3));
            Assert.False(array2.SequenceEqual(new ImmutableHeadArray<int>(1, new [] { 3 })));

            Assert.Equal(new int[] { 1, 2, 3 }, array3.ToArray());
            Assert.False(array3.SequenceEqual(in array0));
            Assert.False(array3.SequenceEqual(in array1));
            Assert.False(array3.SequenceEqual(in array2));
            Assert.True(array3.SequenceEqual(in array3));
            Assert.False(array3.SequenceEqual(new ImmutableHeadArray<int>(1, new [] { 2, 4 })));

            Assert.True(array3.Pop().SequenceEqual(in array2));
            Assert.True(array3.Unshift().SequenceEqual(new ImmutableHeadArray<int>(2, new int[] { 3 })));

            var iterations = 0;
            var last = 0;
            foreach (ref readonly int i in array3)
            {
                ++iterations;
                Assert.Equal(last + 1, i);
                last = i;
            }
            Assert.Equal(3, last);
            Assert.Equal(3, iterations);
        }

        [Fact]
        public void PopUnshiftAppendPrepend()
        {
            var array0 = new ImmutableHeadArray<int>();
            var array1 = new ImmutableHeadArray<int>(1);
            var array12 = new ImmutableHeadArray<int>(1, new int[] { 2 });
            var array23 = new ImmutableHeadArray<int>(2, new int[] { 3 });
            var array34 = new ImmutableHeadArray<int>(3, new int[] { 4 });
            var array4 = new ImmutableHeadArray<int>(4);

            Assert.True(array23.Pop().Append(array34.Unshift()).SequenceEqual(array12.Map((in int i) => i * 2)));
            Assert.True(array34.Unshift().Prepend(array23.Pop()).SequenceEqual(array12.Map((in int _, int index) => (index + 1) * 2)));
            Assert.Throws<InvalidOperationException>(() => array0.Pop());
            Assert.Throws<InvalidOperationException>(() => array0.Unshift());

            Assert.Empty(array1.Unshift().ToArray());
            Assert.Empty(array1.Pop().ToArray());


            Span<int> buffer = stackalloc int[10];
            Assert.Throws<ArgumentException>(() =>
            {
                Span<int> buffer0 = stackalloc int[1];
                array23.CopyTo(buffer0);
            });
            var size = array12.Append(array34).CopyTo(buffer);
            Assert.Equal(4, size);
            Assert.True(new int[] { 1, 2, 3, 4 }.AsSpan().IsSame(buffer.Slice(0, size)));

            Assert.True(array0.Append(array12).Append(3).SequenceEqual(array12.Append(3)));
            Assert.True(array0.Prepend(array23).Prepend(1).SequenceEqual(array12.Append(3)));
            Assert.True(array1.Append(2).SequenceEqual(array12));
            Assert.True(array4.Prepend(3).SequenceEqual(array34));

            Assert.Empty(array0.Map((in int i) => (byte)i).ToArray());
            Assert.Empty(array0.Map((in int i, int _) => (byte)i).ToArray());

            Assert.Single(array1.Map((in int i) => (byte)(i * 2)).ToArray(), b => b == 2);
            Assert.Single(array1.Map((in int i, int index) => (byte)(i * (index + 1))).ToArray(), b => b == 1);
        }

        [Fact]
        public void ImmutableNullableInvalidDeref()
        {
            ImmutableNullable<int> n = default;
            Assert.Throws<InvalidOperationException>(() => n.Value);
        }

        [Fact]
        public void Builder()
        {
            var b0 = new ImmutableHeadArrayBuilder<int>();
            Assert.Equal(0, b0.Count);
            Assert.Empty(b0.Build().ToArray());
            var b1 = new ImmutableHeadArrayBuilder<int>(1);
            b1.Add(1);
            Assert.Single(b1.Build().ToArray());
            Assert.Single(b1.Build().ToArray(), i => i == 1);
            Assert.Equal(1, b1.Count);
            var b2 = new ImmutableHeadArrayBuilder<int>();
            b2.Add(1);
            b2.Add(2);
            b2.Add(3);
            b2.Add(4);
            b2.Add(5);
            Assert.Equal(new int[] { 1, 2, 3, 4, 5 }, b2.Build().ToArray());
            Assert.Equal(5, b2.Count);
            var b3 = new ImmutableHeadArrayBuilder<int>(8);
            b3.Add(1);
            b3.Add(2);
            b3.Add(3);
            b3.Add(4);
            b3.Add(5);
            Assert.Equal(new int[] { 1, 2, 3, 4, 5 }, b3.Build().ToArray());
            Assert.Equal(5, b3.Count);
        }
    }
}