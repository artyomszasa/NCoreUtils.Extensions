using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
    public class GoogleAccessControlEntry
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "storage#objectAccessControl";

        [JsonPropertyName("entity")]
        public string Entity { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
    }
}