using System;
using System.Runtime.CompilerServices;
using NCoreUtils.Memory;

namespace NCoreUtils
{
    public ref struct SpanBuilder
    {
        readonly Span<char> _span;

        public int Length { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SpanBuilder(Span<char> span)
        {
            _span = span;
            Length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append<T>(T value, IEmplacer<T> emplacer)
        {
            Length += emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append<T>(T value) => Append(value, Emplacer.GetDefault<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(Span<char> value)
        {
            value.CopyTo(_span.Slice(Length));
            Length += value.Length;
        }

        public void Append(byte value)
        {
            Length += UInt8Emplacer.Instance.Emplace(value, _span.Slice(Length));
        }

        public void Append(ushort value)
        {
            Length += UInt16Emplacer.Instance.Emplace(value, _span.Slice(Length));
        }

        public void Append(uint value)
        {
            Length += UInt32Emplacer.Instance.Emplace(value, _span.Slice(Length));
        }

        public void Append(ulong value)
        {
            Length += UInt64Emplacer.Instance.Emplace(value, _span.Slice(Length));
        }

        public void Append(sbyte value)
        {
            Length += Int8Emplacer.Instance.Emplace(value, _span.Slice(Length));
        }

        public void Append(short value)
        {
            Length += Int16Emplacer.Instance.Emplace(value, _span.Slice(Length));
        }

        public void Append(int value)
        {
            Length += Int32Emplacer.Instance.Emplace(value, _span.Slice(Length));
        }

        public void Append(long value)
        {
            Length += Int64Emplacer.Instance.Emplace(value, _span.Slice(Length));
        }

        public void Append(float value, int maxPrecision = 8, string decimalDelimiter = ".")
        {
            Length += SingleEmplacer.Emplace(value, _span.Slice(Length), maxPrecision, decimalDelimiter);
        }

        public void Append(double value, int maxPrecision = 8, string decimalDelimiter = ".")
        {
            Length += DoubleEmplacer.Emplace(value, _span.Slice(Length), maxPrecision, decimalDelimiter);
        }

        public void Append(char value)
        {
            _span[Length] = value;
            ++Length;
        }

        public void Append(string value)
        {
            Length += StringEmplacer.Instance.Emplace(value, _span.Slice(Length));
        }

        public override string ToString() => _span.Slice(0, Length).ToString();
    }
}