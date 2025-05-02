using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class GoogleBucketAccessControl(
    string kind = "storage#bucketAccessControl",
    string? id = default,
    string? selfLink = default,
    string? bucket = default,
    string? entity = default,
    string? role = default,
    string? email = default,
    string? domain = default,
    string? entityId = default,
    string? etag = default,
    GoogleBucketAccessControlProjectTeam? projectTeam = default)
{
    [JsonPropertyName("kind")]
    public string Kind { get; } = kind;

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; } = id;

    [JsonPropertyName("selfLink")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SelfLink { get; } = selfLink;

    [JsonPropertyName("bucket")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Bucket { get; } = bucket;

    [JsonPropertyName("entity")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Entity { get; } = entity;

    [JsonPropertyName("role")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Role { get; } = role;

    [JsonPropertyName("email")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Email { get; } = email;

    [JsonPropertyName("domain")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Domain { get; } = domain;

    [JsonPropertyName("entityId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? EntityId { get; } = entityId;

    [JsonPropertyName("etag")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Etag { get; } = etag;

    [JsonPropertyName("projectTeam")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public GoogleBucketAccessControlProjectTeam? ProjectTeam { get; } = projectTeam;
}