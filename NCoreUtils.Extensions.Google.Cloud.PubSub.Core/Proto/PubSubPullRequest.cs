using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.PubSub.Proto;

public class PubSubPullRequest(int maxMessages)
{
    [JsonPropertyName("maxMessages")]
    public int MaxMessages { get; } = maxMessages;
}