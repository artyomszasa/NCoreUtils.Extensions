using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

/// <summary>
/// Field values for a classification label.
/// </summary>
public class ClassificationLabelFieldValue(string fieldId, string selection)
{
    /// <summary>
    /// The field ID for the Classification Label Value.
    /// </summary>
    [JsonPropertyName("fieldId")]
    public string FieldId { get; } = fieldId;

    /// <summary>
    /// Selection choice ID for the selection option. Should only be set if the field type is SELECTION in the Google Drive Label.Field object.
    /// </summary>
    [JsonPropertyName("selection")]
    public string Selection { get; } = selection;
}
