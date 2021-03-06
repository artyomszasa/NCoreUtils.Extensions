using System;
using System.Collections.Generic;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class DictionaryTests
    {
        [Fact]
        public void GetOrDefault()
        {
            Assert.Throws<ArgumentNullException>(() => DictionaryExtensions.GetOrDefault<int, int>(null, 2));

            var d = new Dictionary<int, int> { { 2, 3 } };

            Assert.Equal(3, d.GetOrDefault(2));
            Assert.Equal(default(int), d.GetOrDefault(3));
            Assert.Equal(5, d.GetOrDefault(3, 5));
        }

        [Fact]
        public void GetOrSupply()
        {
            var d = new Dictionary<int, int> { { 2, 3 } };

            Assert.Throws<ArgumentNullException>(() => DictionaryExtensions.GetOrSupply<int, int>(null, 2, () => 3));
            Assert.Throws<ArgumentNullException>(() => DictionaryExtensions.GetOrSupply<int, int>(d, 2, null));

            // supply not called
            Assert.Equal(3, d.GetOrSupply(2, () => throw new InvalidOperationException("Should not be called")));
            Assert.Equal(5, d.GetOrSupply(3, () => 5));
        }
    }
}