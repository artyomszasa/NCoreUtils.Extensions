using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class FileThumbnail(string? image = default, string? mimeType = default)
{
    [JsonPropertyName("image")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Image { get; } = image;

    [JsonPropertyName("mimeType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MimeType { get; } = mimeType;
}