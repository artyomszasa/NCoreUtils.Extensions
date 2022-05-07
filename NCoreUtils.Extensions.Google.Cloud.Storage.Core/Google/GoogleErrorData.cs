using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
    public class GoogleErrorData
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}