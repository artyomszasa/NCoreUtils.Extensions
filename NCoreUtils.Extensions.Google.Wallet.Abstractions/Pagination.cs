using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class Pagination(int resultsPerPage, string? nextPageToken)
{
    [JsonPropertyName("resultsPerPage")]
    public int ResultsPerPage { get; } = resultsPerPage;

    [JsonPropertyName("nextPageToken")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? NextPageToken { get; } = nextPageToken;
}
