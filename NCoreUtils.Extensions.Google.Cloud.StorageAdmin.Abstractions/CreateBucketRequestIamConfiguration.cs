using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class CreateBucketRequestIamConfiguration(
    string? publicAccessPrevention,
    CreateBucketRequestIamConfigurationUniformBucketLevelAccess? uniformBucketLevelAccess)
{
    [JsonPropertyName("publicAccessPrevention")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PublicAccessPrevention { get; } = publicAccessPrevention;

    [JsonPropertyName("uniformBucketLevelAccess")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public CreateBucketRequestIamConfigurationUniformBucketLevelAccess? UniformBucketLevelAccess { get; } = uniformBucketLevelAccess;
}
