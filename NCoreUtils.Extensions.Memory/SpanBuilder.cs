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
        public void Append<T>(T value)
            => Append(value, Emplacer.GetDefault<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(Span<char> value)
        {
            value.CopyTo(_span.Slice(Length));
            Length += value.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(byte value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ushort value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(uint value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(ulong value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(sbyte value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(short value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(int value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(long value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(float value, int maxPrecision = 8, string decimalDelimiter = ".")
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length), maxPrecision, decimalDelimiter);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(double value, int maxPrecision = 8, string decimalDelimiter = ".")
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length), maxPrecision, decimalDelimiter);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(char value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(string value)
        {
            Length += Emplacer.Emplace(value, _span.Slice(Length));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend<T>(T value, IEmplacer<T> emplacer)
        {
            if (emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend<T>(T value)
            => TryAppend(value, Emplacer.GetDefault<T>());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(char value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(sbyte value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(short value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(int value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(long value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(byte value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(ushort value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(uint value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(ulong value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(double value, int maxPrecision = DoubleEmplacer.DefaultMaxPrecision, string decimalSeparator = DoubleEmplacer.DefaultDecimalSeparator)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), maxPrecision, decimalSeparator, out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(float value, int maxPrecision = SingleEmplacer.DefaultMaxPrecision, string decimalSeparator = SingleEmplacer.DefaultDecimalSeparator)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), maxPrecision, decimalSeparator, out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAppend(string value)
        {
            if (Emplacer.TryEmplace(value, _span.Slice(Length), out var used))
            {
                Length += used;
                return true;
            }
            return false;
        }

        public override string ToString() => _span.Slice(0, Length).ToString();
    }
}