using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class CreateBucketRequestIamConfigurationUniformBucketLevelAccess(bool enabled)
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; } = enabled;
}
