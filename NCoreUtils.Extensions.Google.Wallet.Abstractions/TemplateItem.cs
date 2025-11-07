using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

[method: JsonConstructor]
public class TemplateItem(
    FieldSelector? firstValue,
    FieldSelector? secondValue = default,
    string? predefinedItem = default)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("firstValue")]
    public FieldSelector? FirstValue { get; } = firstValue;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("secondValue")]
    public FieldSelector? SecondValue { get; } = secondValue;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("predefinedItem")]
    public string? PredefinedItem { get; } = predefinedItem;

    public TemplateItem(string predefinedItem)
        : this(default, default, predefinedItem)
    { }
}
