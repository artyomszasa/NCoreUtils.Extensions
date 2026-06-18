using System.Text.Json.Serialization;
using NCoreUtils.Google.Gmail.Json;

namespace NCoreUtils.Google.Gmail;

[JsonConverter(typeof(LabelTypeConverter))]
public enum LabelType
{
    [JsonEnumValue("system")]
    System,

    [JsonEnumValue("user")]
    User
}