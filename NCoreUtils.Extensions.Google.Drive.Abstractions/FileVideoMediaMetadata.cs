using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class FileVideoMediaMetadata(int? width = default, int? height = default, long? durationMillis = default)
{
    [JsonPropertyName("width")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Width { get; } = width;

    [JsonPropertyName("height")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Height { get; } = height;

    [JsonPropertyName("durationMillis")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long? DurationMillis { get; } = durationMillis;
}