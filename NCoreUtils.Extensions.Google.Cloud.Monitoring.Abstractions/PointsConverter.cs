using System.Text.Json;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.Monitoring;

public class PointsConverter : JsonConverter<Points>
{
    public override Points Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var pointTypeInfo = options.GetTypeInfo<Point>();
        reader.Expect(JsonTokenType.StartArray);
        reader.ReadOrThrow();
        var point0 = JsonSerializer.Deserialize(ref reader, pointTypeInfo)
            ?? throw new JsonException("Deserializing point produced null.");
        reader.ReadOrThrow();
        if (reader.TokenType == JsonTokenType.EndArray)
        {
            return new Points(point0);
        }
        var points = new List<Point> { point0 };
        do
        {
            var point = JsonSerializer.Deserialize(ref reader, pointTypeInfo)
                ?? throw new JsonException("Deserializing point produced null.");
            points.Add(point);
            reader.ReadOrThrow();
        }
        while (reader.TokenType != JsonTokenType.EndArray);
        return new Points(points);
    }

    public override void Write(Utf8JsonWriter writer, Points value, JsonSerializerOptions options)
    {
        var pointTypeInfo = options.GetTypeInfo<Point>();
        writer.WriteStartArray();
        switch (value._value)
        {
            case null:
                break;
            case Point point:
                JsonSerializer.Serialize(writer, point, pointTypeInfo);
                break;
            case Point[] array:
                foreach (var point in array)
                {
                    JsonSerializer.Serialize(writer, point, pointTypeInfo);
                }
                break;
            case List<Point> list:
                foreach (var point in list)
                {
                    JsonSerializer.Serialize(writer, point, pointTypeInfo);
                }
                break;
            default:
                var roList = (IReadOnlyList<Point>)value._value;
                for (var i = 0; i < roList.Count; ++i)
                {
                    JsonSerializer.Serialize(writer, roList[i], pointTypeInfo);
                }
                break;
        }
        writer.WriteEndArray();
    }
}