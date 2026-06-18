using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

/// <summary>
/// The body of a message part.
/// </summary>
public class MessagePartBody(
    string attachmentId,
    int? size,
    string? data)
{
    /// <summary>
    /// When present, contains the ID of an external attachment that can be retrieved in a separate <c>messages.attachments.get</c> request.
    /// When not present, the entire message part body is contained in the data field.
    /// </summary>
    [JsonPropertyName("attachmentId")]
    public string AttachmentId { get; } = attachmentId;

    /// <summary>
    /// The size of the message part in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Size { get; } = size;

    /// <summary>
    /// The body data of a MIME message part as a base64url encoded string.
    /// Might be empty for MIME container types that have no message body or when the body data is sent as a separate attachment.
    /// An attachment ID is present if the body data is contained in a separate attachment.
    /// </summary>
    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Data { get; } = data;
}
