using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class CardBarcodeSectionDetails(
    BarcodeSectionDetail? firstTopDetail = default,
    BarcodeSectionDetail? firstBottomDetail = default,
    BarcodeSectionDetail? secondTopDetail = default)
{
    [JsonPropertyName("firstTopDetail")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public BarcodeSectionDetail? FirstTopDetail { get; } = firstTopDetail;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("firstBottomDetail")]
    public BarcodeSectionDetail? FirstBottomDetail { get; } = firstBottomDetail;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("secondTopDetail")]
    public BarcodeSectionDetail? SecondTopDetail { get; } = secondTopDetail;
}
