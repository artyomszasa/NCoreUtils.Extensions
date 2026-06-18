using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class FileLinkShareMetadata(bool? securityUpdateEligible = default, bool? securityUpdateEnabled = default)
{
    [JsonPropertyName("securityUpdateEligible")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SecurityUpdateEligible { get; } = securityUpdateEligible;

    [JsonPropertyName("securityUpdateEnabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? SecurityUpdateEnabled { get; } = securityUpdateEnabled;
}