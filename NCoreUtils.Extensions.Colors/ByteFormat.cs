using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils.Colors;

public static class ByteFormat
{
    private static ReadOnlySpan<ushort> N2 => [0x03130,0x03131,0x03132,0x03133,0x03134,0x03135,0x03136,0x03137,0x03138,0x03139,0x03230,0x03231,0x03232,0x03233,0x03234,0x03235,0x03236,0x03237,0x03238,0x03239,0x03330,0x03331,0x03332,0x03333,0x03334,0x03335,0x03336,0x03337,0x03338,0x03339,0x03430,0x03431,0x03432,0x03433,0x03434,0x03435,0x03436,0x03437,0x03438,0x03439,0x03530,0x03531,0x03532,0x03533,0x03534,0x03535,0x03536,0x03537,0x03538,0x03539,0x03630,0x03631,0x03632,0x03633,0x03634,0x03635,0x03636,0x03637,0x03638,0x03639,0x03730,0x03731,0x03732,0x03733,0x03734,0x03735,0x03736,0x03737,0x03738,0x03739,0x03830,0x03831,0x03832,0x03833,0x03834,0x03835,0x03836,0x03837,0x03838,0x03839,0x03930,0x03931,0x03932,0x03933,0x03934,0x03935,0x03936,0x03937,0x03938,0x03939];

    private static ReadOnlySpan<uint> N3 => [0x0313030,0x0313031,0x0313032,0x0313033,0x0313034,0x0313035,0x0313036,0x0313037,0x0313038,0x0313039,0x0313130,0x0313131,0x0313132,0x0313133,0x0313134,0x0313135,0x0313136,0x0313137,0x0313138,0x0313139,0x0313230,0x0313231,0x0313232,0x0313233,0x0313234,0x0313235,0x0313236,0x0313237,0x0313238,0x0313239,0x0313330,0x0313331,0x0313332,0x0313333,0x0313334,0x0313335,0x0313336,0x0313337,0x0313338,0x0313339,0x0313430,0x0313431,0x0313432,0x0313433,0x0313434,0x0313435,0x0313436,0x0313437,0x0313438,0x0313439,0x0313530,0x0313531,0x0313532,0x0313533,0x0313534,0x0313535,0x0313536,0x0313537,0x0313538,0x0313539,0x0313630,0x0313631,0x0313632,0x0313633,0x0313634,0x0313635,0x0313636,0x0313637,0x0313638,0x0313639,0x0313730,0x0313731,0x0313732,0x0313733,0x0313734,0x0313735,0x0313736,0x0313737,0x0313738,0x0313739,0x0313830,0x0313831,0x0313832,0x0313833,0x0313834,0x0313835,0x0313836,0x0313837,0x0313838,0x0313839,0x0313930,0x0313931,0x0313932,0x0313933,0x0313934,0x0313935,0x0313936,0x0313937,0x0313938,0x0313939,0x0323030,0x0323031,0x0323032,0x0323033,0x0323034,0x0323035,0x0323036,0x0323037,0x0323038,0x0323039,0x0323130,0x0323131,0x0323132,0x0323133,0x0323134,0x0323135,0x0323136,0x0323137,0x0323138,0x0323139,0x0323230,0x0323231,0x0323232,0x0323233,0x0323234,0x0323235,0x0323236,0x0323237,0x0323238,0x0323239,0x0323330,0x0323331,0x0323332,0x0323333,0x0323334,0x0323335,0x0323336,0x0323337,0x0323338,0x0323339,0x0323430,0x0323431,0x0323432,0x0323433,0x0323434,0x0323435,0x0323436,0x0323437,0x0323438,0x0323439,0x0323530,0x0323531,0x0323532,0x0323533,0x0323534,0x0323535];

    private static ReadOnlySpan<char> HexLookup =>
    [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
        'A', 'B', 'C', 'D', 'E', 'F'
    ];

    [MethodImpl(Opt.Inline)]
    internal static void EmplaceHexNoCheck(ref char buf, byte value0, byte value1, byte value2)
    {
        ref var lookup = ref MemoryMarshal.GetReference(HexLookup);
#if NETSTANDARD2_0 || NETFRAMEWORK
        buf = Unsafe.Add(ref lookup, unchecked((int)((uint)value0 >> 4)));
        Unsafe.Add(ref buf, 1) = Unsafe.Add(ref lookup, unchecked((int)((uint)value0 & 0x0Fu)));
        Unsafe.Add(ref buf, 2) = Unsafe.Add(ref lookup, unchecked((int)((uint)value1 >> 4)));
        Unsafe.Add(ref buf, 3) = Unsafe.Add(ref lookup, unchecked((int)((uint)value1 & 0x0Fu)));
        Unsafe.Add(ref buf, 4) = Unsafe.Add(ref lookup, unchecked((int)((uint)value2 >> 4)));
        Unsafe.Add(ref buf, 5) = Unsafe.Add(ref lookup, unchecked((int)((uint)value2 & 0x0Fu)));
#else
        buf = Unsafe.Add(ref lookup, (nuint)value0 >> 4);
        Unsafe.Add(ref buf, 1) = Unsafe.Add(ref lookup, (nuint)value0 & 0x0F);
        Unsafe.Add(ref buf, 2) = Unsafe.Add(ref lookup, (nuint)value1 >> 4);
        Unsafe.Add(ref buf, 3) = Unsafe.Add(ref lookup, (nuint)value1 & 0x0F);
        Unsafe.Add(ref buf, 4) = Unsafe.Add(ref lookup, (nuint)value2 >> 4);
        Unsafe.Add(ref buf, 5) = Unsafe.Add(ref lookup, (nuint)value2 & 0x0F);
#endif
    }

