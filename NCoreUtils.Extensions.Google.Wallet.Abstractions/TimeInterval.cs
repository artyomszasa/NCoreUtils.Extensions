using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class TimeInterval(DateTime start, DateTime end)
{
    [JsonPropertyName("start")]
    public DateTime Start { get; } = start;

    [JsonPropertyName("end")]
    public DateTime End { get; } = end;
}
