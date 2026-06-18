using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

public class Label(
    string id,
    string name,
    MessageListVisibility messageListVisibility,
    LabelListVisibility labelListVisibility,
    LabelType type = LabelType.User,
    int? messagesTotal = default,
    int? messagesUnread = default,
    int? threadsTotal = default,
    int? threadsUnread = default,
    LabelColor? color = default)
{
    [JsonPropertyName("id")]
    public string Id { get; } = id;

    [JsonPropertyName("name")]
    public string Name { get; } = name;

    [JsonPropertyName("messageListVisibility")]
    public MessageListVisibility MessageListVisibility { get; } = messageListVisibility;

    [JsonPropertyName("labelListVisibility")]
    public LabelListVisibility LabelListVisibility { get; } = labelListVisibility;

    [JsonPropertyName("type")]
    public LabelType Type { get; } = type;

    [JsonPropertyName("messagesTotal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MessagesTotal { get; } = messagesTotal;

    [JsonPropertyName("messagesUnread")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MessagesUnread { get; } = messagesUnread;

    [JsonPropertyName("threadsTotal")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ThreadsTotal { get; } = threadsTotal;

    [JsonPropertyName("threadsUnread")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? ThreadsUnread { get; } = threadsUnread;

    [JsonPropertyName("color")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LabelColor? Color { get; } = color;
}