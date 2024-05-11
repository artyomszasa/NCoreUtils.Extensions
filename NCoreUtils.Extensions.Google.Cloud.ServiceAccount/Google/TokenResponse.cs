using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

public record TokenResponse(
    [property: JsonPropertyName("access_token")]
    string? AccessToken,
    [property: JsonPropertyName("scope")]
    string? Scope,
    [property: JsonPropertyName("token_type")]
    string? TokenType,
    [property: JsonPropertyName("expires_in")]
    [property: JsonConverter(typeof(TimeSpanSecondsConverter))]
    TimeSpan ExpiresIn
);