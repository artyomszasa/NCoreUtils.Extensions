using System;
using Xunit;

namespace NCoreUtils
{
    public class SpanExtensionsTests
    {
        [Fact]
        public void GenericIsSameTests()
        {
            var cbuffer0 = "012345".AsSpan();
            Span<char> cbuffer1 = stackalloc char[cbuffer0.Length];
            var cbuffer2 = "01234".AsSpan();
            Span<char> cbufferX = stackalloc char[cbuffer2.Length];
            cbuffer2.CopyTo(cbufferX);
            var cbuffer3 = "012346".AsSpan();
            Span<char> cbufferY = stackalloc char[cbuffer3.Length];
            cbuffer3.CopyTo(cbufferY);
            cbuffer0.CopyTo(cbuffer1);
            Assert.True(cbuffer0.IsSame(cbuffer0));
            Assert.True(cbuffer1.IsSame(cbuffer1));
            Assert.True(cbuffer0.IsSame(cbuffer1));
            Assert.True(cbuffer1.IsSame(cbuffer0));
            Assert.False(cbuffer2.IsSame(cbuffer0));
            Assert.False(cbuffer0.IsSame(cbuffer2));
            Assert.False(cbufferX.IsSame(cbuffer0));
            Assert.False(cbuffer0.IsSame(cbufferX));
            Assert.False(cbuffer3.IsSame(cbuffer0));
            Assert.False(cbuffer0.IsSame(cbuffer3));
            Assert.False(cbuffer3.IsSame(cbuffer0));
            Assert.False(cbuffer0.IsSame(cbuffer3));
            Assert.False(cbuffer3.IsSame(cbuffer1));
            Assert.False(cbuffer1.IsSame(cbuffer3));
            Assert.False(cbufferX.IsSame(cbuffer1));
            Assert.False(cbuffer1.IsSame(cbufferX));
            Assert.False(cbufferY.IsSame(cbuffer1));
            Assert.False(cbuffer1.IsSame(cbufferY));
        }

        [Fact]
        public void ByteIsSameTests()
        {
            ReadOnlySpan<byte> cbuffer0 = new byte[]{0, 1, 2, 3, 4, 5}.AsSpan();
            Span<byte> cbuffer1 = stackalloc byte[cbuffer0.Length];
            ReadOnlySpan<byte> cbuffer2 = new byte[]{0,1,2,3,4}.AsSpan();
            Span<byte> cbufferX = stackalloc byte[cbuffer2.Length];
            cbuffer2.CopyTo(cbufferX);
            ReadOnlySpan<byte> cbuffer3 = new byte[]{0,1,2,3,4,6}.AsSpan();
            Span<byte> cbufferY = stackalloc byte[cbuffer3.Length];
            cbuffer3.CopyTo(cbufferY);
            cbuffer0.CopyTo(cbuffer1);
            Assert.True(cbuffer0.IsSame(cbuffer0));
            Assert.True(cbuffer1.IsSame(cbuffer1));
            Assert.True(cbuffer0.IsSame(cbuffer1));
            Assert.True(cbuffer1.IsSame(cbuffer0));
            Assert.False(cbuffer2.IsSame(cbuffer0));
            Assert.False(cbuffer0.IsSame(cbuffer2));
            Assert.False(cbufferX.IsSame(cbuffer0));
            Assert.False(cbuffer0.IsSame(cbufferX));
            Assert.False(cbuffer3.IsSame(cbuffer0));
            Assert.False(cbuffer0.IsSame(cbuffer3));
            Assert.False(cbuffer3.IsSame(cbuffer0));
            Assert.False(cbuffer0.IsSame(cbuffer3));
            Assert.False(cbuffer3.IsSame(cbuffer1));
            Assert.False(cbuffer1.IsSame(cbuffer3));
            Assert.False(cbufferX.IsSame(cbuffer1));
            Assert.False(cbuffer1.IsSame(cbufferX));
            Assert.False(cbufferY.IsSame(cbuffer1));
            Assert.False(cbuffer1.IsSame(cbufferY));
        }
    }
}