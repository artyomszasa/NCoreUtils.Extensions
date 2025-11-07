using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

[method: JsonConstructor]
public readonly struct ImageUri(string uri)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ImageUri(string uri) => new(uri);

    [JsonPropertyName("uri")]
    public string Uri { get; } = uri;
}
