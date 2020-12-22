using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
    public class GoogleObjectsData
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "storage#objects";

        [JsonPropertyName("nextPageToken")]
        public string? NextPageToken { get; set; }

        [JsonPropertyName("prefixes")]
        public List<string>? Prefixes { get; set; }

        [JsonPropertyName("items")]
        public List<GoogleObjectData>? Items { get; set; }
    }
}