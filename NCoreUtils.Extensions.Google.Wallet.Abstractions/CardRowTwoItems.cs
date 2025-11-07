using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class CardRowTwoItems(
    TemplateItem startItem,
    TemplateItem endItem)
{
    [JsonPropertyName("startItem")]
    public TemplateItem StartItem { get; } = startItem;

    [JsonPropertyName("endItem")]
    public TemplateItem EndItem { get; } = endItem;
}
