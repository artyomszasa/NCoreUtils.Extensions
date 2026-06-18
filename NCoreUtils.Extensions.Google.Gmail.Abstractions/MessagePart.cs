using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

/// <summary>
/// A single part of a multi-part email.
/// </summary>
public class MessagePart(
    string partId,
    string? mimeType = default,
    string? filename = default,
    IReadOnlyList<MessagePartHeader>? headers = default,
    MessagePartBody? body = default,
    IReadOnlyList<MessagePart>? parts = default)
{
    /// <summary>
    /// The immutable ID of the message part.
    /// </summary>
    [JsonPropertyName("partId")]
    public string PartId { get; } = partId;

    /// <summary>
    /// The MIME type of the message part.
    /// </summary>
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; } = mimeType;

    /// <summary>
    /// The filename of the attachment. Only present if this message part represents an attachment.
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; } = filename;

    /// <summary>
    /// List of headers on this message part. For the top-level message part, representing the entire message,
    /// the headers must conform to the <see href="https://www.rfc-editor.org/rfc/rfc2822">RFC 2822</see> standard.
    /// For inner message parts, headers are roughly structured but not guaranteed to be in compliance with RFC 2822.
    /// </summary>
    [JsonPropertyName("headers")]
    public IReadOnlyList<MessagePartHeader>? Headers { get; } = headers;

    /// <summary>
    /// The message part body for this part, which may be empty for container MIME types.
    /// </summary>
    [JsonPropertyName("body")]
    public MessagePartBody? Body { get; } = body;

    /// <summary>
    /// The child message parts of this part. This only applies to container MIME message parts (for example, <c>multipart/*</c>).
    /// For non-container MIME message parts, this field is empty. For more information, see
    /// <see href="https://developers.google.com/gmail/api/guides/messages#body_structure">Message body structure</see>.
    /// </summary>
    [JsonPropertyName("parts")]
    public IReadOnlyList<MessagePart>? Parts { get; } = parts;
}
