using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class DetailsTemplateOverride(IReadOnlyList<DetailsItemInfo> detailsItemInfos)
{
    [JsonPropertyName("detailsItemInfos")]
    public IReadOnlyList<DetailsItemInfo> DetailsItemInfos { get; } = detailsItemInfos;
}
