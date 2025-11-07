using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class Barcode(
    string type,
    string value,
    string? renderEncoding = default,
    string? alternateText = default,
    LocalizedString? showCodeText = default)
{
    [JsonPropertyName("type")]
    public string Type { get; } = type;

    [JsonPropertyName("renderEncoding")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? RenderEncoding { get; } = renderEncoding;

    [JsonPropertyName("value")]
    public string Value { get; } = value;

    [JsonPropertyName("alternateText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AlternateText { get; } = alternateText;

    [JsonPropertyName("showCodeText")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LocalizedString? ShowCodeText { get; } = showCodeText;
}
