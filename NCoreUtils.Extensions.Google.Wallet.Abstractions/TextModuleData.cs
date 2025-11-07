using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class TextModuleData(
    string header,
    string body,
    LocalizedString? localizedHeader,
    LocalizedString? localizedBody,
    string? id)
{
    [JsonPropertyName("header")]
    public string Header { get; } = header;

    [JsonPropertyName("body")]
    public string Body { get; } = body;

    [JsonPropertyName("localizedHeader")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LocalizedString? LocalizedHeader { get; } = localizedHeader;

    [JsonPropertyName("localizedBody")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LocalizedString? LocalizedBody { get; } = localizedBody;

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; } = id;
}
