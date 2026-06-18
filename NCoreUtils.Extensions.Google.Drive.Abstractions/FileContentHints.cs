using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class FileContentHints(string? indexableText = default, FileThumbnail? thumbnail = default)
{
    [JsonPropertyName("indexableText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? IndexableText { get; } = indexableText;

    [JsonPropertyName("thumbnail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileThumbnail? Thumbnail { get; } = thumbnail;
}