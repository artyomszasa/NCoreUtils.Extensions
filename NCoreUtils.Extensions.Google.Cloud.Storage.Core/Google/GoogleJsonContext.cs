using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        GenerationMode = JsonSourceGenerationMode.Default,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonSerializable(typeof(GoogleObjectsData))]
    [JsonSerializable(typeof(GoogleObjectData))]
    [JsonSerializable(typeof(GoogleErrorResponse))]
    [JsonSerializable(typeof(GoogleErrorData))]
    [JsonSerializable(typeof(GoogleObjectPatchData))]
    internal partial class GoogleJsonContext : JsonSerializerContext
    {

    }
}