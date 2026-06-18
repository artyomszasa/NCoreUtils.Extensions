using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail.Json;

[JsonEnumConverter(typeof(LabelType))]
public sealed partial class LabelTypeConverter : JsonConverter<LabelType> { }