using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

public class MessageAdded(Message message)
{
    [JsonPropertyName("message")]
    public Message Message { get; } = message;
}

public class MessageDeleted(Message message)
{
    [JsonPropertyName("message")]
    public Message Message { get; } = message;
}

public class LabelAdded(Message message, IReadOnlyList<string> labelIds)
{
    [JsonPropertyName("message")]
    public Message Message { get; } = message;

    /// <summary>
    /// Label IDs added to the message.
    /// </summary>
    [JsonPropertyName("labelIds")]
    public IReadOnlyList<string> LabelIds { get; } = labelIds;
}

public class LabelRemoved(Message message, IReadOnlyList<string> labelIds)
{
    [JsonPropertyName("message")]
    public Message Message { get; } = message;

    /// <summary>
    /// Label IDs removed from the message.
    /// </summary>
    [JsonPropertyName("labelIds")]
    public IReadOnlyList<string> LabelIds { get; } = labelIds;
}

public class History(
    string id,
    IReadOnlyList<Message>? messages,
    IReadOnlyList<MessageAdded>? messagesAdded,
    IReadOnlyList<MessageDeleted>? messagesDeleted,
    IReadOnlyList<LabelAdded>? labelsAdded,
    IReadOnlyList<LabelRemoved>? labelsRemoved)
{
    [JsonPropertyName("id")]
    public string Id { get; } = id;

    [JsonPropertyName("messages")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<Message>? Messages { get; } = messages;

    [JsonPropertyName("messagesAdded")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<MessageAdded>? MessagesAdded { get; } = messagesAdded;

    [JsonPropertyName("messagesDeleted")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<MessageDeleted>? MessagesDeleted { get; } = messagesDeleted;

    [JsonPropertyName("labelsAdded")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<LabelAdded>? LabelsAdded { get; } = labelsAdded;

    [JsonPropertyName("labelsRemoved")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<LabelRemoved>? LabelsRemoved { get; } = labelsRemoved;
}

public class ListHistoryResponse(
    IReadOnlyList<History>? history,
    string? nextPageToken,
    string? historyId)
{
    [JsonPropertyName("history")]
    public IReadOnlyList<History>? History { get; } = history;

    [JsonPropertyName("nextPageToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? NextPageToken { get; } = nextPageToken;

    [JsonPropertyName("historyId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? HistoryId { get; } = historyId;
}