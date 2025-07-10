using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.PubSub;

public class ReceivedMessage(string? ackId, PubSubMessage message, int deliveryAttempt)
{
    [JsonPropertyName("ackId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AckId { get; } = ackId;

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PubSubMessage Message { get; } = message;

    [JsonPropertyName("deliveryAttempt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int DeliveryAttempt { get; } = deliveryAttempt;
}
