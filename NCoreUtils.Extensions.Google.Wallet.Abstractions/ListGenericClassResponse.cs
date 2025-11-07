using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class ListGenericClassResponse(
    IReadOnlyList<GenericClass> resources,
    Pagination pagination)
{
    [JsonPropertyName("resources")]
    public IReadOnlyList<GenericClass> Resources { get; } = resources;

    [JsonPropertyName("pagination")]
    public Pagination Pagination { get; } = pagination;
}