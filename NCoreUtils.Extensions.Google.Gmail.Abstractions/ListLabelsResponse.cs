using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

public class ListLabelsResponse(IReadOnlyList<Label>? labels)
{
    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<Label>? Labels { get; } = labels;
}