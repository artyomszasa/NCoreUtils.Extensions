using System;
using System.Globalization;
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

            public override string ToString() => Value?.ToString();
        }

        sealed class EmplaceableBox<T> : IEmplaceable<EmplaceableBox<T>>
        {
            public T Value { get; }

            public EmplaceableBox(T value) => Value = value;

            public override string ToString() => Value?.ToString();

            public int Emplace(Span<char> span)
            {
                return Emplacer.GetDefault<T>().Emplace(Value, span);
            }

            public bool TryEmplace(Span<char> span, out int used)
            {
                return Emplacer.GetDefault<T>().TryEmplace(Value, span, out used);
            }
        }

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
            var actual = span.Slice(0, length).ToString();
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
            var actual = span.Slice(0, length).ToString();
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
            var actual = span.Slice(0, length).ToString();
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
            var actual = span.Slice(0, length).ToString();
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
            var actual = span.Slice(0, length).ToString();
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
            var actual = span.Slice(0, length).ToString();
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
            var actual = span.Slice(0, length).ToString();
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
            var actual = span.Slice(0, length).ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-100.0f, 3.0f, "G10")]
        [InlineData(-10.0f, 3.0f, "G9")]
        [InlineData(-10.0f, 4.0f, "G9")]
        [InlineData(-10.0f, 6.0f, "G9")]
        [InlineData((float)0, (float)1, "G")]
        [InlineData(10.0f, 6.0f, "G9")]
        [InlineData(10.0f, 4.0f, "G9")]
        [InlineData(10.0f, 3.0f, "G9")]
        [InlineData(100.0f, 3.0f, "G10")]
        public void SingleSuccess(float a, float b, string format)
        {
            var value = a / b;
            var expected = value.ToString(format, CultureInfo.InvariantCulture);
            Span<char> span = stackalloc char[40];
            var length = Emplacer.GetDefault<float>().Emplace(value, span);
            var actual = span.Slice(0, length).ToString();
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-100.0, 3.0, "G10")]
        [InlineData(-10.0, 3.0, "G9")]
        [InlineData(-10.0, 4.0, "G9")]
        [InlineData(-10.0, 6.0, "G9")]
        [InlineData((double)0, (double)1, "G")]
        [InlineData(10.0, 6.0, "G9")]
        [InlineData(10.0, 4.0, "G9")]
        [InlineData(10.0, 3.0, "G9")]
        [InlineData(100.0, 3.0, "G10")]
        public void DoubleSuccess(double a, double b, string format)
        {
            var value = a / b;
            var expected = value.ToString(format, CultureInfo.InvariantCulture);
            Span<char> span = stackalloc char[40];
            var length = Emplacer.GetDefault<double>().Emplace(value, span);
            var actual = span.Slice(0, length).ToString();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void StringSuccess()
        {
            var expected = "0123456789";
            Span<char> span = stackalloc char[10];
            var length = Emplacer.GetDefault<string>().Emplace(expected, span);
            var actual = span.Slice(0, length).ToString();
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
        }

        [Fact]
        public void GenericSuccess()
        {
            Span<char> span = stackalloc char[20];
            var ivalue = new Box<int>(2);
            var iexpected = ivalue.ToString();
            var ilength = Emplacer.GetDefault<Box<int>>().Emplace(ivalue, span);
            var iactual = span.Slice(0, ilength).ToString();
            Assert.Equal(iexpected, iactual);
            var nvalue = new Box<string>(null);
            var nexpected = "";
            var nlength = Emplacer.GetDefault<Box<string>>().Emplace(nvalue, span);
            var nactual = span.Slice(0, nlength).ToString();
            Assert.Equal(nexpected, nactual);
            var xexpected = "";
            var xlength = Emplacer.GetDefault<Box<string>>().Emplace(null, span);
            var xactual = span.Slice(0, xlength).ToString();
            Assert.Equal(xexpected, xactual);
        }

        [Fact]
        public void EmplaceableSuccess()
        {
            Span<char> span = stackalloc char[20];
            var ivalue = new EmplaceableBox<int>(2);
            var iexpected = ivalue.ToString();
            var ilength = Emplacer.GetDefault<EmplaceableBox<int>>().Emplace(ivalue, span);
            var iactual = span.Slice(0, ilength).ToString();
            Assert.Equal(iexpected, iactual);
            var nvalue = new EmplaceableBox<string>(null);
            var nexpected = "";
            var nlength = Emplacer.GetDefault<EmplaceableBox<string>>().Emplace(nvalue, span);
            var nactual = span.Slice(0, nlength).ToString();
            Assert.Equal(nexpected, nactual);
        }

        [Theory]
        [InlineData((byte)0)]
        [InlineData((byte)2)]
        [InlineData((byte)25)]
        [InlineData((byte)255)]
        public void UInt8Failure(byte value)
        {
            var expected = value.ToString();
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                Emplacer.GetDefault<byte>().Emplace(value, span);
            });
        }

        [Theory]
        [InlineData((ulong)0)]
        [InlineData((ulong)2)]
        [InlineData((ulong)25)]
        [InlineData((ulong)255)]
        public void UInt64Failure(ulong value)
        {
            var expected = value.ToString();
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                Emplacer.GetDefault<ulong>().Emplace(value, span);
            });
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
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<char> span = stackalloc char[expected.Length - 1];
                Emplacer.GetDefault<long>().Emplace(value, span);
            });
        }

        [Fact]
        public void CharFailure()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<char> span = stackalloc char[0];
                Emplacer.GetDefault<char>().Emplace('x', span);
            });
        }

        [Fact]
        public void GenericFailure()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Span<char> span = stackalloc char[1];
                Emplacer.GetDefault<Box<int>>().Emplace(new Box<int>(22), span);
            });
        }
    }
}