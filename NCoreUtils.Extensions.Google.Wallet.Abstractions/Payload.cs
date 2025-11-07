using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class Payload(
    IReadOnlyList<GenericClass>? genericClasses = default,
    IReadOnlyList<GenericObject>? genericObjects = default)
{
    [JsonPropertyName("genericClasses")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<GenericClass>? GenericClasses { get; } = genericClasses;

    [JsonPropertyName("genericObjects")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<GenericObject>? GenericObjects { get; } = genericObjects;
}