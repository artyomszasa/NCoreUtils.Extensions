using System.Text.Json.Serialization;
using NCoreUtils.Google.Gmail.Json;

namespace NCoreUtils.Google.Gmail;

[JsonConverter(typeof(FormatConverter))]
public enum Format
{
    /// <summary>
    /// Returns only email message ID and labels; does not return the email headers, body, or payload.
    /// </summary>
    [JsonEnumValue("minimal")]
    Minimal = 0,

    /// <summary>
    /// Returns the full email message data with body content parsed in the <see cref="Message.Payload"/> field; the
    /// <see cref="Message.Raw"/> field is not used. Format cannot be used when accessing the api using the
    /// gmail.metadata scope.
    /// </summary>
    [JsonEnumValue("full")]
    Full = 1,

    /// <summary>
    /// Returns the full email message data with body content in the <see cref="Message.Raw"/> field as a base64url
    /// encoded string; the <see cref="Message.Payload"/> field is not used. Format cannot be used when accessing the
    /// api using the gmail.metadata scope.
    /// </summary>
    [JsonEnumValue("raw")]
    Raw = 2,

    /// <summary>
    /// Returns only email message ID, labels, and email headers.
    /// </summary>
    [JsonEnumValue("metadata")]
    Metadata = 3
}