using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

[method: JsonConstructor]
public class FirstRowOption(string? transitOption, FieldSelector? fieldOption = default)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("transitOption")]
    public string? TransitOption { get; } = transitOption;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("fieldOption")]
    public FieldSelector? FieldOption { get; } = fieldOption;

    public FirstRowOption(FieldSelector fieldOption) : this(default, fieldOption) { }
}
