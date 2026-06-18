using System.Text.Json.Serialization;
using NCoreUtils.Google.Gmail.Json;

namespace NCoreUtils.Google.Gmail;

[JsonConverter(typeof(HistoryTypeConverter))]
public enum HistoryType
{
    [JsonEnumValue("messageAdded")]
    MessageAdded,

    [JsonEnumValue("messageDeleted")]
    MessageDeleted,

    [JsonEnumValue("labelAdded")]
    LabelAdded,

    [JsonEnumValue("labelRemoved")]
    LabelRemoved
}