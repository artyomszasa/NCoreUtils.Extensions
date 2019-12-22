using System;
using System.Collections.Generic;
using NCoreUtils.Collections;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class TinyImmutableArrayTests
    {
        [Fact]
        public void BasicFunctionality()
        {
            var array0 = TinyImmutableArray.Empty<int>();
            var array1 = TinyImmutableArray.Create<int>(1);
            var array2 = TinyImmutableArray.Create<int>(1, 2);
            var array3 = TinyImmutableArray.Create<int>(1, 2, 3);
            var array4 = TinyImmutableArray.Create<int>(new [] { 1, 2, 3, 4 });
            var array5 = TinyImmutableArray.Create<int>(new [] { 1, 2, 3, 4, 5 });

            Assert.Equal(0, array0.Count);
            Assert.Equal(1, array1.Count);
            Assert.Equal(2, array2.Count);
            Assert.Equal(3, array3.Count);
            Assert.Equal(4, array4.Count);
            Assert.Equal(5, array5.Count);

            Assert.Throws<IndexOutOfRangeException>(() => array0[0]);
            Assert.Throws<IndexOutOfRangeException>(() => array0[1]);
            Assert.Throws<IndexOutOfRangeException>(() => array0[2]);
            Assert.Throws<IndexOutOfRangeException>(() => array0[3]);

            Assert.Equal(1, array1[0]);
            Assert.Throws<IndexOutOfRangeException>(() => array1[1]);
            Assert.Throws<IndexOutOfRangeException>(() => array1[2]);
            Assert.Throws<IndexOutOfRangeException>(() => array1[3]);

            Assert.Equal(1, array2[0]);
            Assert.Equal(2, array2[1]);
            Assert.Throws<IndexOutOfRangeException>(() => array2[2]);
            Assert.Throws<IndexOutOfRangeException>(() => array2[3]);

            Assert.Equal(1, array3[0]);
            Assert.Equal(2, array3[1]);
            Assert.Equal(3, array3[2]);
            Assert.Throws<IndexOutOfRangeException>(() => array3[3]);

            Assert.Equal(1, array4[0]);
            Assert.Equal(2, array4[1]);
            Assert.Equal(3, array4[2]);
            Assert.Equal(4, array4[3]);

            Assert.Equal(1, array5[0]);
            Assert.Equal(2, array5[1]);
            Assert.Equal(3, array5[2]);
            Assert.Equal(4, array5[3]);
            Assert.Equal(5, array5[4]);
        }

        [Fact]
        public void Enumeration()
        {
            var array0 = TinyImmutableArray.Empty<int>();
            var array1 = TinyImmutableArray.Create<int>(1);
            var array2 = TinyImmutableArray.Create<int>(1, 2);
            var array3 = TinyImmutableArray.Create<int>(1, 2, 3);
            var array4 = TinyImmutableArray.Create<int>(new [] { 1, 2, 3, 4 });
            var array5 = TinyImmutableArray.Create<int>(new [] { 1, 2, 3, 4, 5 });

            var index = 0;
            foreach (var item in array0)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in array1)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in array2)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in array3)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in array4)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in array5)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in (IEnumerable<int>)array0)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in (IEnumerable<int>)array1)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in (IEnumerable<int>)array2)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in (IEnumerable<int>)array3)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in (IEnumerable<int>)array4)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in (IEnumerable<int>)array5)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }
        }

        [Fact]
        public void Builder()
        {
            var array0 = Create(0);
            var array1 = Create(1);
            var array2 = Create(2);
            var array3 = Create(3);

            var index = 0;
            foreach (var item in array0)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in array1)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in array2)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            index = 0;
            foreach (var item in array3)
            {
                Assert.Equal(index + 1, item);
                ++index;
            }

            static TinyImmutableArray<int> Create(int count)
            {
                var builder = TinyImmutableArray.CreateBuilder<int>();
                for (var i = 0; i < count; ++i)
                {
                    builder.Add(i + 1);
                }
                return builder.Build();
            }
        }

        [Fact]
        public void Enumerator()
        {
            var array = TinyImmutableArray.Create(0, 1, 2);
            var enumerator = array.GetEnumerator();
            Assert.Equal(default, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(0, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(1, enumerator.Current);
            Assert.True(enumerator.MoveNext());
            Assert.Equal(2, enumerator.Current);
            Assert.False(enumerator.MoveNext());
        }
    }
}