using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

/// <summary>
/// A restriction for copy and download of the file.
/// </summary>
public class DownloadRestriction(
    bool? restrictedForReaders = default,
    bool? restrictedForWriters = default)
{
    /// <summary>
    /// Whether download and copy is restricted for readers.
    /// </summary>
    [JsonPropertyName("restrictedForReaders")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? RestrictedForReaders { get; } = restrictedForReaders;

    /// <summary>
    /// Whether download and copy is restricted for writers. If true, download is also restricted for readers.
    /// </summary>
    [JsonPropertyName("restrictedForWriters")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? RestrictedForWriters { get; } = restrictedForWriters;
}
