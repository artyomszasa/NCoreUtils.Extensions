using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class CardTemplateOverride(IReadOnlyList<CardRowTemplateInfo> cardRowTemplateInfos)
{
    [JsonPropertyName("cardRowTemplateInfos")]
    public IReadOnlyList<CardRowTemplateInfo> CardRowTemplateInfos { get; } = cardRowTemplateInfos;
}
