using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

[method: JsonConstructor]
public class CardRowTemplateInfo(
    CardRowOneItem? oneItem,
    CardRowTwoItems? twoItems,
    CardRowThreeItems? threeItems)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CardRowTemplateInfo(CardRowOneItem oneItem) => new(oneItem);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CardRowTemplateInfo(CardRowTwoItems twoItems) => new(twoItems);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator CardRowTemplateInfo(CardRowThreeItems threeItems) => new(threeItems);

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("oneItem")]
    public CardRowOneItem? OneItem { get; } = oneItem;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("twoItems")]
    public CardRowTwoItems? TwoItems { get; } = twoItems;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("threeItems")]
    public CardRowThreeItems? ThreeItems { get; } = threeItems;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CardRowTemplateInfo(CardRowOneItem oneItem) : this(oneItem, default, default) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CardRowTemplateInfo(CardRowTwoItems twoItems) : this(default, twoItems, default) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CardRowTemplateInfo(CardRowThreeItems threeItems) : this(default, default, threeItems) { }
}
