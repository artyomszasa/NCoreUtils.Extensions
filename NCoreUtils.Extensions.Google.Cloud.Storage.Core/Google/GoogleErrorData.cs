using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
    public class GoogleErrorData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("errors")]
        public IReadOnlyList<GoogleErrorDetails>? Errors { get; set; }
    }
}