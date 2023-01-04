using System;
using System.Globalization;
using NCoreUtils.Memory;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class SpanBuilderTests
    {
        private sealed class WholePartEmplacer : IEmplacer<double>
        {
            public int Emplace(double value, Span<char> span)
                => Emplacer.Emplace((long)value, span);

            public bool TryEmplace(double value, Span<char> span, out int used)
                => Emplacer.TryEmplace((long)value, span, out used);
        }

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

        [Theory]
        [InlineData(-100.0f, 3.0f, "G10")]
        [InlineData(-10.0f, 3.0f, "G9")]
        [InlineData(-10.0f, 5.0f, "G9")]
        [InlineData(-10.0f, 4.0f, "G9")]
        [InlineData(-10.0f, 6.0f, "G9")]
        [InlineData((float)0, (float)1, "G")]
        [InlineData(10.0f, 6.0f, "G9")]
        [InlineData(10.0f, 5.0f, "G9")]
        [InlineData(10.0f, 4.0f, "G9")]
        [InlineData(10.0f, 3.0f, "G9")]
        [InlineData(100.0f, 3.0f, "G10")]
        public void Single(float a, float b, string format)
        {
            var value = a / b;
            var expected = value.ToString(format, CultureInfo.InvariantCulture);
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                builder.Append(value);
                Assert.Equal(expected, builder.ToString());
            }
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                Assert.True(builder.TryAppend(value));
                Assert.Equal(expected, builder.ToString());
            }
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                var builder = new SpanBuilder(span);
                Assert.False(builder.TryAppend(value));
            }
        }

        [Theory]
        [InlineData(-100.0, 3.0, "G10")]
        [InlineData(-10.0, 3.0, "G9")]
        [InlineData(-10.0, 5.0, "G9")]
        [InlineData(-10.0, 4.0, "G9")]
        [InlineData(-10.0, 6.0, "G9")]
        [InlineData((double)0, (double)1, "G")]
        [InlineData(10.0, 6.0, "G9")]
        [InlineData(10.0, 5.0, "G9")]
        [InlineData(10.0, 4.0, "G9")]
        [InlineData(10.0, 3.0, "G9")]
        [InlineData(100.0, 3.0, "G10")]
        public void Double(double a, double b, string format)
        {
            var value = a / b;
            var expected = value.ToString(format, CultureInfo.InvariantCulture);
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                builder.Append(value);
                Assert.Equal(expected, builder.ToString());
            }
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                Assert.True(builder.TryAppend(value));
                Assert.Equal(expected, builder.ToString());
            }
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                var builder = new SpanBuilder(span);
                Assert.False(builder.TryAppend(value));
            }
        }

        private static bool TryAppendSpanCopy(ref SpanBuilder builder, ReadOnlySpan<char> span)
        {
            Span<char> copy = stackalloc char[span.Length];
            span.CopyTo(copy);
            return builder.TryAppend(copy);
        }

        private static bool TryAppendReadOnlySpanCopy(ref SpanBuilder builder, ReadOnlySpan<char> span)
        {
            Span<char> copy = stackalloc char[span.Length];
            span.CopyTo(copy);
            return builder.TryAppend((ReadOnlySpan<char>)copy);
        }

        [Fact]
        public void Span()
        {
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                builder.Append("01234");
                builder.Append(span[..builder.Length]);
                Assert.Equal("0123401234", builder.ToString());
            }
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                {
                    builder.Append("01234");
                    Assert.True(builder.TryAppend(span[..builder.Length]));
                }
                Assert.Equal("0123401234", builder.ToString());
            }
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                builder.Append("01234");
                builder.Append(((ReadOnlySpan<char>)span)[..builder.Length]);
                Assert.Equal("0123401234", builder.ToString());
            }
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                Assert.True(builder.TryAppend("01234"));
                Assert.True(builder.TryAppend(span[..builder.Length]));
                Assert.Equal("0123401234", builder.ToString());
            }
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                Assert.True(builder.TryAppend("01234"));
                Assert.True(builder.TryAppend(((ReadOnlySpan<char>)span)[..builder.Length]));
                Assert.Equal("0123401234", builder.ToString());
            }
            {
                Span<char> span = stackalloc char[6];
                var builder = new SpanBuilder(span);
                builder.Append("01234");
                Assert.False(builder.TryAppend(span[..builder.Length]));
            }
            {
                Span<char> span = stackalloc char[6];
                var builder = new SpanBuilder(span);
                builder.Append("01234");
                Assert.False(builder.TryAppend(((ReadOnlySpan<char>)span)[..builder.Length]));
            }
        }

        [Fact]
        public void Generic()
        {
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                var guid = Guid.NewGuid();
                builder.Append(guid);
                Assert.Equal(guid.ToString(), builder.ToString());
            }
            {
                Span<char> span = stackalloc char[500];
                var builder = new SpanBuilder(span);
                var guid = Guid.NewGuid();
                Assert.True(builder.TryAppend(guid));
                Assert.Equal(guid.ToString(), builder.ToString());
            }
        }

        [Fact]
        public void ExplicitEmplacer()
        {
            {
                Span<char> span = stackalloc char[2];
                var builder = new SpanBuilder(span);
                Assert.True(builder.TryAppend(12.5, new WholePartEmplacer()));
                Assert.Equal("12", builder.ToString());
            }
            {
                Span<char> span = stackalloc char[1];
                var builder = new SpanBuilder(span);
                Assert.False(builder.TryAppend(12.5, new WholePartEmplacer()));
            }
        }

        [Fact]
        public void Failures()
        {
            Span<char> buffer = stackalloc char[0];
            var builder = new SpanBuilder(buffer);
            Assert.False(builder.TryAppend('x'));
            Assert.False(builder.TryAppend((sbyte.MaxValue)));
            Assert.False(builder.TryAppend((short.MaxValue)));
            Assert.False(builder.TryAppend((int.MaxValue)));
            Assert.False(builder.TryAppend((long.MaxValue)));
            Assert.False(builder.TryAppend((byte.MaxValue)));
            Assert.False(builder.TryAppend((ushort.MaxValue)));
            Assert.False(builder.TryAppend((uint.MaxValue)));
            Assert.False(builder.TryAppend((ulong.MaxValue)));
            Assert.False(builder.TryAppend("xxx"));
        }

        [Fact]
        public void TrySuccess()
        {
            Span<char> buffer = stackalloc char[8 * 1024];
            var builder = new SpanBuilder(buffer);
            Assert.True(builder.TryAppend('x'));
            Assert.True(builder.TryAppend((sbyte.MaxValue)));
            Assert.True(builder.TryAppend((short.MaxValue)));
            Assert.True(builder.TryAppend((int.MaxValue)));
            Assert.True(builder.TryAppend((long.MaxValue)));
            Assert.True(builder.TryAppend((byte.MaxValue)));
            Assert.True(builder.TryAppend((ushort.MaxValue)));
            Assert.True(builder.TryAppend((uint.MaxValue)));
            Assert.True(builder.TryAppend((ulong.MaxValue)));
            Assert.True(builder.TryAppend("xxx"));
            Assert.Equal($"x{sbyte.MaxValue}{short.MaxValue}{int.MaxValue}{long.MaxValue}{byte.MaxValue}{ushort.MaxValue}{uint.MaxValue}{ulong.MaxValue}xxx", builder.ToString());
        }
    }
}