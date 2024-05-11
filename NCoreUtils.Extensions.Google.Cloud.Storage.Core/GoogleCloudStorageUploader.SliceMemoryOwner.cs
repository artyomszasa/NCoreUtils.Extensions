using System.Buffers;

namespace NCoreUtils;

public partial class GoogleCloudStorageUploader
{
    private sealed class SliceMemoryOwner : IMemoryOwner<byte>
    {
        public IMemoryOwner<byte> Source { get; }

        public int Size { get; }

        public SliceMemoryOwner(IMemoryOwner<byte> source, int size)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            if (source.Memory.Length < size)
            {
                throw new ArgumentException("Supplied memory is smaller than the requested slice size.", nameof(source));
            }
            Size = size;
        }

        public Memory<byte> Memory
            => Source.Memory[..Size];

        public void Dispose()
            => Source.Dispose();
    }
}