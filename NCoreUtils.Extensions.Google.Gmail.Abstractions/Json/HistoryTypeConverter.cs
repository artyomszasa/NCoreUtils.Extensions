using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail.Json;

[JsonEnumConverter(typeof(HistoryType))]
public sealed partial class HistoryTypeConverter : JsonConverter<HistoryType> { }