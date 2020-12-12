using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NCoreUtils
{
    public static class StreamCompleteReadExtensions
    {
#if !NETSTANDARD2_1
        private static async ValueTask<int> ReadAsync(
            this Stream stream,
            Memory<byte> buffer,
            CancellationToken cancellationToken = default)
        {
            var array = ArrayPool<byte>.Shared.Rent(Math.Min(32 * 1024, buffer.Length));
            try
            {
                var read = await stream.ReadAsync(array, 0, Math.Min(buffer.Length, array.Length));
                if (read > 0)
                {
                    array.AsSpan().Slice(0, read).CopyTo(buffer.Span);
                }
                return read;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }
#endif


        public static async ValueTask<int> ReadCompleteAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {

            var read = await stream.ReadAsync(buffer, cancellationToken);
            var total = read;
            while (read != 0 && total < buffer.Length)
            {
                read = await stream.ReadAsync(buffer.Slice(total), cancellationToken);
                total += read;
            }
            return total;
        }
    }
}