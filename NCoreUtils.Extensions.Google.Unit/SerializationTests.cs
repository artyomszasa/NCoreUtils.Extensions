using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using NCoreUtils.Google.Cloud.Monitoring;

namespace NCoreUtils.Google;

public class SerializationTests
{
    private sealed class MonitoringTestData : IEnumerable<object[]>
    {
        private static readonly TimeSeries TimeSeries0 = new(
            metric: new Metric("TEST"),
            resource: new MonitoredResource("TEST", new Dictionary<string, string> { { "prop", "VALUE" } }),
            metricKind: MetricKinds.Gauge,
            points: new Point(new DateTimeOffset(2025, 8, 20, 0, 0, 0, TimeSpan.FromHours(1)), 201),
            valueType: TypedValue.ValueType.Integer
        );

        private static readonly string Expected0 = "{\"metric\":{\"type\":\"TEST\"},\"resource\":{\"type\":\"TEST\",\"labels\":{\"prop\":\"VALUE\"}},\"metricKind\":\"GAUGE\",\"valueType\":\"INT64\",\"points\":[{\"interval\":{\"endTime\":\"2025-08-19T23:00:00.000Z\"},\"value\":{\"int64Value\":201}}]}";

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return [TimeSeries0, Expected0, MonitoringV3ApiSerializerContext.Default];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [ClassData(typeof(MonitoringTestData))]
    [Theory]
    public void SerializeData(object input, string expected, JsonSerializerContext serializerContext)
    {
        var actual = JsonSerializer.Serialize(input, serializerContext.GetTypeInfo(input.GetType())!);
        Assert.Equal(expected, actual);
    }

}