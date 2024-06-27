using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[method:JsonConstructor]
public readonly struct ProvisioningIssue(string reason, string details)
{
    [JsonPropertyName("reason")]
    public string Reason { get; } = reason;

    [JsonPropertyName("details")]
    public string Details { get; } = details;
}

public class ManagedCertificate(
    IReadOnlyList<string>? domains = default,
    IReadOnlyList<string>? dnsAuthorizations = default,
    string? issuanceConfig = default,
    string? state = default,
    ProvisioningIssue? provisioningIssue = default)
{
    [JsonPropertyName("domains")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Domains { get; } = domains;

    [JsonPropertyName("dnsAuthorizations")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? DnsAuthorizations { get; } = dnsAuthorizations;

    [JsonPropertyName("issuanceConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? IssuanceConfig { get; } = issuanceConfig;

    [JsonPropertyName("state")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? State { get; } = state;

    [JsonPropertyName("provisioningIssue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ProvisioningIssue? ProvisioningIssue { get; } = provisioningIssue;

    // FIXME: authorizationAttemptInfo
}

public class Certificate(
    string name,
    string? description = default,
    IReadOnlyDictionary<string, string>? labels = default,
    IReadOnlyList<string>? sanDnsnames = default,
    string? pemCertificate = default,
    string? scope = default,
    DateTimeOffset? createTime = default,
    DateTimeOffset? updateTime = default,
    DateTimeOffset? expireTime = default,
    ManagedCertificate? managed = default)
{
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string Name { get; } = name;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; } = description;

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;

    [JsonPropertyName("sanDnsnames")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? SanDnsnames { get; } = sanDnsnames;

    [JsonPropertyName("pemCertificate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PemCertificate { get; } = pemCertificate;

    [JsonPropertyName("scope")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Scope { get; } = scope;

    [JsonPropertyName("createTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? CreateTime { get; } = createTime;

    [JsonPropertyName("updateTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? UpdateTime { get; } = updateTime;

    [JsonPropertyName("expireTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTimeOffset? ExpireTime { get; } = expireTime;

    [JsonPropertyName("managed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ManagedCertificate? Managed { get; } = managed;
}