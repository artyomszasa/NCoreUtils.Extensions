using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[method: JsonConstructor]
public readonly struct DnsResourceRecord(string name, string type, string data)
{
    [JsonPropertyName("name")]
    public string Name { get; } = name;

    [JsonPropertyName("type")]
    public string Type { get; } = type;

    [JsonPropertyName("data")]
    public string Data { get; } = data;
}

public class DnsAuthorization(
    string name,
    string domain,
    IReadOnlyDictionary<string, string>? labels = default,
    string? description = default,
    DnsResourceRecord? dnsResourceRecord = default,
    string? type = default,
    DateTimeOffset? createTime = default,
    DateTimeOffset? updateTime = default)
{
    [JsonPropertyName("name")]
    public string Name { get; } = name;

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; } = description;

    [JsonPropertyName("domain")]
    public string Domain { get; } = domain;

    [JsonPropertyName("dnsResourceRecord")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DnsResourceRecord? DnsResourceRecord { get; } = dnsResourceRecord;

    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Type { get; } = type;

    [JsonPropertyName("createTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? CreateTime { get; } = createTime;

    [JsonPropertyName("updateTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? UpdateTime { get; } = updateTime;
}