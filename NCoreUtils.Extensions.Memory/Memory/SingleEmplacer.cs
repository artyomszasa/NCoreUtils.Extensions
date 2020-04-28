using System;
using System.Runtime.CompilerServices;

namespace NCoreUtils.Memory
{
    public sealed class SingleEmplacer : IEmplacer<float>
    {
        public static SingleEmplacer Default { get; } = new SingleEmplacer();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static int Emplace(float value, Span<char> span, int maxPrecision, string decimalSeparator = ".")
        {
            int length;
            if (0.0f == value)
            {
                if (span.Length < 1)
                {
                    throw new InvalidOperationException("Provided span must be at least 1 character long.");
                }
                length = 1;
                span[0] = '0';
            }
            else
            {
                Span<char> fbuffer = stackalloc char[maxPrecision];
                var uvalue = Math.Abs(value);
                // intgeral part
                var ivalue = (int)value;
                var isNegative = ivalue < 0 ? 1 : 0;
                var uivalue = Math.Abs(ivalue);
                // floating part
                var fvalue = uvalue - (float)uivalue;
                // intgeral part length...
                var ilength = (int)Math.Floor(Math.Log10(uivalue)) + 1 + isNegative;
                // stringify floating part locally to get value...
                var flength = 0;
                var flast = maxPrecision - 1;
                while (flength < maxPrecision)
                {
                    var v = fvalue * Math.Pow(10.0, (double)(flength + 1));
                    if (0.0 == v % 10.0)
                    {
                        break;
                    }
                    if (flength == flast)
                    {
                        fbuffer[flength] = I((int)Math.Round(v) % 10);
                    }
                    else
                    {
                        fbuffer[flength] = I((int)v % 10);
                    }
                    ++flength;
                }
                length = ilength + (flength == 0 ? 0 : flength + decimalSeparator.Length);
                if (span.Length < length)
                {
                    throw new InvalidOperationException($"Provided span must be at least {length} character(s) long.");
                }
                Int32Emplacer.Instance.Emplace(ivalue, span);
                if (flength > 0)
                {
                    decimalSeparator.AsSpan().CopyTo(span.Slice(ilength));
                    fbuffer.Slice(0, flength).CopyTo(span.Slice(ilength + decimalSeparator.Length));
                }
            }
            return length;

            static char I(int value) => (char)('0' + value);
        }

        public int MaxPrecision { get; }

        public string DecimalSeparator { get; }

        public SingleEmplacer(int maxPrecision = 8, string decimalSeparator = ".")
        {
            MaxPrecision = maxPrecision;
            DecimalSeparator = decimalSeparator;
        }

        public int Emplace(float value, Span<char> span)
            => Emplace(value, span, MaxPrecision, DecimalSeparator);
    }
}