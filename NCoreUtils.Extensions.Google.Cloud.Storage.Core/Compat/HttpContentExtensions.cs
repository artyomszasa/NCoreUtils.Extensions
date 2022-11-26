using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http;

public static class HttpContentExtensions
{
    public static Task CopyToAsync(this HttpContent content, Stream destination, CancellationToken _)
        => content.CopyToAsync(destination);
}