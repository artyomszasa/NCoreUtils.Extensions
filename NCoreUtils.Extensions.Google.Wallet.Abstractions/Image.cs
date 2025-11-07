using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class Image(ImageUri sourceUri, LocalizedString? contentDescription)
{
    [JsonPropertyName("sourceUri")]
    public ImageUri SourceUri { get; } = sourceUri;

    [JsonPropertyName("contentDescription")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LocalizedString? ContentDescription { get; } = contentDescription;
}
