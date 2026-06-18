using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class Content(string role, IReadOnlyList<Part> parts)
{
    public static implicit operator Content(string userText) => new(role: "user", parts: [userText]);

    [JsonPropertyName("role")]
    public string Role { get; } = role;

    [JsonPropertyName("parts")]
    public IReadOnlyList<Part> Parts { get; } = parts;
}
