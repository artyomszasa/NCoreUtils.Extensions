using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

internal readonly struct Base64PropertiesWithValues
    : IEquatable<Base64PropertiesWithValues>
{
    private const uint MaskPadding = 0x03u;

    private readonly uint _data;

    public uint PaddingSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => unchecked(_data & MaskPadding);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Base64PropertiesWithValues(uint data)
        => _data = data;

    public Base64PropertiesWithValues(Base64Properties value, uint paddingSize)
        : this(unchecked((uint)value | (paddingSize & MaskPadding)))
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Base64Properties ExtractProperties() => unchecked((Base64Properties)(_data & ~MaskPadding));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasFlag(Base64Properties value) => unchecked((Base64Properties)_data).HasFlag(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasAnyFlag(ReadOnlySpan<Base64Properties> values)
    {
        foreach (var value in values)
        {
            if (unchecked((Base64Properties)_data).HasFlag(value))
            {
                return true;
            }
        }
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasAnyFlag(Base64Properties value1, Base64Properties value2) => HasFlag(value1) || HasFlag(value2);

    #region equality

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Base64PropertiesWithValues other)
        => _data == other._data;

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is Base64PropertiesWithValues other && Equals(other);

    public override int GetHashCode()
        => _data.GetHashCode();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Base64PropertiesWithValues left, Base64PropertiesWithValues right)
        => left.Equals(right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Base64PropertiesWithValues left, Base64PropertiesWithValues right)
        => !(left == right);

    #endregion
}
