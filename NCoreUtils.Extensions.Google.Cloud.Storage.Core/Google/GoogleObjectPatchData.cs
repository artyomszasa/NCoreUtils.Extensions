using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class GoogleObjectPatchData
{
    // [JsonPropertyName("name")]
    // public string? Name { get; set; }

    [JsonPropertyName("contentDisposition")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentDisposition { get; set; }

    [JsonPropertyName("contentEncoding")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentEncoding { get; set; }

    [JsonPropertyName("contentLanguage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentLanguage { get; set; }

    [JsonPropertyName("contentType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContentType { get; set; }

    [JsonPropertyName("cacheControl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CacheControl { get; set; }

    [JsonPropertyName("metadata")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, string>? Metadata { get; set; }

    [JsonPropertyName("acl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<GoogleAccessControlEntry>? Acl { get; set; }
}