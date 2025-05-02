using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class GoogleBucketAccessControlProjectTeam(
    string? projectNumber = default,
    string? team = default)
{
    [JsonPropertyName("projectNumber")]
    public string? ProjectNumber { get; } = projectNumber;

    [JsonPropertyName("team")]
    public string? Team { get; } = team;
}
