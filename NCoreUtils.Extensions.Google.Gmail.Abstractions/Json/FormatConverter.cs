using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail.Json;

[JsonEnumConverter(typeof(Format))]
public sealed partial class FormatConverter : JsonConverter<Format> { }