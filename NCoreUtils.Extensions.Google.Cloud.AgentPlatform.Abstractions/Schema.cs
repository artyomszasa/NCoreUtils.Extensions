using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class Schema(
    string? type = default,
    string? format = default,
    string? title = default,
    string? description = default,
    bool? nullable = default,
    // TODO: default,
    Schema? items = default,
    long? minItems = default,
    long? maxItems = default,
    IReadOnlyList<string>? @enum = default,
    IReadOnlyDictionary<string, Schema>? properties = default,
    IReadOnlyList<string>? propertyOrdering = default,
    IReadOnlyList<string>? required = default,
    long? minProperties = default,
    long? maxProperties = default,
    decimal? minimum = default,
    decimal? maximum = default,
    string? minLength = default,
    string? maxLength = default,
    string? pattern = default,
    // TODO:example
    IReadOnlyList<Schema>? anyOf = default,
    // TODO: additionalProperties
    string? @ref = default,
    IReadOnlyDictionary<string, Schema>? defs = default)
{
    [JsonPropertyName("type")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Type { get; } = type;

    [JsonPropertyName("format")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Format { get; } = format;

    [JsonPropertyName("title")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Title { get; } = title;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; } = description;

    [JsonPropertyName("nullable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Nullable { get; } = nullable;

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Schema? Items { get; } = items;

    [JsonPropertyName("minItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long? MinItems { get; } = minItems;

    [JsonPropertyName("maxItems")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long? MaxItems { get; } = maxItems;

    [JsonPropertyName("properties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, Schema>? Properties { get; }= properties;

    [JsonPropertyName("propertyOrdering")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? PropertyOrdering { get; } = propertyOrdering;

    [JsonPropertyName("required")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Required { get; } = required;

    [JsonPropertyName("minProperties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long? MinProperties { get; } = minProperties;

    [JsonPropertyName("maxProperties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public long? MaxProperties { get; } = maxProperties;

    [JsonPropertyName("minimum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public decimal? Minimum { get; } = minimum;

    [JsonPropertyName("maximum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public decimal? Maximum { get; } = maximum;

    [JsonPropertyName("minLength")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MinLength { get; } = minLength;

    [JsonPropertyName("maxLength")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MaxLength { get; } = maxLength;

    [JsonPropertyName("pattern")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Pattern { get; } = pattern;

    [JsonPropertyName("anyOf")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<Schema>? AnyOf { get; } = anyOf;

    [JsonPropertyName("defs")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyDictionary<string, Schema>? Defs { get; } = defs;

    [JsonPropertyName("ref")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Ref { get; } = @ref;

    [JsonPropertyName("enum")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? Enum { get; } = @enum;

}
