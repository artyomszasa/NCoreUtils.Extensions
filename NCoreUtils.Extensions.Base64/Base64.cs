using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace NCoreUtils;

public static partial class Base64
{
    #region options

    public struct BufferUsageOptions
    {
        private int _maxStackBufferSize;

        private int _maxPooledBufferSize;

        private int _maxStackCharBufferSize;

        private int _maxPooledCharBufferSize;

        public int MaxStackBufferSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => _maxStackBufferSize;
            set
            {
                Preconditions.ThrowIfLessThan(value, 0);
                _maxStackBufferSize = value;
                _maxStackCharBufferSize = value / sizeof(char);
            }
        }

        public int MaxPooledBufferSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => _maxPooledBufferSize;
            set
            {
                Preconditions.ThrowIfLessThan(value, 0);
                _maxPooledBufferSize = value;
                _maxPooledCharBufferSize = value / sizeof(char);
            }
        }

        public readonly int MaxStackCharBufferSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _maxStackCharBufferSize;
        }

        public readonly int MaxPooledCharBufferSize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _maxPooledCharBufferSize;
        }
    }

    public delegate void ConfigureBufferUsageDelegate(scoped ref BufferUsageOptions options);

    private static BufferUsageOptions Options = new()
    {
        MaxStackBufferSize = 512,
        MaxPooledBufferSize = 8 * 1024
    };

    private static int MaxStackBufferSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Options.MaxStackBufferSize;
    }

    private static int MaxPooledBufferSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Options.MaxPooledBufferSize;
    }

    private static int MaxStackCharBufferSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Options.MaxStackCharBufferSize;
    }

    private static int MaxPooledCharBufferSize
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Options.MaxPooledCharBufferSize;
    }

    public static void ConfigureBufferUsage(ConfigureBufferUsageDelegate configure)
        => configure(ref Options);

    #endregion
    private static void InspectMeaningfulCharacters(ReadOnlySpan<char> base64, ref Base64Properties properties)
    {
        foreach (var ch in base64)
        {
            switch (ch)
            {
                case '+':
                    properties |= Base64Properties.ContainsPlus;
                    break;
                case '/':
                    properties |= Base64Properties.ContainsSlash;
                    break;
                case '-':
                    properties |= Base64Properties.ContainsHypen;
                    break;
                case '_':
                    properties |= Base64Properties.ContainsUnderscope;
                    break;
                default:
                    if (!IsAsciiLetterOrDigit(ch))
                    {
                        properties |= Base64Properties.ContainsInvalid;
                        // TODO: return may be reasonable here...
                    }
                    break;
            }
        }
    }

    private static Base64Info Inspect(ReadOnlySpan<char> base64)
    {
        if (base64.IsEmpty)
        {
            return new(default, 0);
        }
        var properties = Base64Properties.None;
        var (chunks, remainder) = DivRem((uint)base64.Length, 4u);
        if (remainder == 0)
        {
            // fix size --> may contain padding
            properties |= Base64Properties.Aligned;
            if (base64[^1] == '=')
            {
                properties |= Base64Properties.ContainsPadding;
                if (base64[^2] == '=')
                {
                    InspectMeaningfulCharacters(base64[..^2], ref properties);
                    return new(new(properties, 2), chunks * 3 - 2);
                }
                InspectMeaningfulCharacters(base64[..^1], ref properties);
                return new(new(properties, 1), chunks * 3 - 1);
            }
            InspectMeaningfulCharacters(base64, ref properties);
            return new(new(properties, 0), chunks * 3);
        }
        // dynamic size --> must not contain padding
        InspectMeaningfulCharacters(base64, ref properties);
        return new(new (properties, remainder - 1), chunks * 3 + (remainder - 1));
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool TryDecodeCore(ReadOnlySpan<char> base64, Base64Info info, Span<byte> buffer, out int written)
    {
        if (unchecked((uint)buffer.Length) < info.DataLength)
        {
            written = default;
            return false;
        }
        switch (info.Alphabet)
        {
            case Base64Alphabet.Standard:
                // if data is padded then proceed without modifying...
                if (info.IsAligned)
                {
                    return TryDecodeStandardPadded(base64, buffer, out written);
                }
                // ...otherwise pad it and decode padded
                return TryDecodeStandardNotPadded(base64, buffer, out written);
            case Base64Alphabet.Url:
                if (info.ContainsPadding)
                {
                    // strip padding
                    return TryDecodeUrlPadded(base64, info.PaddingSize, buffer, out written);
                }
                return TryDecodeUrlNotPadded(base64, buffer, out written);
            default:
                written = default;
                return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryDecode(ReadOnlySpan<char> base64, Span<byte> buffer, out int written)
    {
        if (base64.IsEmpty)
        {
            written = 0;
            return true;
        }
        return TryDecodeCore(base64, Inspect(base64), buffer, out written);
    }

    public static int Decode(ReadOnlySpan<char> base64, Span<byte> buffer)
    {
        var info = Inspect(base64);
        if (info.Alphabet == Base64Alphabet.Invalid)
        {
            throw new InvalidOperationException("Could not decode base64 data: data is invalid.");
        }
        if (TryDecode(base64, buffer, out var written))
        {
            return written;
        }
        throw new InvalidOperationException("Could not decode base64 data: buffer is too small.");
    }

    public static byte[] Decode(ReadOnlySpan<char> base64)
    {
        var info = Inspect(base64);
        if (info.Alphabet == Base64Alphabet.Invalid)
        {
            throw new InvalidOperationException("Could not decode base64 data: data is invalid.");
        }
        if (info.DataLength == 0)
        {
            return [];
        }
        var result = new byte[info.DataLength];
        if (!TryDecodeCore(base64, info, result, out var written))
        {
            throw new InvalidOperationException("Could not decode base64 data: unexpected error.");
        }
        Debug.Assert(written == result.Length);
        return result;
    }

    public static bool TryEncode(ReadOnlySpan<byte> data, Span<char> buffer, out int written, Base64Alphabet alphabet = Base64Alphabet.Standard, bool? pad = default)
    {
        if (data.IsEmpty)
        {
            written = 0;
            return true;
        }
        if (alphabet == Base64Alphabet.Standard)
        {
            if (pad == false)
            {
                return TryEncodeStandardNotPaddedCore(data, buffer, out written);
            }
            return TryEncodeStandardPaddedCore(data, buffer, out written);
        }
        if (alphabet == Base64Alphabet.Url)
        {
            if (pad == true)
            {
                return TryEncodeUrlPaddedCore(data, buffer, out written);
            }
            return TryEncodeUrlNotPaddedCore(data, buffer, out written);
        }
        throw new ArgumentException($"Invalid base64 alphabet: {alphabet}.", nameof(alphabet));
    }

    public static int Encode(ReadOnlySpan<byte> data, Span<char> buffer, Base64Alphabet alphabet = Base64Alphabet.Standard, bool? pad = default)
    {
        if (!TryEncode(data, buffer, out var written, alphabet, pad))
        {
            throw new InvalidOperationException("Could not encode data: buffer is too small.");
        }
        return written;
    }

    public static char[] EncodeToChars(ReadOnlySpan<byte> data, Base64Alphabet alphabet = Base64Alphabet.Standard, bool? pad = default)
    {
        var (fullChunks, remainder) = DivRem((uint)data.Length, 3u);
        var shouldPad = pad ?? (alphabet != Base64Alphabet.Url);
        var bufferSize = unchecked((int)(shouldPad
            ? remainder == 0 ? fullChunks * 4 : (fullChunks + 1) * 4
            : fullChunks * 4 + (remainder switch { 1 => 2u, 2 => 3u, _ => 0u })));
        var array = new char[bufferSize];
        var written = Encode(data, array, alphabet, pad);
        Debug.Assert(array.Length == written);
        return array;
    }

    public static string EncodeToString(ReadOnlySpan<byte> data, Base64Alphabet alphabet = Base64Alphabet.Standard, bool? pad = default)
    {
        var (fullChunks, remainder) = DivRem((uint)data.Length, 3u);
        var shouldPad = pad ?? (alphabet != Base64Alphabet.Url);
        var bufferSize = unchecked((int)(shouldPad
            ? remainder == 0 ? fullChunks * 4 : (fullChunks + 1) * 4
            : fullChunks * 4 + (remainder switch { 1 => 2u, 2 => 3u, _ => 0u })));
        if (bufferSize <= MaxStackCharBufferSize)
        {
            Span<char> buffer = stackalloc char[bufferSize];
            return EncodeAndStringify(data, buffer, alphabet, shouldPad);
        }
        if (bufferSize <= MaxPooledCharBufferSize)
        {
            using var buffer = SpanOwner.Allocate<char>(bufferSize);
            return EncodeAndStringify(data, buffer, alphabet, shouldPad);
        }
        var array = new char[bufferSize];
        return EncodeAndStringify(data, array.AsSpan(0, bufferSize), alphabet, shouldPad);

        static string EncodeAndStringify(ReadOnlySpan<byte> data, Span<char> buffer, Base64Alphabet alphabet, bool pad)
        {
            var written = Encode(data, buffer, alphabet, pad);
            Debug.Assert(buffer.Length == written);
#if NETFRAMEWORK
            return buffer.ToString();
#else
            return new string(buffer);
#endif
        }
    }
}