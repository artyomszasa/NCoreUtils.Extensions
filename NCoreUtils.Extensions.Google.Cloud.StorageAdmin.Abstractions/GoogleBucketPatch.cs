using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[JsonConverter(typeof(JsonConverter))]
public class GoogleBucketPatch
{
    // NOTE: once older versions (especially .NET 6) have run out of support JsonTypeResolver modifiers should be used
    // instead. For not explicit converter is the only cross-version solution.
    public sealed class JsonConverter : JsonConverter<GoogleBucketPatch>
    {
        public override GoogleBucketPatch? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }
            reader.Expect(JsonTokenType.StartObject);
            reader.ReadOrThrow();
            var res = new GoogleBucketPatch();
            while (reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.ValueTextEquals("storageClass"u8))
                {
                    reader.ReadOrThrow();
                    res.StorageClass = reader.GetString();
                }
                else if (reader.ValueTextEquals("acl"u8))
                {
                    reader.ReadOrThrow();
                    var aclTypeInfo = options.GetTypeInfo<IReadOnlyList<GoogleBucketAccessControl>>();
                    res.Acl = JsonSerializer.Deserialize(ref reader, aclTypeInfo);
                }
                else if (reader.ValueTextEquals("cors"u8))
                {
                    reader.ReadOrThrow();
                    var aclTypeInfo = options.GetTypeInfo<IReadOnlyList<GoogleBucketCors>>();
                    res.Cors = JsonSerializer.Deserialize(ref reader, aclTypeInfo);
                }
                else if (reader.ValueTextEquals("labels"u8))
                {
                    reader.ReadOrThrow();
                    var aclTypeInfo = options.GetTypeInfo<IReadOnlyDictionary<string, string>>();
                    res.Labels = JsonSerializer.Deserialize(ref reader, aclTypeInfo);
                }
                else
                {
                    reader.ReadOrThrow();
                    reader.Skip();
                }
                reader.ReadOrThrow();
            }
            return res;
        }

        public override void Write(Utf8JsonWriter writer, GoogleBucketPatch value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (value.StorageClassValue.TryGetValue(out var storageClass))
            {
                if (storageClass is null)
                {
                    writer.WriteNull("storageClass"u8);
                }
                else
                {
                    writer.WriteString("storageClass"u8, storageClass);
                }
            }
            if (value.AclValue.TryGetValue(out var acl))
            {
                if (acl is null)
                {
                    writer.WriteNull("acl"u8);
                }
                else
                {
                    var aclTypeInfo = options.GetTypeInfo<IReadOnlyList<GoogleBucketAccessControl>>();
                    writer.WritePropertyName("acl"u8);
                    JsonSerializer.Serialize(writer, acl, aclTypeInfo);
                }
            }
            if (value.CorsValue.TryGetValue(out var cors))
            {
                if (cors is null)
                {
                    writer.WriteNull("cors"u8);
                }
                else
                {
                    var corsTypeInfo = options.GetTypeInfo<IReadOnlyList<GoogleBucketCors>>();
                    writer.WritePropertyName("cors"u8);
                    JsonSerializer.Serialize(writer, cors, corsTypeInfo);
                }
            }
            if (value.LabelsValue.TryGetValue(out var labels))
            {
                if (labels is null)
                {
                    writer.WriteNull("labels"u8);
                }
                else
                {
                    var labelsTypeInfo = options.GetTypeInfo<IReadOnlyDictionary<string, string>>();
                    writer.WritePropertyName("labels"u8);
                    JsonSerializer.Serialize(writer, labels, labelsTypeInfo);
                }
            }
            writer.WriteEndObject();
        }
    }

    [JsonPropertyName("storageClass")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Maybe<string?> StorageClassValue { get; set; }

    [JsonIgnore]
    public string? StorageClass
    {
        set => StorageClassValue = value.Just();
    }

    [JsonPropertyName("acl")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Maybe<IReadOnlyList<GoogleBucketAccessControl>?> AclValue { get; set; }

    [JsonIgnore]
    public IReadOnlyList<GoogleBucketAccessControl>? Acl
    {
        set => AclValue = value.Just();
    }

    [JsonPropertyName("cors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Maybe<IReadOnlyList<GoogleBucketCors>?> CorsValue { get; set; }

    [JsonIgnore]
    public IReadOnlyList<GoogleBucketCors>? Cors
    {
        set => CorsValue = value.Just();
    }

    [JsonPropertyName("labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Maybe<IReadOnlyDictionary<string, string>?> LabelsValue { get; set; }

    [JsonIgnore]
    public IReadOnlyDictionary<string, string>? Labels
    {
        set => LabelsValue = value.Just();
    }
}