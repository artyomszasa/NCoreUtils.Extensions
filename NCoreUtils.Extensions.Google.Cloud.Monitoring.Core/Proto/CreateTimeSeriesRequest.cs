using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring.Proto;

public class CreateTimeSeriesRequest(IReadOnlyList<TimeSeries> timeSeries)
{
    [JsonPropertyName("timeSeries")]
    public IReadOnlyList<TimeSeries> TimeSeries { get; } = timeSeries;
}