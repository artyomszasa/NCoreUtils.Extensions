using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google
{
    public class GoogleObjectPatchData
    {
        // [JsonPropertyName("name")]
        // public string? Name { get; set; }

        [JsonPropertyName("contentDisposition")]
        public string? ContentDisposition { get; set; }

        [JsonPropertyName("contentEncoding")]
        public string? ContentEncoding { get; set; }

        [JsonPropertyName("contentLanguage")]
        public string? ContentLanguage { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("cacheControl")]
        public string? CacheControl { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        [JsonPropertyName("acl")]
        public List<GoogleAccessControlEntry> Acl { get; set; } = new List<GoogleAccessControlEntry>();
    }
}