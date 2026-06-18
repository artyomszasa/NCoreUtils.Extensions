using System.Text.Json.Serialization;
using NCoreUtils.Google;

namespace NCoreUtils.Internal;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Default,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
[JsonSerializable(typeof(GoogleErrorResponse))]
[JsonSerializable(typeof(GoogleErrorData))]
internal partial class GoogleJsonContext : JsonSerializerContext { }