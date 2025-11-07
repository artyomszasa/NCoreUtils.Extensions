using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

[method: JsonConstructor]
public readonly struct DateTime(DateTimeOffset date)
{
    public static implicit operator DateTime(DateTimeOffset date) => new(date);

    [JsonConverter(typeof(NonNullableRfc3339DateTimeOffsetConverter))]
    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; } = date;
}
