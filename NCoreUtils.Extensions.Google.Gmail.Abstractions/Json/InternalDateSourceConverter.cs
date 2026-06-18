using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail.Json;

[JsonEnumConverter(typeof(InternalDateSource))]
public sealed partial class InternalDateSourceConverter : JsonConverter<InternalDateSource> { }