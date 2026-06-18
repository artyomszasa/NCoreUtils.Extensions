using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

/// <summary>
/// Download restrictions applied to the file.
/// </summary>
public class DownloadRestrictionsMetadata(
    DownloadRestriction? itemDownloadRestriction = default,
    DownloadRestriction? effectiveDownloadRestrictionWithContext = default)
{
    /// <summary>
    /// The download restriction of the file applied directly by the owner or organizer.
    /// </summary>
    [JsonPropertyName("itemDownloadRestriction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DownloadRestriction? ItemDownloadRestriction { get; } = itemDownloadRestriction;

    /// <summary>
    /// Output only. The effective download restriction applied to this file.
    /// </summary>
    [JsonPropertyName("effectiveDownloadRestrictionWithContext")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DownloadRestriction? EffectiveDownloadRestrictionWithContext { get; } = effectiveDownloadRestrictionWithContext;
}
