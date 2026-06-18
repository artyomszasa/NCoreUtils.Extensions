using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class ThinkingConfig(
    bool? includeThoughts,
    int? thinkingBudget,
    string? thinkingLevel)
{
    [JsonPropertyName("includeThoughts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IncludeThoughts { get; } = includeThoughts;

    [JsonPropertyName("thinkingBudget")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ThinkingBudget { get; } = thinkingBudget;

    [JsonPropertyName("thinkingLevel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ThinkingLevel { get; } = thinkingLevel;
}
