using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public class FileImageMediaMetadataLocation(
    double latitude,
    double longitude,
    double altitude)
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; } = latitude;

    [JsonPropertyName("longitude")]
    public double Longitude { get; } = longitude;

    [JsonPropertyName("altitude")]
    public double Altitude { get; } = altitude;
}

public class FileImageMediaMetadata(
    int? width = default,
    int? height = default,
    FileImageMediaMetadataLocation? location = default,
    int? rotation = default,
    string? time = default,
    string? cameraMake = default,
    string? cameraModel = default,
    float? exposureTime = default,
    float? aperture = default,
    bool? flashUsed = default,
    float? focalLength = default,
    int? isoSpeed = default,
    string? meteringMode = default,
    string? sensor = default,
    string? exposureMode = default,
    string? colorSpace = default,
    string? whiteBalance = default,
    float? exposureBias = default,
    float? maxApertureValue = default,
    int? subjectDistance = default,
    string? lens = default)
{
    [JsonPropertyName("width")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Width { get; } = width;

    [JsonPropertyName("height")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Height { get; } = height;

    [JsonPropertyName("location")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileImageMediaMetadataLocation? Location { get; } = location;

    [JsonPropertyName("rotation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Rotation { get; } = rotation;

    [JsonPropertyName("time")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Time { get; } = time;

    [JsonPropertyName("cameraMake")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CameraMake { get; } = cameraMake;

    [JsonPropertyName("cameraModel")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? CameraModel { get; } = cameraModel;

    [JsonPropertyName("exposureTime")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public float? ExposureTime { get; } = exposureTime;

    [JsonPropertyName("aperture")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public float? Aperture { get; } = aperture;

    [JsonPropertyName("flashUsed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? FlashUsed { get; } = flashUsed;

    [JsonPropertyName("focalLength")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public float? FocalLength { get; } = focalLength;

    [JsonPropertyName("isoSpeed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? IsoSpeed { get; } = isoSpeed;

    [JsonPropertyName("meteringMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MeteringMode { get; } = meteringMode;

    [JsonPropertyName("sensor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Sensor { get; } = sensor;

    [JsonPropertyName("exposureMode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ExposureMode { get; } = exposureMode;

    [JsonPropertyName("colorSpace")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ColorSpace { get; } = colorSpace;

    [JsonPropertyName("whiteBalance")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? WhiteBalance { get; } = whiteBalance;

    [JsonPropertyName("exposureBias")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public float? ExposureBias { get; } = exposureBias;

    [JsonPropertyName("maxApertureValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public float? MaxApertureValue { get; } = maxApertureValue;

    [JsonPropertyName("subjectDistance")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? SubjectDistance { get; } = subjectDistance;

    [JsonPropertyName("lens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Lens { get; } = lens;
}