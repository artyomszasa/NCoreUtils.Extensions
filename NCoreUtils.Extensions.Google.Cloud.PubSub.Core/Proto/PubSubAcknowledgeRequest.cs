using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.PubSub.Proto;

public class PubSubAcknowledgeRequest(IReadOnlyList<string> ackIds)
{
    [JsonPropertyName("ackIds")]
    public IReadOnlyList<string> AckIds { get; } = ackIds ?? [];
}