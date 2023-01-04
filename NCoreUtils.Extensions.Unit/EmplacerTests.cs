using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NCoreUtils.Memory;
using Xunit;

namespace NCoreUtils.Extensions.Unit
{
    public class EmplacerTests
    {
        sealed class Box<T>
        {
            public T Value { get; }

            public Box(T value) => Value = value;

            public override string? ToString() => Value?.ToString();
        }

        sealed class EmplaceableBox<T> : ISpanEmplaceable
        {
            public T Value { get; }

            public EmplaceableBox(T value) => Value = value;

            public override string? ToString() => Value?.ToString();

            public string ToString(string? format, IFormatProvider? formatProvider)
                => ToString() ?? string.Empty;

            public bool TryEmplace(Span<char> span, out int used)
            {
                return Emplacer.GetDefault<T>().TryEmplace(Value, span, out used);
            }
        }

        private static readonly BinaryFormatter _formatter = new();

#pragma warning disable SYSLIB0011
        private static T Reserialize<T>(T value)
        {
            using var buffer = new MemoryStream();
            _formatter.Serialize(buffer, value!);
            buffer.Seek(0L, SeekOrigin.Begin);
            return (T)_formatter.Deserialize(buffer);
        }
#pragma warning restore SYSLIB0011

        [Theory]
        [InlineData((byte)0)]
        [InlineData((byte)2)]
        [InlineData((byte)25)]
        [InlineData((byte)255)]
        public void UInt8Success(byte value)
        {
            var expected = value.ToString();
            Span<char> span = stackalloc char[3];
            var length = Emplacer.GetDefault<byte>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<byte>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((ushort)0)]
        [InlineData((ushort)6)]
        [InlineData((ushort)65)]
        [InlineData((ushort)655)]
        [InlineData((ushort)6553)]
        [InlineData((ushort)65535)]
        public void UInt16Success(ushort value)
        {
            var expected = value.ToString();
            Span<char> span = stackalloc char[5];
            var length = Emplacer.GetDefault<ushort>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<ushort>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((uint)0)]
        [InlineData((uint)4)]
        [InlineData((uint)42)]
        [InlineData((uint)429)]
        [InlineData((uint)4294)]
        [InlineData((uint)42949)]
        [InlineData((uint)429496)]
        [InlineData((uint)4294967)]
        [InlineData((uint)42949672)]
        [InlineData((uint)429496729)]
        [InlineData((uint)4294967295)]
        public void UInt32Success(uint value)
        {
            var expected = value.ToString();
            Span<char> span = stackalloc char[10];
            var length = Emplacer.GetDefault<uint>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<uint>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((ulong)0ul)]
        [InlineData((ulong)4ul)]
        [InlineData((ulong)42ul)]
        [InlineData((ulong)429ul)]
        [InlineData((ulong)4294ul)]
        [InlineData((ulong)42949ul)]
        [InlineData((ulong)429496ul)]
        [InlineData((ulong)4294967ul)]
        [InlineData((ulong)42949672ul)]
        [InlineData((ulong)429496729ul)]
        [InlineData((ulong)4294967295ul)]
        [InlineData((ulong)18446744073709551615ul)]
        public void UInt64Success(ulong value)
        {
            var expected = value.ToString();
            Span<char> span = stackalloc char[20];
            var length = Emplacer.GetDefault<ulong>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<ulong>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((sbyte)-127)]
        [InlineData((sbyte)-12)]
        [InlineData((sbyte)-1)]
        [InlineData((sbyte)0)]
        [InlineData((sbyte)1)]
        [InlineData((sbyte)12)]
        [InlineData((sbyte)127)]
        public void Int8Success(sbyte value)
        {
            var expected = value.ToString();
            Span<char> span = stackalloc char[4];
            var length = Emplacer.GetDefault<sbyte>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<sbyte>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((short)-32768)]
        [InlineData((short)-3276)]
        [InlineData((short)-327)]
        [InlineData((short)-32)]
        [InlineData((short)-3)]
        [InlineData((short)0)]
        [InlineData((short)3)]
        [InlineData((short)32)]
        [InlineData((short)327)]
        [InlineData((short)3276)]
        [InlineData((short)32767)]
        public void Int16Success(short value)
        {
            var expected = value.ToString();
            Span<char> span = stackalloc char[6];
            var length = Emplacer.GetDefault<short>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<short>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData((int)-2147483648)]
        [InlineData((int)-32768)]
        [InlineData((int)-3276)]
        [InlineData((int)-327)]
        [InlineData((int)-32)]
        [InlineData((int)-3)]
        [InlineData((int)0)]
        [InlineData((int)3)]
        [InlineData((int)32)]
        [InlineData((int)327)]
        [InlineData((int)3276)]
        [InlineData((int)32767)]
        [InlineData((int)2147483647)]
        public void Int32Success(int value)
        {
            var expected = value.ToString();
            Span<char> span = stackalloc char[11];
            var length = Emplacer.GetDefault<int>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<int>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-9223372036854775808L)]
        [InlineData((long)-32768)]
        [InlineData((long)-3276)]
        [InlineData((long)-327)]
        [InlineData((long)-32)]
        [InlineData((long)-3)]
        [InlineData((long)0)]
        [InlineData((long)3)]
        [InlineData((long)32)]
        [InlineData((long)327)]
        [InlineData((long)3276)]
        [InlineData((long)32767)]
        [InlineData(9223372036854775807L)]
        public void Int64Success(long value)
        {
            var expected = value.ToString();
            Span<char> span = stackalloc char[20];
            var length = Emplacer.GetDefault<long>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<long>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-100.0f, 3.0f, "G10")]
        [InlineData(-10.0f, 3.0f, "G9")]
        [InlineData(-10.0f, 4.0f, "G9")]
        [InlineData(-10.0f, 5.0f, "G9")]
        [InlineData(-10.0f, 6.0f, "G9")]
        [InlineData((float)0, (float)1, "G")]
        [InlineData(10.0f, 6.0f, "G9")]
        [InlineData(10.0f, 5.0f, "G9")]
        [InlineData(10.0f, 4.0f, "G9")]
        [InlineData(10.0f, 3.0f, "G9")]
        [InlineData(100.0f, 3.0f, "G10")]
        public void SingleSuccess(float a, float b, string format)
        {
            var value = a / b;
            var expected = value.ToString(format, CultureInfo.InvariantCulture);
            Span<char> span = stackalloc char[40];
            var length = Emplacer.GetDefault<float>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<float>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
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
        public void DoubleSuccess(double a, double b, string format)
        {
            var value = a / b;
            var expected = value.ToString(format, CultureInfo.InvariantCulture);
            Span<char> span = stackalloc char[40];
            var length = Emplacer.GetDefault<double>().Emplace(value, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<double>().TryEmplace(value, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringSuccess()
        {
            var expected = "0123456789";
            Span<char> span = stackalloc char[10];
            var length = Emplacer.GetDefault<string>().Emplace(expected, span);
            var actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(expected, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.GetDefault<string>().TryEmplace(expected, span, out length));
            actual = span[..length].ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CharSuccess()
        {
            var expected = '0';
            Span<char> span = stackalloc char[1];
            var length = Emplacer.GetDefault<char>().Emplace(expected, span);
            var actual = span[0];
            Assert.Equal(expected, actual);
            Assert.True(Emplacer.TryEmplace(expected, span, out length));
            actual = span[0];
            Assert.Equal(expected, actual);
            span[0] = '\0';
            Assert.True(Emplacer.GetDefault<char>().TryEmplace(expected, span, out length));
            actual = span[0];
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GenericSuccess()
        {
            Span<char> span = stackalloc char[20];
            var ivalue = new Box<int>(2);
            var iexpected = ivalue.ToString();
            var ilength = Emplacer.GetDefault<Box<int>>().Emplace(ivalue, span);
            var iactual = span[..ilength].ToString();
            Assert.Equal(iexpected, iactual);
            ilength = Emplacer.Emplace(ivalue, span);
            iactual = span[..ilength].ToString();
            Assert.Equal(iexpected, iactual);
            var nvalue = new Box<string?>(null);
            var nexpected = "";
            var nlength = Emplacer.GetDefault<Box<string?>>().Emplace(nvalue, span);
            var nactual = span[..nlength].ToString();
            Assert.Equal(nexpected, nactual);
            var xexpected = "";
            var xlength = Emplacer.GetDefault<Box<string?>>().Emplace(null!, span);
            var xactual = span[..xlength].ToString();
            Assert.Equal(xexpected, xactual);
        }

        [Fact]
        public void EmplaceableSuccess()
        {
            {
                Span<char> span = stackalloc char[20];
                var ivalue = new EmplaceableBox<int>(2);
                var iexpected = ivalue.ToString();
                var ilength = Emplacer.GetDefault<EmplaceableBox<int>>().Emplace(ivalue, span);
                var iactual = span[..ilength].ToString();
                Assert.Equal(iexpected, iactual);
            }
            {
                Span<char> span = stackalloc char[20];
                var ivalue = new EmplaceableBox<int>(2);
                var iexpected = ivalue.ToString();
                Assert.True(Emplacer.GetDefault<EmplaceableBox<int>>().TryEmplace(ivalue, span, out var ilength));
                var iactual = span[..ilength].ToString();
                Assert.Equal(iexpected, iactual);
            }
            {
                Span<char> span = stackalloc char[20];
                var nvalue = new EmplaceableBox<string?>(null);
                var nexpected = "";
                var nlength = Emplacer.GetDefault<EmplaceableBox<string?>>().Emplace(nvalue, span);
                var nactual = span[..nlength].ToString();
                Assert.Equal(nexpected, nactual);
            }
        }

        [Theory]
        [InlineData((byte)0)]
        [InlineData((byte)2)]
        [InlineData((byte)25)]
        [InlineData((byte)255)]
        public void UInt8Failure(byte value)
        {
            var expected = value.ToString();
            var exn = Assert.Throws<InsufficientBufferSizeException>(() =>
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                Emplacer.GetDefault<byte>().Emplace(value, span);
            });
            Assert.Equal(expected.Length - 1, exn.SizeAvailable);
            Assert.Equal(expected.Length, exn.SizeRequired);
            Span<char> span = stackalloc char[expected.Length - 1];
            Assert.False(Emplacer.TryEmplace(value, span, out var length));
            Assert.Equal(0, length);
        }

        [Theory]
        [InlineData((ulong)0)]
        [InlineData((ulong)2)]
        [InlineData((ulong)25)]
        [InlineData((ulong)255)]
        public void UInt64Failure(ulong value)
        {
            var expected = value.ToString();
            var exn = Assert.Throws<InsufficientBufferSizeException>(() =>
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                Emplacer.GetDefault<ulong>().Emplace(value, span);
            });
            Assert.Equal(expected.Length - 1, exn.SizeAvailable);
            Assert.Equal(expected.Length, exn.SizeRequired);
            Span<char> span = stackalloc char[expected.Length - 1];
            Assert.False(Emplacer.TryEmplace(value, span, out var length));
            Assert.Equal(0, length);
        }

        [Theory]
        [InlineData(-9223372036854775808L)]
        [InlineData((long)-32768)]
        [InlineData((long)-3276)]
        [InlineData((long)-327)]
        [InlineData((long)-32)]
        [InlineData((long)-3)]
        [InlineData((long)0)]
        [InlineData((long)3)]
        [InlineData((long)32)]
        [InlineData((long)327)]
        [InlineData((long)3276)]
        [InlineData((long)32767)]
        [InlineData(9223372036854775807L)]
        public void Int64Failure(long value)
        {
            var expected = value.ToString();
            var exn = Assert.Throws<InsufficientBufferSizeException>(() =>
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                Emplacer.GetDefault<long>().Emplace(value, span);
            });
            Assert.Equal(expected.Length - 1, exn.SizeAvailable);
            Assert.Equal(expected.Length, exn.SizeRequired);
            Span<char> span = stackalloc char[expected.Length - 1];
            Assert.False(Emplacer.TryEmplace(value, span, out var length));
            Assert.Equal(0, length);
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
        public void SingleFailure(float a, float b, string format)
        {
            var value = a / b;
            var expected = value.ToString(format, CultureInfo.InvariantCulture);
            var exn = Assert.Throws<InsufficientBufferSizeException>(() =>
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                Emplacer.GetDefault<float>().Emplace(value, span);
            });
            Assert.Equal(expected.Length - 1, exn.SizeAvailable);
            Assert.Equal(expected.Length, exn.SizeRequired);
            Span<char> span = stackalloc char[expected.Length - 1];
            Assert.False(Emplacer.TryEmplace(value, span, out var length));
            Assert.Equal(0, length);
            Assert.False(Emplacer.TryEmplace(value, span, SingleEmplacer.DefaultMaxPrecision, out length));
            Assert.Equal(0, length);
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
        public void DoubleFailure(double a, double b, string format)
        {
            var value = a / b;
            var expected = value.ToString(format, CultureInfo.InvariantCulture);
            var exn = Assert.Throws<InsufficientBufferSizeException>(() =>
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                Emplacer.GetDefault<double>().Emplace(value, span);
            });
            Assert.Equal(expected.Length - 1, exn.SizeAvailable);
            Assert.Equal(expected.Length, exn.SizeRequired);
            Span<char> span = stackalloc char[expected.Length - 1];
            Assert.False(Emplacer.TryEmplace(value, span, out var length));
            Assert.Equal(0, length);
            Assert.False(Emplacer.TryEmplace(value, span, DoubleEmplacer.DefaultMaxPrecision, out length));
            Assert.Equal(0, length);
        }

        [Fact]
        public void StringFailure()
        {
            var expected = "0123456789";
            var exn = Assert.Throws<InsufficientBufferSizeException>(() =>
            {
                Span<char> span = stackalloc char[9];
                Emplacer.GetDefault<string>().Emplace(expected, span);
            });
            Assert.Equal(9, exn.SizeAvailable);
            Assert.Equal(10, exn.SizeRequired);
            Span<char> span = stackalloc char[9];
            Assert.False(Emplacer.TryEmplace(expected, span, out var length));
            Assert.Equal(0, length);
        }

        [Fact]
        public void CharFailure()
        {
            var exn = Assert.Throws<InsufficientBufferSizeException>(() =>
            {
                Span<char> span = stackalloc char[0];
                Emplacer.GetDefault<char>().Emplace('x', span);
            });
            Assert.Equal(0, exn.SizeAvailable);
            Assert.Equal(1, exn.SizeRequired);
            Span<char> span = stackalloc char[0];
            Assert.False(Emplacer.TryEmplace('x', span, out var length));
            Assert.Equal(0, length);
            Assert.False(Emplacer.GetDefault<char>().TryEmplace('x', span, out length));
            Assert.Equal(0, length);
        }

        [Fact]
        public void GenericFailure()
        {
            Assert.Throws<InsufficientBufferSizeException>(() =>
            {
                Span<char> span = stackalloc char[1];
                Emplacer.GetDefault<Box<int>>().Emplace(new Box<int>(22), span);
            });
            Span<char> span = stackalloc char[1];
            Assert.False(Emplacer.TryEmplace(new Box<int>(22), span, out var length));
            Assert.Equal(0, length);
        }

        [Fact]
        public void EmplaceableFailure()
        {
            var exn = Assert.Throws<InsufficientBufferSizeException>(() =>
            {
                Span<char> span = stackalloc char[1];
                Emplacer.GetDefault<EmplaceableBox<int>>().Emplace(new EmplaceableBox<int>(22), span);
            });
            Assert.Equal(1, exn.SizeAvailable);
            Span<char> span = stackalloc char[1];
            Assert.False(Emplacer.TryEmplace(new EmplaceableBox<int>(22), span, out var length));
            Assert.Equal(0, length);
        }

        [Fact]
        public void ReserializeException()
        {
            var exn0 = new InsufficientBufferSizeException("message");
            var exn1 = new InsufficientBufferSizeException(10);
            var exn2 = new InsufficientBufferSizeException(10, 20);
            var e0 = Reserialize(exn0);
            Assert.Equal(exn0.Message, e0.Message);
            Assert.False(e0.SizeAvailable.HasValue);
            Assert.False(e0.SizeRequired.HasValue);
            var e1 = Reserialize(exn1);
            Assert.Equal(exn1.Message, e1.Message);
            Assert.True(e1.SizeAvailable.HasValue);
            Assert.False(e1.SizeRequired.HasValue);
            Assert.Equal(exn1.SizeAvailable, e1.SizeAvailable);
            var e2 = Reserialize(exn2);
            Assert.Equal(exn2.Message, e2.Message);
            Assert.True(e2.SizeAvailable.HasValue);
            Assert.True(e2.SizeRequired.HasValue);
            Assert.Equal(exn2.SizeAvailable, e2.SizeAvailable);
            Assert.Equal(exn2.SizeRequired, e2.SizeRequired);
        }
    }
}