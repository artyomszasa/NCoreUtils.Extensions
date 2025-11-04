using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

[method: JsonConstructor]
public class TimeSeries(
    Metric metric,
    MonitoredResource resource,
    // FIXME: metadata
    string metricKind,
    Points points,
    TypedValue.ValueType? valueType = default,
    string? unit = default,
    string? description = default)
{
    [JsonPropertyName("metric")]
    public Metric Metric { get; private set; } = metric;

    [JsonPropertyName("resource")]
    public MonitoredResource Resource { get; private set; } = resource;

    [JsonPropertyName("metricKind")]
    public string MetricKind { get; private set; } = metricKind;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("valueType")]
    public TypedValue.ValueType? ValueType { get; private set; } = valueType;

    [JsonPropertyName("points")]
    public Points Points { get; private set; } = points;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("unit")]
    public string? Unit { get; private set; } = unit;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("description")]
    public string? Description { get; private set; } = description;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeSeries(
        Metric metric,
        MonitoredResource resource,
        // FIXME: metadata
        string metricKind,
        IReadOnlyList<Point> points,
        TypedValue.ValueType? valueType = default,
        string? unit = default,
        string? description = default)
        : this(metric, resource, metricKind, new Points(points), valueType, unit, description)
    { }

    public TimeSeries Update(
        Metric metric,
        MonitoredResource resource,
        // FIXME: metadata
        string metricKind,
        Points points,
        TypedValue.ValueType? valueType = default,
        string? unit = default,
        string? description = default)
    {
        Metric = metric;
        Resource = resource;
        MetricKind = metricKind;
        Points = points;
        ValueType = valueType;
        Unit = unit;
        Description = description;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TimeSeries Update(
        Metric metric,
        MonitoredResource resource,
        // FIXME: metadata
        string metricKind,
        IReadOnlyList<Point> points,
        TypedValue.ValueType? valueType = default,
        string? unit = default,
        string? description = default)
        => Update(
            metric,
            resource,
            metricKind,
            new Points(points),
            valueType,
            unit,
            description
        );
}