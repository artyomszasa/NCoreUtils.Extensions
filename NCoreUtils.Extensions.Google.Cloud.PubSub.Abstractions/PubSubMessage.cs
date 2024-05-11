using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.PubSub;

public class PubSubMessage(string? data, string? messageId)
{
    [JsonPropertyName("data")]
    public string? Data { get; } = data;

    [JsonPropertyName("messageId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? MessageId { get; } = messageId;
}