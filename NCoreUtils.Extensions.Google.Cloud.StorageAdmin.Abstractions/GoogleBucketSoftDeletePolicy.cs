using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class GoogleBucketSoftDeletePolicy(
    long? retentionDurationSeconds = default,
    DateTimeOffset? effectiveTime = default)
{
    [JsonPropertyName("retentionDurationSeconds")]
    [JsonConverter(typeof(PermssiveNullableInt64Converter))]
    public long? RetentionDurationSeconds { get; } = retentionDurationSeconds;

    [JsonPropertyName("effectiveTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    public DateTimeOffset? EffectiveTime { get; } = effectiveTime;
}
