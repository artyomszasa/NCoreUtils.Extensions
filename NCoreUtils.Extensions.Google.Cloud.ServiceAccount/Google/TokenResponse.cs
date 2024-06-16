using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public class TokenResponse(string? accessToken, string? scope, string? tokenType, TimeSpan expiresIn)
{
    [property: JsonPropertyName("access_token")]
    public string? AccessToken { get; } = accessToken;

    [property: JsonPropertyName("scope")]
    public string? Scope { get; } = scope;

    [property: JsonPropertyName("token_type")]
    public string? TokenType { get; } = tokenType;

    [property: JsonPropertyName("expires_in")]
    [property: JsonConverter(typeof(TimeSpanSecondsConverter))]
    public TimeSpan ExpiresIn { get; } = expiresIn;
}

// public record TokenResponse(
//     [property: JsonPropertyName("access_token")]
//     string? AccessToken,
//     [property: JsonPropertyName("scope")]
//     string? Scope,
//     [property: JsonPropertyName("token_type")]
//     string? TokenType,
//     [property: JsonPropertyName("expires_in")]
//     [property: JsonConverter(typeof(TimeSpanSecondsConverter))]
//     TimeSpan ExpiresIn
// );