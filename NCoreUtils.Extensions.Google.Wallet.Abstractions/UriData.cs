using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class UriData(string uri, string? description, LocalizedString? localizedDescription, string? id)
{
    [JsonPropertyName("uri")]
    public string Uri { get; } = uri;

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Description { get; } = description;

    [JsonPropertyName("localizedDescription")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LocalizedString? LocalizedDescription { get; } = localizedDescription;

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; } = id;
}
