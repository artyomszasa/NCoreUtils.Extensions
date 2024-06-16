using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

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
    bool? defaultEventBasedHold = default)
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

    // [JsonPropertyName("timeCreated")]
    // public DateTimeOffset TimeCreated { get; }

    // FIXME: updated
}