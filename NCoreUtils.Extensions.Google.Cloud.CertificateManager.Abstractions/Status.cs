using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class Status(int code, string? message = default)
{
    [JsonPropertyName("code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Code { get; } = code;

    [JsonPropertyName("message")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Message { get; } = message;

    // FIXME: details
}