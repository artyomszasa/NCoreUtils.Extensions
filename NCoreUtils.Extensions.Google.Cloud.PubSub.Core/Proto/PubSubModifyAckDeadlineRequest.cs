using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.PubSub.Proto;

public class PubSubModifyAckDeadlineRequest(IReadOnlyList<string> ackIds, int ackDeadlineSeconds)
{
    [JsonPropertyName("ackIds")]
    public IReadOnlyList<string> AckIds { get; } = ackIds ?? [];

    [JsonPropertyName("ackDeadlineSeconds")]
    public int AckDeadlineSeconds { get; } = ackDeadlineSeconds;
}