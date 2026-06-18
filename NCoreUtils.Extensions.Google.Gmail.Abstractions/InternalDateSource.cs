using System.Text.Json.Serialization;
using NCoreUtils.Google.Gmail.Json;

namespace NCoreUtils.Google.Gmail;

[JsonConverter(typeof(InternalDateSourceConverter))]
public enum InternalDateSource
{
    /// <summary>
    /// Internal message date set to current time when received by Gmail.
    /// </summary>
    [JsonEnumValue("receivedTime")]
    ReceivedTime = 0,

    /// <summary>
    /// Internal message time based on 'Date' header in email, when valid.
    /// </summary>
    [JsonEnumValue("dateHeader")]
    DateHeader = 1
}