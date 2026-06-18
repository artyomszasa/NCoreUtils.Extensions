using System.Text.Json.Serialization;
using NCoreUtils.Google.Gmail.Json;

namespace NCoreUtils.Google.Gmail;

[JsonConverter(typeof(LabelListVisibilityConverter))]
public enum LabelListVisibility
{
    /// <summary>
    /// Show the label in the label list.
    /// </summary>
    [JsonEnumValue("labelShow")]
    Show,

    /// <summary>
    /// Show the label if there are any unread messages with that label.
    /// </summary>
    [JsonEnumValue("labelShowIfUnread")]
    ShowIfUnread,

    /// <summary>
    /// Do not show the label in the label list.
    /// </summary>
    [JsonEnumValue("labelHide")]
    Hide
}