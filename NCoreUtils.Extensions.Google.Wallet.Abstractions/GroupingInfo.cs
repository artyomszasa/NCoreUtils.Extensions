using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class GroupingInfo(string groupingId, int? sortIndex = default)
{
    public static implicit operator GroupingInfo(string groupingId) => new(groupingId);

    [JsonPropertyName("groupingId")]
    public string GroupingId { get; } = groupingId;

    [JsonPropertyName("sortIndex")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? SortIndex { get; } = sortIndex;
}
