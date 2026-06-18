using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

public class GenerationConfig(
    IReadOnlyList<string>? stopSequences = default,
    string? responseMimeType = default,
    IReadOnlyList<string>? responseModalities = default,
    ThinkingConfig? thinkingConfig = default,
    double? temperature = default,
    double? topP = default,
    double? topK = default,
    int? candidateCount = default,
    int? maxOutputTokens = default,
    bool? responseLogprobs = default,
    int? logprobs = default,
    double? presencePenalty = default,
    double? frequencyPenalty = default,
    int? seed = default,
    Schema? responseSchema = default,
    // TODO: responseJsonSchema
    // TODO: routingConfig
    bool? audioTimestamp = default,
    string? mediaResolution = default,
    // TODO: speechConfig
    // TODO: imageConfig
    bool? enableAffectiveDialog = default)
{
    [JsonPropertyName("stopSequences")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? StopSequences { get; } = stopSequences;

    [JsonPropertyName("responseMimeType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ResponseMimeType { get; } = responseMimeType;

    [JsonPropertyName("responseModalities")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? ResponseModalities { get; } = responseModalities;

    [JsonPropertyName("thinkingConfig")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ThinkingConfig? ThinkingConfig { get; } = thinkingConfig;

    [JsonPropertyName("temperature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? Temperature { get; } = temperature;

    [JsonPropertyName("topP")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? TopP { get; } = topP;

    [JsonPropertyName("topK")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? TopK { get; } = topK;

    [JsonPropertyName("candidateCount")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? CandidateCount { get; } = candidateCount;

    [JsonPropertyName("maxOutputTokens")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? MaxOutputTokens { get; } = maxOutputTokens;

    [JsonPropertyName("responseLogprobs")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? ResponseLogprobs { get; } = responseLogprobs;

    [JsonPropertyName("logprobs")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Logprobs { get; } = logprobs;

    [JsonPropertyName("presencePenalty")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? PresencePenalty { get; } = presencePenalty;

    [JsonPropertyName("frequencyPenalty")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public double? FrequencyPenalty { get; } = frequencyPenalty;

    [JsonPropertyName("seed")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int? Seed { get; } = seed;

    [JsonPropertyName("responseSchema")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Schema? ResponseSchema { get; } = responseSchema;

    [JsonPropertyName("audioTimestamp")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? AudioTimestamp { get; } = audioTimestamp;

    [JsonPropertyName("mediaResolution")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MediaResolution { get; } = mediaResolution;

    [JsonPropertyName("enableAffectiveDialog")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? EnableAffectiveDialog { get; } = enableAffectiveDialog;
}
