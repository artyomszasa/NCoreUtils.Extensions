using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

[method: JsonConstructor]
public class Point(TimeInterval interval, TypedValue value)
{
    [JsonPropertyName("interval")]
    public TimeInterval Interval { get; private set; } = interval;

    [JsonPropertyName("value")]
    public TypedValue Value { get; private set; } = value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point(DateTimeOffset endTime, long value)
        : this(new(endTime), TypedValue.Integer(value))
    { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point(DateTimeOffset endTime, double value)
        : this(new(endTime), TypedValue.Double(value))
    { }

    public Point Update(TimeInterval interval, TypedValue value)
    {
        Interval = interval;
        Value = value;
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point Update(DateTimeOffset endTime, long value)
        => Update(new(endTime), TypedValue.Integer(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Point Update(DateTimeOffset endTime, double value)
        => Update(new(endTime), TypedValue.Double(value));
}
