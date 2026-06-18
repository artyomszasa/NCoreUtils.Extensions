using System;

namespace NCoreUtils;

public static partial class Base64
{
    private interface IBase64EncodeLookup
    {
        char Lookup(int value);
    }

    private readonly struct StandardBase64EncodeLookup : IBase64EncodeLookup
    {
        private static ReadOnlySpan<char> EncodeTable =>
        [
            'A', // 0x00
            'B', // 0x01
            'C', // 0x02
            'D', // 0x03
            'E', // 0x04
            'F', // 0x05
            'G', // 0x06
            'H', // 0x07
            'I', // 0x08
            'J', // 0x09
            'K', // 0x0a
            'L', // 0x0b
            'M', // 0x0c
            'N', // 0x0d
            'O', // 0x0e
            'P', // 0x0f
            'Q', // 0x10
            'R', // 0x11
            'S', // 0x12
            'T', // 0x13
            'U', // 0x14
            'V', // 0x15
            'W', // 0x16
            'X', // 0x17
            'Y', // 0x18
            'Z', // 0x19
            'a', // 0x1a
            'b', // 0x1b
            'c', // 0x1c
            'd', // 0x1d
            'e', // 0x1e
            'f', // 0x1f
            'g', // 0x20
            'h', // 0x21
            'i', // 0x22
            'j', // 0x23
            'k', // 0x24
            'l', // 0x25
            'm', // 0x26
            'n', // 0x27
            'o', // 0x28
            'p', // 0x29
            'q', // 0x2a
            'r', // 0x2b
            's', // 0x2c
            't', // 0x2d
            'u', // 0x2e
            'v', // 0x2f
            'w', // 0x30
            'x', // 0x31
            'y', // 0x32
            'z', // 0x33
            '0', // 0x34
            '1', // 0x35
            '2', // 0x36
            '3', // 0x37
            '4', // 0x38
            '5', // 0x39
            '6', // 0x3a
            '7', // 0x3b
            '8', // 0x3c
            '9', // 0x3d
            '+', // 0x3e
            '/', // 0x3f
        ];

        public char Lookup(int value)
        {
            if (value > 0x3f)
            {
                throw new InvalidOperationException("Should never happen: invalid encoded value.");
            }
            return EncodeTable[value];
        }
    }

    private static bool TryEncodeCore<TEncoder>(TEncoder encoder, ReadOnlySpan<byte> data, Span<char> buffer, out int written, bool pad)
        where TEncoder : IBase64EncodeLookup
    {
        var (fullChunkCount, reminder) = DivRem(unchecked((uint)data.Length), 3);
        var targetSize = pad
            ? reminder == 0 ? fullChunkCount * 4 : (fullChunkCount + 1) * 4
            : fullChunkCount * 4 + (reminder switch { 1 => 2, 2 => 3, _ => 0 });
        if (buffer.Length < targetSize)
        {
            written = default;
            return false;
        }
        var bufferOffset = 0;
        // process full chunks
        for (var i = 0; i < fullChunkCount; ++i)
        {
            var offset = i * 3;
            var b0 = data[offset++];
            var b1 = data[offset++];
            var b2 = data[offset];
            var bch0 = b0 >> 2;
            int bch1 = ((b0 & 0x03) << 4) | (b1 >> 4);
            int bch2 = ((b1 & 0x0F) << 2) | (b2 >> 6);
            int bch3 = b2 & 0x3F;
            buffer[bufferOffset++] = encoder.Lookup(bch0);
            buffer[bufferOffset++] = encoder.Lookup(bch1);
            buffer[bufferOffset++] = encoder.Lookup(bch2);
            buffer[bufferOffset++] = encoder.Lookup(bch3);
        }
        switch (reminder)
        {
            case 1:
                var b = data[^1];
                buffer[bufferOffset++] = encoder.Lookup(b >> 2);
                buffer[bufferOffset++] = encoder.Lookup((b & 0x03) << 4);
                if (pad)
                {
                    buffer[bufferOffset++] = '=';
                    buffer[bufferOffset++] = '=';
                }
                break;
            case 2:
                var b0 = data[^2];
                var b1 = data[^1];
                buffer[bufferOffset++] = encoder.Lookup(b0 >> 2);
                buffer[bufferOffset++] = encoder.Lookup(((b0 & 0x03) << 4) | (b1 >> 4));
                buffer[bufferOffset++] = encoder.Lookup((b1 & 0x0F) << 2);
                if (pad)
                {
                    buffer[bufferOffset++] = '=';
                }
                break;
        }
        written = bufferOffset;
        return true;
    }

    private static bool TryEncodeStandardNotPaddedCore(ReadOnlySpan<byte> data, Span<char> buffer, out int written)
        => TryEncodeCore(default(StandardBase64EncodeLookup), data, buffer, out written, false);

    private static bool TryEncodeStandardPaddedCore(ReadOnlySpan<byte> data, Span<char> buffer, out int written)
        => TryEncodeCore(default(StandardBase64EncodeLookup), data, buffer, out written, true);
}