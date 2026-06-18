using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

public class ModifyMessageRequest(
    IReadOnlyList<string>? addLabelIds = default,
    IReadOnlyList<string>? removeLabelIds = default,
    IReadOnlyList<ClassificationLabelValue>? addClassificationLabels = default,
    IReadOnlyList<string>? removeClassificationLabelIds = default)
{
    /// <summary>
    /// A list of IDs of labels to add to this message. You can add up to 100 labels with each update.
    /// </summary>
    [JsonPropertyName("addLabelIds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? AddLabelIds { get; } = addLabelIds;

    /// <summary>
    /// A list IDs of labels to remove from this message. You can remove up to 100 labels with each update.
    /// </summary>
    [JsonPropertyName("removeLabelIds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? RemoveLabelIds { get; } = removeLabelIds;

    /// <summary>
    /// A list of classification label values to add. If a Classification Label with the same label ID is already
    /// applied to the message, fields with existing field IDs will be updated and fields with new field IDs will be
    /// added. There's a limit of 20 Classification Label values per request. If the message is already classified and
    /// the final total number of Classification Label values exceeds the maximum allowed number of Classification
    /// Label values per message, the modification fails.
    /// </summary>
    [JsonPropertyName("addClassificationLabels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<ClassificationLabelValue>? AddClassificationLabels { get; } = addClassificationLabels;

    /// <summary>
    /// A list of Classification Label values to remove from this message.
    /// </summary>
    [JsonPropertyName("removeClassificationLabelIds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? RemoveClassificationLabelIds { get; } = removeClassificationLabelIds;
}