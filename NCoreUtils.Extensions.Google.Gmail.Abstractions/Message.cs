using System.Text.Json.Serialization;
using NCoreUtils.Google.Gmail.Json;

namespace NCoreUtils.Google.Gmail;

/// <summary>
/// An email message.
/// </summary>
public class Message(
    string id,
    string? threadId,
    IReadOnlyList<string> labelIds,
    string snippet,
    string historyId,
    DateTimeOffset? internalDate,
    MessagePart? payload,
    int? sizeEstimate,
    string? raw,
    IReadOnlyList<ClassificationLabelValue>? classificationLabelValues = default)
{
    /// <summary>
    /// The immutable ID of the message.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; } = id;

    /// <summary>
    /// The ID of the thread the message belongs to. To add a message or draft to a thread, the following criteria must be met:
    /// <list type="number">
    /// <item>
    /// <description>The requested <c>threadId</c> must be specified on the <c>Message</c> or <c>Draft.Message</c> you supply with your request.</description>
    /// </item>
    /// <item>
    /// <description>The <c>References</c> and <c>In-Reply-To</c> headers must be set in compliance with the <see href="https://www.rfc-editor.org/rfc/rfc2822">RFC 2822</see> standard.</description>
    /// </item>
    /// <item>
    /// <description>The <c>Subject</c> headers must match.</description>
    /// </item>
    /// </list>
    /// </summary>
    [JsonPropertyName("threadId")]
    public string? ThreadId { get; } = threadId;

    /// <summary>
    /// List of IDs of labels applied to this message.
    /// </summary>
    [JsonPropertyName("labelIds")]
    public IReadOnlyList<string> LabelIds { get; } = labelIds;

    /// <summary>
    /// A short part of the message text.
    /// </summary>
    [JsonPropertyName("snippet")]
    public string Snippet { get; } = snippet;

    /// <summary>
    /// The ID of the last history record that modified this message.
    /// </summary>
    [JsonPropertyName("historyId")]
    public string HistoryId { get; } = historyId;

    /// <summary>
    /// The internal message creation timestamp (epoch ms), which determines ordering in the inbox.
    /// For normal SMTP-received email, this represents the time the message was originally accepted by Google,
    /// which is more reliable than the <c>Date</c> header. However, for API-migrated mail, it can be configured
    /// by client to be based on the <c>Date</c> header.
    /// </summary>
    [JsonPropertyName("internalDate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonConverter(typeof(NullableEpochMsConverter))]
    public DateTimeOffset? InternalDate { get; } = internalDate;

    /// <summary>
    /// The parsed email structure in the message parts.
    /// </summary>
    [JsonPropertyName("payload")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public MessagePart? Payload { get; } = payload;

    /// <summary>
    /// Estimated size in bytes of the message.
    /// </summary>
    [JsonPropertyName("sizeEstimate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? SizeEstimate { get; } = sizeEstimate;

    /// <summary>
    /// The entire email message in an <see href="https://www.rfc-editor.org/rfc/rfc2822">RFC 2822</see> formatted and
    /// base64url encoded string. Returned in <c>messages.get</c> and <c>drafts.get</c> responses when the <c>format=RAW</c>
    /// parameter is supplied.
    /// </summary>
    [JsonPropertyName("raw")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Raw { get; } = raw;

    /// <summary>
    /// Classification Label values on the message. Available Classification Label schemas can be queried using the
    /// Google Drive Labels API. Each classification label ID must be unique. If duplicate IDs are provided, only
    /// one will be retained, and the selection is arbitrary. Only used for Google Workspace accounts.
    /// </summary>
    [JsonPropertyName("classificationLabelValues")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<ClassificationLabelValue>? ClassificationLabelValues { get; } = classificationLabelValues;
}
