using System.Text.Json.Serialization;
using NCoreUtils.Google.Gmail.Json;

namespace NCoreUtils.Google.Gmail;

[JsonConverter(typeof(MessageListVisibilityConverter))]
public enum MessageListVisibility
{
    /// <summary>
    /// Show the label in the message list.
    /// </summary>
    [JsonEnumValue("show")]
    Show,

    /// <summary>
    /// Do not show the label in the message list.
    /// </summary>
    [JsonEnumValue("hide")]
    Hide
}