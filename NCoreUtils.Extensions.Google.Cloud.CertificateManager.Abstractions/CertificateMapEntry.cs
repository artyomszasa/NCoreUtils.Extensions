using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class CertificateMapEntry(
    string name,
    string? description = default,
    IReadOnlyDictionary<string, string>? labels = default,
    IReadOnlyList<string>? certificates = default,
    string? state = default,
    string? hostname = default,
    DateTimeOffset? createTime = default,
    DateTimeOffset? updateTime = default)
{
    [JsonPropertyName("name")]
    public string Name { get; } = name;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; } = description;

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;

    [JsonPropertyName("certificates")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Certificates { get; } = certificates;

    [JsonPropertyName("state")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? State { get; } = state;

    [JsonPropertyName("hostname")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Hostname { get; } = hostname;

    [JsonPropertyName("createTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? CreateTime { get; } = createTime;

    [JsonPropertyName("updateTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? UpdateTime { get; } = updateTime;
}