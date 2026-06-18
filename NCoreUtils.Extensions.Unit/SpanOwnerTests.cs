using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Xunit;

namespace NCoreUtils.Extensions.Unit;

public class SpanOwnerTests
{
    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static void Ignore<T>(T item) { /* noop */ }

    [Fact]
    public void Default()
    {
        using var buffer = SpanOwner.Allocate<byte>(127);
        Assert.Equal(127, buffer.Length);
        Assert.Equal(127, buffer.Span.Length);
        Assert.True(buffer.DangerousGetArray().Length >= 127);
        Assert.Equal(2, buffer[1..3].Length);
        for (var i = 0; i < 127; ++i)
        {
            buffer[i] = (byte)i;
        }
        for (var i = 0; i < 127; ++i)
        {
            Assert.Equal(i, buffer[i]);
        }
        {
            var j = 0;
            var enumerator = buffer.GetEnumerator();
            while (j < 127)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(j, enumerator.Current);
                ++j;
            }
            j = 0;
        }
        {
            var j = 0;
            var enumerator = ((Span<byte>)buffer).GetEnumerator();
            while (j < 127)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(j, enumerator.Current);
                ++j;
            }
            j = 0;
        }
        {
            var j = 0;
            var enumerator = ((ReadOnlySpan<byte>)buffer).GetEnumerator();
            while (j < 127)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(j, enumerator.Current);
                ++j;
            }
            j = 0;
        }

        Assert.Throws<ArgumentOutOfRangeException>(static () =>
        {
            using var buffer = SpanOwner.Allocate<byte>(127);
            Ignore(buffer[127]);
        });
    }

    [Fact]
    public void DefaultNonShared()
    {
        var pool = ArrayPool<byte>.Create();
        using var buffer = SpanOwner.Allocate(pool, 127);
        Assert.Equal(127, buffer.Length);
        Assert.Equal(127, buffer.Span.Length);
        Assert.True(buffer.DangerousGetArray().Length >= 127);
        Assert.Equal(2, buffer[1..3].Length);
        for (var i = 0; i < 127; ++i)
        {
            buffer[i] = (byte)i;
        }
        for (var i = 0; i < 127; ++i)
        {
            Assert.Equal(i, buffer[i]);
        }
        {
            var j = 0;
            var enumerator = buffer.GetEnumerator();
            while (j < 127)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(j, enumerator.Current);
                ++j;
            }
            j = 0;
        }
        {
            var j = 0;
            var enumerator = ((Span<byte>)buffer).GetEnumerator();
            while (j < 127)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(j, enumerator.Current);
                ++j;
            }
            j = 0;
        }
        {
            var j = 0;
            var enumerator = ((ReadOnlySpan<byte>)buffer).GetEnumerator();
            while (j < 127)
            {
                Assert.True(enumerator.MoveNext());
                Assert.Equal(j, enumerator.Current);
                ++j;
            }
            j = 0;
        }

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            using var buffer = SpanOwner.Allocate(pool, 127);
            Ignore(buffer[127]);
        });
    }
}