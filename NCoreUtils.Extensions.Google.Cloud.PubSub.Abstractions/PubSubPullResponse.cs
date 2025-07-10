using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.PubSub;

public class PubSubPullResponse(IReadOnlyList<ReceivedMessage>? receivedMessages)
{
    [JsonPropertyName("receivedMessages")]
    public IReadOnlyList<ReceivedMessage>? ReceivedMessages { get; } = receivedMessages;
}