using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
    public class GoogleErrorResponse
    {
        [JsonPropertyName("error")]
        public GoogleErrorData Error { get; set; } = default!;
    }
}