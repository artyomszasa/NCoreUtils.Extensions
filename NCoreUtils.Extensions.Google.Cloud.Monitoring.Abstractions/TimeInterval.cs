using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

[method: JsonConstructor]
public readonly struct TimeInterval(
    DateTimeOffset endTime,
    DateTimeOffset? startTime = default)
{
    [JsonPropertyName("endTime")]
    [JsonConverter(typeof(NonNullableRfc3339DateTimeOffsetConverter))]
    public DateTimeOffset EndTime { get; } = endTime;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    [JsonPropertyName("startTime")]
    public DateTimeOffset? StartTime { get; } = startTime;
}
