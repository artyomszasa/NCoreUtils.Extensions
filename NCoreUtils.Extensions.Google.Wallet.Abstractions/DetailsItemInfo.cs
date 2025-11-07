using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class DetailsItemInfo(TemplateItem item)
{
    [JsonPropertyName("item")]
    public TemplateItem Item { get; } = item;
}
