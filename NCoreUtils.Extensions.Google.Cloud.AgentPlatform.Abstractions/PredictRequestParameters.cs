using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class PredictRequestParameters(
    int sampleCount,
    bool? addWatermark = default,
    string? aspectRatio = default,
    bool? enhancePrompt = default,
    string? language = default,
    string? negativePrompt = default,
    PredictRequestOutputOptions? outputOptions = default,
    string? personGeneration = default,
    string? safetySetting = default,
    string? sampleImageSize = default,
    uint? seed = default,
    string? storageUri = default)
{
    [JsonPropertyName("sampleCount")]
    public int SampleCount { get; } = sampleCount;

    [JsonPropertyName("addWatermark")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? AddWatermark { get; } = addWatermark;

    [JsonPropertyName("aspectRatio")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? AspectRatio { get; } = aspectRatio;

    [JsonPropertyName("enhancePrompt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? EnhancePrompt { get; } = enhancePrompt;

    [JsonPropertyName("language")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Language { get; } = language;

    [JsonPropertyName("negativePrompt")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? NegativePrompt { get; } = negativePrompt;

    [JsonPropertyName("outputOptions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PredictRequestOutputOptions? OutputOptions { get; } = outputOptions;

    [JsonPropertyName("personGeneration")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? PersonGeneration { get; } = personGeneration;

    [JsonPropertyName("safetySetting")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SafetySetting { get; } = safetySetting;

    [JsonPropertyName("sampleImageSize")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SampleImageSize { get; } = sampleImageSize;

    [JsonPropertyName("seed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public uint? Seed { get; } = seed;

    [JsonPropertyName("storageUri")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? StorageUri { get; } = storageUri;
}
