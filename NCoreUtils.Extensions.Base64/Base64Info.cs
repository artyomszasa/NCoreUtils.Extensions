using System.Runtime.CompilerServices;

namespace NCoreUtils;

[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly ref struct Base64Info(Base64PropertiesWithValues properties, uint dataLength)
{
    public uint DataLength
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => dataLength;
    }

    public uint PaddingSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => properties.PaddingSize;
    }

    public Base64Alphabet Alphabet
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (ContainsInvalid)
            {
                return Base64Alphabet.Invalid;
            }
            if (properties.HasAnyFlag(Base64Properties.ContainsHypen, Base64Properties.ContainsUnderscope))
            {
                return properties.HasAnyFlag(Base64Properties.ContainsPlus, Base64Properties.ContainsSlash) ? Base64Alphabet.Invalid : Base64Alphabet.Url;
            }
            return Base64Alphabet.Standard;
        }
    }

    public bool IsValid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !(properties.HasFlag(Base64Properties.ContainsInvalid)
            || (properties.HasAnyFlag(Base64Properties.ContainsHypen, Base64Properties.ContainsUnderscope)
                && properties.HasAnyFlag(Base64Properties.ContainsPlus, Base64Properties.ContainsSlash)));
    }

    public bool IsAligned
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => properties.HasFlag(Base64Properties.Aligned);
    }

    public bool ContainsPadding
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => properties.HasFlag(Base64Properties.ContainsPadding);
    }

    public bool ContainsInvalid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => properties.HasFlag(Base64Properties.ContainsInvalid);
    }
}