    [MethodImpl(Opt.Inline)]
    internal static void EmplaceHexNoCheck(ref char buf, byte value0, byte value1, byte value2, byte value3)
    {
        ref var lookup = ref MemoryMarshal.GetReference(HexLookup);
#if NETSTANDARD2_0 || NETFRAMEWORK
        buf = Unsafe.Add(ref lookup, unchecked((int)((uint)value0 >> 4)));
        Unsafe.Add(ref buf, 1) = Unsafe.Add(ref lookup, unchecked((int)((uint)value0 & 0x0Fu)));
        Unsafe.Add(ref buf, 2) = Unsafe.Add(ref lookup, unchecked((int)((uint)value1 >> 4)));
        Unsafe.Add(ref buf, 3) = Unsafe.Add(ref lookup, unchecked((int)((uint)value1 & 0x0Fu)));
        Unsafe.Add(ref buf, 4) = Unsafe.Add(ref lookup, unchecked((int)((uint)value2 >> 4)));
        Unsafe.Add(ref buf, 5) = Unsafe.Add(ref lookup, unchecked((int)((uint)value2 & 0x0Fu)));
        Unsafe.Add(ref buf, 6) = Unsafe.Add(ref lookup, unchecked((int)((uint)value3 >> 4)));
        Unsafe.Add(ref buf, 7) = Unsafe.Add(ref lookup, unchecked((int)((uint)value3 & 0x0Fu)));
#else
        buf = Unsafe.Add(ref lookup, (nuint)value0 >> 4);
        Unsafe.Add(ref buf, 1) = Unsafe.Add(ref lookup, (nuint)value0 & 0x0F);
        Unsafe.Add(ref buf, 2) = Unsafe.Add(ref lookup, (nuint)value1 >> 4);
        Unsafe.Add(ref buf, 3) = Unsafe.Add(ref lookup, (nuint)value1 & 0x0F);
        Unsafe.Add(ref buf, 4) = Unsafe.Add(ref lookup, (nuint)value2 >> 4);
        Unsafe.Add(ref buf, 5) = Unsafe.Add(ref lookup, (nuint)value2 & 0x0F);
        Unsafe.Add(ref buf, 6) = Unsafe.Add(ref lookup, (nuint)value3 >> 4);
        Unsafe.Add(ref buf, 7) = Unsafe.Add(ref lookup, (nuint)value3 & 0x0F);
#endif
    }

    [MethodImpl(Opt.NoInlineOptimize)]
    public static bool TryFormat(byte value, Span<char> destination, out int charsWritten)
    {
        uint uvalue = value;
        if (uvalue < 10)
        {
            if (destination.Length < 1) { goto fail; }
            destination[0] = unchecked((char)(uvalue + '0'));
            charsWritten = 1;
            return true;
        }
        if (uvalue < 100)
        {
            if (destination.Length < 2) { goto fail; }
            var offset = unchecked((nuint)uvalue - 10);
            var chs = Unsafe.Add(
                ref MemoryMarshal.GetReference(N2),
#if NETSTANDARD2_0 || NETFRAMEWORK
                unchecked((int)offset)
#else
                offset
#endif
            );
            destination[0] = unchecked((char)((uint)chs >> 8));
            destination[1] = unchecked((char)(chs & 0X0FFu));
            charsWritten = 2;
            return true;
        }
        if (destination.Length < 3) { goto fail; }
        {
            var offset = unchecked((nuint)uvalue - 100);
            var chs = Unsafe.Add(
                ref MemoryMarshal.GetReference(N3),
#if NETSTANDARD2_0 || NETFRAMEWORK
                unchecked((int)offset)
#else
                offset
#endif
            );
            destination[0] = unchecked((char)(chs >> 16));
            destination[1] = unchecked((char)((chs >> 8) & 0x0FF));
            destination[2] = unchecked((char)(chs & 0xFF));
            charsWritten = 3;
            return true;
        }
    fail:
        charsWritten = default;
        return false;
    }
}