using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail.Json;

[JsonEnumConverter(typeof(LabelListVisibility))]
public sealed partial class LabelListVisibilityConverter : JsonConverter<LabelListVisibility> { }