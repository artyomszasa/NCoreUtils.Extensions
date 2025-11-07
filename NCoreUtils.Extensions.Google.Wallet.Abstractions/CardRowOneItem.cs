using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class CardRowOneItem(TemplateItem item)
{
    [JsonPropertyName("item")]
    public TemplateItem Item { get; } = item;
}
