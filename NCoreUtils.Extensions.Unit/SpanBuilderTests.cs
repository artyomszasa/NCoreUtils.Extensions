using System;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class SpanBuilderTests
    {
        [Fact]
        public void Primitive()
        {
            Span<char> span = stackalloc char[500];
            var builder = new SpanBuilder(span);
            builder.Append((byte)1);
            builder.Append((ushort)1);
            builder.Append((uint)1);
            builder.Append((ulong)1);
            builder.Append((sbyte)1);
            builder.Append((short)1);
            builder.Append(1);
            builder.Append((long)1);
            builder.Append('1');
            Assert.Equal("111111111", builder.ToString());
        }

        [Fact]
        public void Span()
        {
            Span<char> span = stackalloc char[500];
            var builder = new SpanBuilder(span);
            builder.Append("01234");
            builder.Append(span.Slice(0, builder.Length));
            Assert.Equal("0123401234", builder.ToString());
        }

        [Fact]
        public void Generic()
        {
            Span<char> span = stackalloc char[500];
            var builder = new SpanBuilder(span);
            var guid = Guid.NewGuid();
            builder.Append(guid);
            Assert.Equal(guid.ToString(), builder.ToString());
        }
    }
}