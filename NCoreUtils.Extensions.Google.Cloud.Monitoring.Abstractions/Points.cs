using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

[JsonConverter(typeof(PointsConverter))]
public readonly struct Points
{
    public static implicit operator Points(Point point)
        => new(point);

    public static implicit operator Points(Point[] points)
        => new(points);

    internal readonly object? _value;

    public Points(Point value)
        => _value = value;

    public Points(IReadOnlyList<Point> value)
        => _value = value;
}