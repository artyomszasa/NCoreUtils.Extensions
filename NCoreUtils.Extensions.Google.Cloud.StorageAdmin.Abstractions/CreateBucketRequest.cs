using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class CreateBucketRequest(
    string name,
    CreateBucketRequestAutoClass? autoclass = default,
    IReadOnlyList<CreateBucketRequestCors>? cors = default,
    CreateBucketRequestIamConfiguration? iamConfiguration = default,
    IReadOnlyDictionary<string, string>? labels = default,
    string? location = default,
    string? storageClass = default)
{
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Name { get; } = name switch
    {
        null => throw new ArgumentNullException(nameof(name)),
        "" => throw new ArgumentException("Bucket name must not be empty.", nameof(name)),
        var value => value
    };

    // FIXME: acl

    [JsonPropertyName("autoclass")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public CreateBucketRequestAutoClass? Autoclass { get; } = autoclass;

    [JsonPropertyName("cors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<CreateBucketRequestCors>? Cors { get; } = cors;

    [JsonPropertyName("iamConfiguration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public CreateBucketRequestIamConfiguration? IamConfiguration { get; } = iamConfiguration;

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, string>? Labels { get; } = labels;

    [JsonPropertyName("location")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Location { get; } = location;

    // FIXME: logging

    // FIXME: softDeletePolicy

    [JsonPropertyName("storageClass")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? StorageClass { get; } = storageClass;

    // FIXME: versioning
}