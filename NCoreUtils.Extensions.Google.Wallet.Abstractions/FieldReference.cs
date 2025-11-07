using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class FieldReference(string fieldPath, string? dateFormat = default)
{
    [JsonPropertyName("fieldPath")]
    public string FieldPath { get; } = fieldPath;

    [JsonPropertyName("dateFormat")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? DateFormat { get; } = dateFormat;
}
