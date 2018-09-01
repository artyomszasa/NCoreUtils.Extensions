using System;
using System.Collections.Generic;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class ListTests
    {
        [Fact]
        public void Pop()
        {
            var list = new List<int> { 1, 2 };
            var list0 = new List<int>();

            Assert.Throws<ArgumentNullException>(() => ListExtensions.Pop<int>(null));
            Assert.Throws<InvalidOperationException>(() => list0.Pop());

            Assert.Equal(2, list.Pop());
            Assert.Equal(new [] { 1 }, list);

        }
    }
}