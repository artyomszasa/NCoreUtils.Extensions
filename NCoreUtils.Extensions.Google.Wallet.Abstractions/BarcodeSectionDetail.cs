using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class BarcodeSectionDetail(FieldSelector fieldSelector)
{
    [JsonPropertyName("fieldSelector")]
    public FieldSelector FieldSelector { get; } = fieldSelector;
}
