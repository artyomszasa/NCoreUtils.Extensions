using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class CreateBucketRequestAutoClass(bool enabled)
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; } = enabled;
}
