using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class ClassTemplateInfo(
    CardBarcodeSectionDetails? cardBarcodeSectionDetails = default,
    CardTemplateOverride? cardTemplateOverride = default,
    DetailsTemplateOverride? detailsTemplateOverride = default,
    ListTemplateOverride? listTemplateOverride = default)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("cardBarcodeSectionDetails")]
    public CardBarcodeSectionDetails? CardBarcodeSectionDetails { get; } = cardBarcodeSectionDetails;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("cardTemplateOverride")]
    public CardTemplateOverride? CardTemplateOverride { get; } = cardTemplateOverride;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("detailsTemplateOverride")]
    public DetailsTemplateOverride? DetailsTemplateOverride { get; } = detailsTemplateOverride;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("listTemplateOverride")]
    public ListTemplateOverride? ListTemplateOverride { get; } = listTemplateOverride;
}
