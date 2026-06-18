using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class FileLabelInfo(IReadOnlyList<object>? labels = default)
{
    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<object>? Labels { get; } = labels;
}