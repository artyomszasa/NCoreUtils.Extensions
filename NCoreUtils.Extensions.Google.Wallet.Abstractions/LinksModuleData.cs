using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class LinksModuleData(IReadOnlyList<UriData> uris)
{
    [JsonPropertyName("uris")]
    public IReadOnlyList<UriData> Uris { get; } = uris;
}
