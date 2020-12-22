using System;
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

        [JsonPropertyName("bucket")]
        public string? BucketName { get; set; }

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

        [JsonPropertyName("etag")]
        public string? ETag { get; set; }

        [JsonPropertyName("md5Hash")]
        public string? Md5Hash { get; set; }

        [JsonPropertyName("size")]
        [JsonConverter(typeof(PermissiveUInt64Converter))]
        public ulong? Size { get; set; }

        [JsonPropertyName("timeCreated")]
        [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
        public DateTimeOffset? TimeCreated { get; set; }

        [JsonPropertyName("updated")]
        [JsonConverter(typeof(Rfc3339DateTimeOffsetConverter))]
        public DateTimeOffset? Updated { get; set; }

        [JsonPropertyName("metadata")]
        public Dictionary<string, string>? Metadata { get; set; }

        [JsonPropertyName("acl")]
        public List<GoogleAccessControlEntry> Acl { get; set; } = new List<GoogleAccessControlEntry>();
    }
}