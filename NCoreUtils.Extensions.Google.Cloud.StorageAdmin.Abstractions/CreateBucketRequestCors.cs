using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class CreateBucketRequestCors(
    int? maxAgeSeconds = default,
    IReadOnlyList<string>? method = default,
    IReadOnlyList<string>? origin = default,
    IReadOnlyList<string>? responseHeader = default)
{
    [JsonPropertyName("maxAgeSeconds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MaxAgeSeconds { get; } = maxAgeSeconds;

    [JsonPropertyName("method")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Method { get; } = method;

    [JsonPropertyName("origin")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Origin { get; } = origin;

    [JsonPropertyName("responseHeader")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? ResponseHeader { get; } = responseHeader;
}
