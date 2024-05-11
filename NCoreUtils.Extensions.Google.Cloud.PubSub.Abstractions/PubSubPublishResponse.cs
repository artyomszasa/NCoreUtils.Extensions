using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.PubSub;

public class PubSubPublishResponse(IReadOnlyList<string> messageIds)
{
    [JsonPropertyName("messageIds")]
    public IReadOnlyList<string> MessageIds { get; } = messageIds;
}