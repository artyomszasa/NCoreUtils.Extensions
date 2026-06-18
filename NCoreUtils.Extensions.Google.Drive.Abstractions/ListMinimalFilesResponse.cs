using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class ListMinimalFilesResponse(
    string? kind = "drive#fileList",
    IReadOnlyList<MinimalFileInfo>? files = default,
    string? nextPageToken = default,
    bool? incompleteSearch = default)
{
    [JsonPropertyName("kind")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Kind { get; } = kind;

    [JsonPropertyName("files")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<MinimalFileInfo>? Files { get; } = files;

    [JsonPropertyName("nextPageToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? NextPageToken { get; } = nextPageToken;

    [JsonPropertyName("incompleteSearch")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IncompleteSearch { get; } = incompleteSearch;
}
