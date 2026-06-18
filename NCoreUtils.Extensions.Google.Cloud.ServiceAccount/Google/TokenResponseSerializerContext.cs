using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[JsonSerializable(typeof(TokenResponse))]
internal partial class TokenResponseSerializerContext : JsonSerializerContext { }
