using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

/// <summary>
/// Classification Labels applied to the email message. [Learn more about classification labels](https://support.google.com/a/answer/9292382).
/// </summary>
public class ClassificationLabelValue(string labelId, IReadOnlyList<ClassificationLabelFieldValue> fields)
{
    /// <summary>
    /// The canonical or raw alphanumeric classification label ID.
    /// </summary>
    [JsonPropertyName("labelId")]
    public string LabelId { get; } = labelId;

    /// <summary>
    /// Field values for the given classification label ID.
    /// </summary>
    [JsonPropertyName("fields")]
    public IReadOnlyList<ClassificationLabelFieldValue> Fields { get; } = fields;
}
