using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
    public class GoogleObjectData
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "storage#object";

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("cacheControl")]
        public string? CacheControl { get; set; }

        [JsonPropertyName("md5Hash")]
        public string? Md5Hash { get; set; }

        [JsonPropertyName("size")]
        public ulong? Size { get; set; }

        [JsonPropertyName("acl")]
        public List<GoogleAccessControlEntry> Acl { get; set; } = new List<GoogleAccessControlEntry>();
    }
}