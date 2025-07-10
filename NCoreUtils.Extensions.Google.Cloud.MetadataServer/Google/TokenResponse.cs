using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class TokenResponse(string? accessToken, string? scope, string? tokenType, TimeSpan? expiresIn)
{
    [property: JsonPropertyName("access_token")]
    public string? AccessToken { get; } = accessToken;

    [property: JsonPropertyName("scope")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Scope { get; } = scope;

    [property: JsonPropertyName("token_type")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TokenType { get; } = tokenType;

    [property: JsonPropertyName("expires_in")]
    [property: JsonConverter(typeof(NullableTimeSpanSecondsConverter))]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TimeSpan? ExpiresIn { get; } = expiresIn;
}