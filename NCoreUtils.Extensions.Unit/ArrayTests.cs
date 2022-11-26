using System;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class ArrayTests
    {
        [Fact]
        public void Initialize()
        {
            Assert.Throws<ArgumentNullException>(() => ArrayExtensions.Initialize<int>(10, null!));
            Assert.Equal(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }, ArrayExtensions.Initialize(10, i => i));
        }

        [Fact]
        public void Map()
        {
            var array = ArrayExtensions.Initialize<int>(10, i => i);
            var expected = ArrayExtensions.Initialize<int>(10, i => i * 2);

            static int selector(int i) => i * 2;

            Assert.Throws<ArgumentNullException>(() => array.Map<int, int>(null!));
            Assert.Throws<ArgumentNullException>(() => ArrayExtensions.Map<int, int>(null!, selector));

            Assert.Equal(expected, array.Map(selector));
        }

        [Fact]
        public void Fill()
        {
            Assert.Throws<ArgumentNullException>(() => ArrayExtensions.Fill(null!, 2));

            var array = ArrayExtensions.Initialize<int>(10, _ => 2);
            foreach (var item in array)
            {
                Assert.Equal(2, item);
            }

            array.Fill();
            foreach (var item in array)
            {
                Assert.Equal(default, item);
            }

            array.Fill(10);
            foreach (var item in array)
            {
                Assert.Equal(10, item);
            }
        }

        [Fact]
        [Obsolete("Backward compatiblity only")]
        public void Slice()
        {
            Assert.Throws<ArgumentNullException>(() => ArrayExtensions.Slice<int>(null!, 0));

            var array = ArrayExtensions.Initialize<int>(10, i => i);

            // no length case
            for (var i = 0; i < array.Length; ++i)
            {
                Assert.Equal(10 - i, array.Slice(i).Length);
                var res = array.Slice(i);
                for (var j = 0; j < res.Length; ++j)
                {
                    Assert.Equal(array[i + j], res[j]);
                }
            }

            // length case
            Assert.Equal(2, array.Slice(2, 2).Length);

            Assert.Throws<ArgumentException>(() => array.Slice(5, 10));
        }
    }
}