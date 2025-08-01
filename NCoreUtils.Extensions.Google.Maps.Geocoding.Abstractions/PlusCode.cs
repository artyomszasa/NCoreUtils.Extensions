using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Maps.Geocoding;

public class PlusCode(string globalCode, string compoundCode)
{
    [JsonPropertyName("globalCode")]
    public string GlobalCode { get; } = globalCode;

    [JsonPropertyName("compoundCode")]
    public string CompoundCode { get; } = compoundCode;
}

