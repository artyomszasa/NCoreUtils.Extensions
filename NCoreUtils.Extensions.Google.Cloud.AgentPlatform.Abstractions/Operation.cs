using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class Error(int code, string message, IReadOnlyList<JsonElement> details)
{
    [JsonPropertyName("code")]
    public int Code { get; } = code;

    [JsonPropertyName("message")]
    public string Message { get; } = message;

    [JsonPropertyName("details")]
    public IReadOnlyList<JsonElement> Details { get; } = details;
}

public class Operation(string? name, JsonElement? metadata, bool? done, Error? error, JsonElement? response)
{
    [JsonPropertyName("name")]
    public string? Name { get; } = name;

    [JsonPropertyName("metadata")]
    public JsonElement? Metadata { get; } = metadata;

    [JsonPropertyName("done")]
    public bool? Done { get; } = done;

    [JsonPropertyName("error")]
    public Error? Error { get; } = error;

    [JsonPropertyName("response")]
    public JsonElement? Response { get; } = response;
}