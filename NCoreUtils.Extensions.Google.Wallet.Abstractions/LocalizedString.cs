using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

[method: JsonConstructor]
public class LocalizedString(
    TranslatedString defaultValue,
    IReadOnlyList<TranslatedString>? translatedValues = default)
{
    [JsonPropertyName("defaultValue")]
    public TranslatedString DefaultValue { get; } = defaultValue;

    [JsonPropertyName("translatedValues")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<TranslatedString>? TranslatedValues { get; } = translatedValues;

    public LocalizedString(string language, string value) : this(new TranslatedString(language, value)) { }
}
