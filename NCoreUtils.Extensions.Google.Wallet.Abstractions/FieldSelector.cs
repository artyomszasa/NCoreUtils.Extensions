using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class FieldSelector(IReadOnlyList<FieldReference> fields)
{
    [JsonPropertyName("fields")]
    public IReadOnlyList<FieldReference> Fields { get; } = fields;
}
