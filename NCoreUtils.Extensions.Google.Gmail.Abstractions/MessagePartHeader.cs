using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

/// <summary>
/// A single header from a message part.
/// </summary>
public class MessagePartHeader(string name, string value)
{
    /// <summary>
    /// The name of the header.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; } = name;

    /// <summary>
    /// The value of the header.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; } = value;
}
