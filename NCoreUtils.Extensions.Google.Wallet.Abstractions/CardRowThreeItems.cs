using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class CardRowThreeItems(
    TemplateItem startItem,
    TemplateItem middleItem,
    TemplateItem endItem)
{
    [JsonPropertyName("startItem")]
    public TemplateItem StartItem { get; } = startItem;

    [JsonPropertyName("middleItem")]
    public TemplateItem MiddleItem { get; } = middleItem;

    [JsonPropertyName("endItem")]
    public TemplateItem EndItem { get; } = endItem;
}
