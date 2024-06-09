using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NCoreUtils;

public readonly partial struct YearAndMonth
    : IEquatable<YearAndMonth>
    , IComparable<YearAndMonth>
    , IComparable
    , IFormattable
{
    private const string Unrepresentable = "Operation would result in creating unrepresentable year/month combination.";

#if NET6_0_OR_GREATER

    private const MethodImplOptions OptInline = MethodImplOptions.AggressiveInlining;

    private const MethodImplOptions OptOptimize = MethodImplOptions.AggressiveOptimization;

    private const MethodImplOptions OptInlineOptimize = MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization;

    private const MethodImplOptions OptNoInlineOptimize = MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization;

#else

    private const MethodImplOptions OptInline = MethodImplOptions.AggressiveInlining;

    private const MethodImplOptions OptOptimize = default;

    private const MethodImplOptions OptInlineOptimize = MethodImplOptions.AggressiveInlining;

    private const MethodImplOptions OptNoInlineOptimize = MethodImplOptions.NoInlining;

#endif

    [MethodImpl(OptInline)]
    public static bool operator ==(YearAndMonth left, YearAndMonth right) => left.Equals(right);

    [MethodImpl(OptInline)]
    public static bool operator !=(YearAndMonth left, YearAndMonth right) => !left.Equals(right);

    [MethodImpl(OptInline)]
    public static bool operator <(YearAndMonth left, YearAndMonth right) => left.Data < right.Data;

    [MethodImpl(OptInline)]
    public static bool operator >(YearAndMonth left, YearAndMonth right) => left.Data > right.Data;

    [MethodImpl(OptInline)]
    public static bool operator <=(YearAndMonth left, YearAndMonth right) => left.Data <= right.Data;

    [MethodImpl(OptInline)]
    public static bool operator >=(YearAndMonth left, YearAndMonth right) => left.Data >= right.Data;

    public static YearAndMonth MinValue => new(1, 1);

    public static YearAndMonth MaxValue => new(ushort.MaxValue, 12);

    /// <summary>
    /// Packs two ushorts into an uint.
    /// </summary>
    /// <remarks>
    /// Using shifts would be even more efficient but in this case we have deal with endianness of the target system
    /// which is unknown at this point. Packing/Unpacking using explicit offset should be consistent on any platform.
    /// </remarks>
    /// <param name="month">Month value.</param>
    /// <param name="year">Year value.</param>
    /// <returns></returns>
    [MethodImpl(OptInlineOptimize)]
    private static uint Pack(ushort month, ushort year)
    {
        // initial initialization of res can be avoided once https://github.com/dotnet/csharplang/issues/1738 is completed
        uint res = default;
        {
            Unsafe.As<uint, ushort>(ref res) = month;
            Unsafe.Add(ref Unsafe.As<uint, ushort>(ref res), 1) = year;
        }
        return res;
    }

    private readonly uint Data;

    public ushort Month
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Unsafe.As<YearAndMonth, ushort>(ref Unsafe.AsRef(in this));
    }

    public ushort Year
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Unsafe.Add(ref Unsafe.As<YearAndMonth, ushort>(ref Unsafe.AsRef(in this)), 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private YearAndMonth(uint data)
        => Data = data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public YearAndMonth(ushort year, ushort month)
    {
        if (year is 0)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be between 1 and 65535.");
        }
        if (month is 0 || month > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
        }
        Data = Pack(month, year);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Deconstruct(out ushort year, out ushort month)
    {
        year = Year;
        month = Month;
    }

    #region equality

    [MethodImpl(OptInlineOptimize)]
    public bool Equals(YearAndMonth other)
        => Data == other.Data;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is YearAndMonth other && Equals(other);

    [MethodImpl(OptOptimize)]
    public override int GetHashCode()
        => unchecked((int)Data);

    #endregion

    #region comparison

    [MethodImpl(OptInlineOptimize)]
    public int CompareTo(YearAndMonth other)
        => Data == other.Data ? 0 : Data < other.Data ? -1 : 1;

    public int CompareTo(object? obj) => obj switch
    {
        YearAndMonth other => CompareTo(other),
        _ => throw new InvalidOperationException("obj must be instance of YearAndMonth"),
    };

    #endregion

    [MethodImpl(OptNoInlineOptimize)]
    public YearAndMonth AddYears(int years)
    {
        int newYear;
        if (years < 0)
        {
            newYear = Year + years;
            if (newYear <= 0)
            {
                goto Invalid;
            }
        }
        else
        {
            newYear = Year + years;
            if (newYear > ushort.MaxValue)
            {
                goto Invalid;
            }
        }
        return new(Pack(Month, unchecked((ushort)newYear)));
    Invalid:
        throw new InvalidOperationException(Unrepresentable);
    }

    [MethodImpl(OptNoInlineOptimize)]
    public YearAndMonth AddMonths(int months)
    {
        int newYear;
#if NET6_0_OR_GREATER
        var (years, month0) = Math.DivRem(Month - 1 + months, 12);
#else
        var month0 = Math.DivRem(Month - 1 + months, 12, out var years);
#endif
        if (months < 0)
        {

            if (month0 < 0)
            {
                --years;
                month0 += 12;
            }
            newYear = Year + years;
            if (newYear <= 0)
            {
                goto Invalid;
            }
        }
        else
        {
            newYear = Year + years;
            if (newYear > ushort.MaxValue)
            {
                goto Invalid;
            }
        }
        return new(Pack(unchecked((ushort)(month0 + 1)), unchecked((ushort)newYear)));
    Invalid:
        throw new InvalidOperationException(Unrepresentable);
    }

    #region formatting

    private static ReadOnlySpan<char> PackedMonths =>
    [
        '0', '1',
        '0', '2',
        '0', '3',
        '0', '4',
        '0', '5',
        '0', '6',
        '0', '7',
        '0', '8',
        '0', '9',
        '1', '0',
        '1', '1',
        '1', '2'
    ];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static char ToAsciiNum(int i) => unchecked((char)(i + '0'));

    [MethodImpl(OptInlineOptimize)]
    private int GetFormattedSize()
    {
        int year = Year;
        int yearSize = 5;
        if (year < 10_000) yearSize = 4;
        if (year < 1_000) yearSize = 3;
        if (year < 100) yearSize = 2;
        if (year < 10) yearSize = 1;
        return yearSize + 3;
    }

    [MethodImpl(OptInlineOptimize)]
    private int FormatToNoCheck(ref char buffer)
    {
        int year = Year;
        int yearSize = 5;
        if (year < 10_000) yearSize = 4;
        if (year < 1_000) yearSize = 3;
        if (year < 100) yearSize = 2;
        if (year < 10) yearSize = 1;
        var pos = yearSize - 1;
        while (pos > 0)
        {
            year = Math.DivRem(year, 10, out var rem);
            Unsafe.Add(ref buffer, pos--) = ToAsciiNum(rem);
        }
        buffer = ToAsciiNum(year);
        // Unsafe.Add(ref buffer, yearSize) = '-';
        buffer = ref Unsafe.Add(ref buffer, yearSize);
        buffer = '-';
        ref var monthChars = ref Unsafe.Add(
            ref MemoryMarshal.GetReference(PackedMonths),
            (Month - 1) * 2
        );
        // Unsafe.Add(ref buffer, yearSize + 1) = monthChars;
        buffer = ref Unsafe.Add(ref buffer, 1);
        buffer = monthChars;
        // Unsafe.Add(ref buffer, yearSize + 2) = Unsafe.Add(ref monthChars, 1);
        buffer = ref Unsafe.Add(ref buffer, 1);
        buffer = Unsafe.Add(ref monthChars, 1);
        return yearSize + 3;
    }

    [MethodImpl(OptNoInlineOptimize)]
    public override string ToString()
    {
        Span<char> buffer = stackalloc char[8];
        var size = FormatToNoCheck(ref MemoryMarshal.GetReference(buffer));
#if NET6_0_OR_GREATER
        return new string(buffer[..size]);
#else
        return buffer[..size].ToString();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(string? format, IFormatProvider? formatProvider)
        => ToString();

    [MethodImpl(OptNoInlineOptimize)]
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (destination.Length < GetFormattedSize())
        {
            charsWritten = 0;
            return false;
        }
        charsWritten = FormatToNoCheck(ref MemoryMarshal.GetReference(destination));
        return true;
    }

    #endregion
}