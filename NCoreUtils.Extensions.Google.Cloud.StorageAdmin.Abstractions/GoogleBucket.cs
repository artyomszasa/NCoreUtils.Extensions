using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class GoogleBucketOwner(
    string? entity = default,
    string? entityId = default)
{
    [JsonPropertyName("entity")]
    public string? Entity { get; } = entity;

    [JsonPropertyName("entityId")]
    public string? EntityId { get; } = entityId;
}

public class GoogleBucket(
    string kind = "storage#bucket",
    string? selfLink = default,
    string? id = default,
    string? name = default,
    string? projectNumber = default,
    string? metageneration = default,
    string? location = default,
    string? storageClass = default,
    string? eTag = default,
    bool? defaultEventBasedHold = default,
    DateTimeOffset? timeCreated = default,
    DateTimeOffset? updated = default,
    DateTimeOffset? softDeleteTime = default,
    DateTimeOffset? hardDeleteTime = default,
    IReadOnlyList<GoogleBucketAccessControl>? acl = default,
    GoogleBucketOwner? owner = default,
    IReadOnlyList<GoogleBucketCors>? cors = default,
    IReadOnlyDictionary<string, string>? labels = default,
    GoogleBucketSoftDeletePolicy? softDeletePolicy = default)
{
    [JsonPropertyName("kind")]
    public string Kind { get; } = kind;

    [JsonPropertyName("selfLink")]
    public string? SelfLink { get; } = selfLink;

    [JsonPropertyName("id")]
    public string? Id { get; } = id;

    [JsonPropertyName("name")]
    public string? Name { get; } = name;

    [JsonPropertyName("projectNumber")]
    public string? ProjectNumber { get; } = projectNumber;

    [JsonPropertyName("metageneration")]
    public string? Metageneration { get; } = metageneration;

    [JsonPropertyName("location")]
    public string? Location { get; } = location;

    [JsonPropertyName("storageClass")]
    public string? StorageClass { get; } = storageClass;

    [JsonPropertyName("etag")]
    public string? ETag { get; } = eTag;

    [JsonPropertyName("defaultEventBasedHold")]
    public bool? DefaultEventBasedHold { get; } = defaultEventBasedHold;

    [JsonPropertyName("timeCreated")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    public DateTimeOffset? TimeCreated { get; } = timeCreated;

    [JsonPropertyName("updated")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    public DateTimeOffset? Updated { get; } = updated;

    [JsonPropertyName("softDeleteTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    public DateTimeOffset? SoftDeleteTime { get; } = softDeleteTime;

    [JsonPropertyName("hardDeleteTime")]
    [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
    public DateTimeOffset? HardDeleteTime { get; } = hardDeleteTime;

    [JsonPropertyName("acl")]
    public IReadOnlyList<GoogleBucketAccessControl>? Acl { get; } = acl;

    // FIXME: hierarchicalNamespace

    // FIXME: encryption

    // FIXME: acl

    // FIXME: defaultObjectAcl

    [JsonPropertyName("owner")]
    public GoogleBucketOwner? Owner { get; } = owner;

    // FIXME: logging

    [JsonPropertyName("cors")]
    public IReadOnlyList<GoogleBucketCors>? Cors { get; } = cors;

    // FIXME: versioning

    // FIXME: lifecycle

    // FIXME: autoclass

    [JsonPropertyName("labels")]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;

    // FIXME: retentionPolicy

    // FIXME: objectRetention

    // FIXME: billing

    // FIXME: iamConfiguration

    // FIXME: ipFilter

    // FIXME: locationType

    // FIXME: customPlacementConfig

    [JsonPropertyName("softDeletePolicy")]
    public GoogleBucketSoftDeletePolicy? SoftDeletePolicy { get; } = softDeletePolicy;

    // FIXME: rpo
}