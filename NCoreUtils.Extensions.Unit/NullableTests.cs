using System;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class NullableTests
    {
        [Fact]
        public void Map()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Throws<ArgumentNullException>(() => NullableExtensions.Map<int, int>(n, null));

            Func<int, int> selector = i => i * 2;

            Assert.True(n.Map(selector).HasValue);
            Assert.Equal(4, n.Map(selector).Value);
            Assert.False(n0.Map(selector).HasValue);

            // selector not called
            Assert.False(n0.Map(new Func<int, int>(_ => throw new InvalidOperationException("Should not be called"))).HasValue);
        }

        [Fact]
        public void Bind()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Throws<ArgumentNullException>(() => NullableExtensions.Bind<int, int>(n, null));

            Func<int, int?> binder1 = i => i * 2;
            Func<int, int?> binder2 = _ => null;

            Assert.True(n.Bind(binder1).HasValue);
            Assert.Equal(4, n.Bind(binder1).Value);
            Assert.False(n0.Bind(binder1).HasValue);

            Assert.False(n.Bind(binder2).HasValue);
            Assert.False(n0.Bind(binder2).HasValue);

            // selector not called
            Assert.False(n0.Bind(new Func<int, int?>(_ => throw new InvalidOperationException("Should not be called"))).HasValue);
        }

        [Fact]
        public void Where()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Throws<ArgumentNullException>(() => NullableExtensions.Where<int>(n, null));

            Assert.True(n.Where(i => i == 2).HasValue);
            Assert.False(n.Where(i => i == 3).HasValue);
            Assert.False(n0.Where(i => i == 2).HasValue);
            Assert.False(n0.Where(i => i == 3).HasValue);
        }

        [Fact]
        public void All()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Throws<ArgumentNullException>(() => NullableExtensions.All<int>(n, null));

            Assert.True(n.All(i => i == 2));
            Assert.False(n.All(i => i == 3));
            Assert.True(n0.All(i => i == 2));
            Assert.True(n0.All(i => i == 3));
        }

        [Fact]
        public void Any()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.True(n.Any());
            Assert.False(n0.Any());
        }

        [Fact]
        public void AnyWithPredicate()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Throws<ArgumentNullException>(() => NullableExtensions.Any<int>(n, null));

            Assert.True(n.Any(i => i == 2));
            Assert.False(n.Any(i => i == 3));
            Assert.False(n0.Any(i => i == 2));
            Assert.False(n0.Any(i => i == 3));
        }

        [Fact]
        public void Supply()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Throws<ArgumentNullException>(() => NullableExtensions.Supply<int>(n, null));

            Assert.True(n.Supply(() => 1).HasValue);
            Assert.True(n0.Supply(() => 1).HasValue);
            Assert.Equal(2, n.Supply(() => 1).Value);
            Assert.Equal(1, n0.Supply(() => 1).Value);

            // not called
            Assert.True(n.Supply(() => throw new InvalidOperationException("Shpuld not be called")).HasValue);
        }

        [Fact]
        public void GetOrDefault()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Equal(2, n.GetOrDefault());
            Assert.Equal(default(int), n0.GetOrDefault());
            Assert.Equal(5, n0.GetOrDefault(5));
        }

        [Fact]
        public void IsEmpty()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.False(n.IsEmpty());
            Assert.True(n0.IsEmpty());
        }

        [Fact]
        public void Aggregate()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Throws<ArgumentNullException>(() => NullableExtensions.Aggregate<int, int>(n, 2, null));

            Assert.Equal(3, n.Aggregate(1, (sum, i) => sum + i));
            Assert.Equal(1, n0.Aggregate(1, (sum, i) => sum + i));

            // not called
            Assert.Equal(1, n0.Aggregate(1, (_, __) => throw new InvalidOperationException("Shpuld not be called")));
        }

        [Fact]
        public void ToEnumerable()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Equal(new int[] { 2 }, n.ToEnumerable());
            Assert.Equal(new int[] { }, n0.ToEnumerable());
        }

        [Fact]
        public void ToList()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Equal(new int[] { 2 }, n.ToList());
            Assert.Equal(new int[] { }, n0.ToList());
        }

        [Fact]
        public void ToArray()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Equal(new int[] { 2 }, n.ToArray());
            Assert.Equal(new int[] { }, n0.ToArray());
        }

        [Fact]
        public void Zip()
        {
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.Throws<ArgumentNullException>(() => NullableExtensions.Zip<int, int, int>(n, n, null));

            Func<int, int, int> selector = (a, b) => a + b;
            Func<int, int, int> fail = (a, b) => throw new InvalidOperationException("Should not be called");

            Assert.True(n.Zip(n, selector).HasValue);
            Assert.False(n0.Zip(n, selector).HasValue);
            Assert.False(n.Zip(n0, selector).HasValue);
            Assert.False(n0.Zip(n0, selector).HasValue);

            Assert.Equal(4, n.Zip(n, selector).Value);

            Assert.False(n0.Zip(n, fail).HasValue);
            Assert.False(n.Zip(n0, fail).HasValue);
            Assert.False(n0.Zip(n0, fail).HasValue);

            Assert.Equal((2,2), n.Zip(n).Value);

        }

        [Fact]
        public void TryGetValue()
        {
            int value;
            Nullable<int> n = 2;
            Nullable<int> n0 = null;

            Assert.True(n.TryGetValue(out value));
            Assert.Equal(2, value);
            Assert.False(n0.TryGetValue(out value));
            Assert.Equal(default(int), value);
        }

    }
}