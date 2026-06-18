using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Gmail;

public class LabelColor(string? textColor, string? backgroundColor)
{
    /// <summary>
    /// The text color of the label, represented as hex string. This field is required in order to set the color of a label.
    /// </summary>
    [JsonPropertyName("textColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? TextColor { get; } = textColor;

    /// <summary>
    /// The background color represented as hex string #RRGGBB (ex #000000). This field is required in order to set the color of a label.
    /// </summary>
    [JsonPropertyName("backgroundColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? BackgroundColor { get; } = backgroundColor;
}
