using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.PubSub.Proto;

public class PubSubPublishRequest(IReadOnlyList<PubSubMessage> messages)
{
    [JsonPropertyName("messages")]
    public IReadOnlyList<PubSubMessage> Messages { get; } = messages;
}