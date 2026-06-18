using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Cloud.AgentPlatform;

// TODO: metadata
[method: JsonConstructor]
public class Part(
    // START UNION
    string? text,
    Blob? inlineData,
    FileData? fileData,
    // TODO: functionCall
    // TODO: functionResponse
    // TODO: executableCode
    // TODO: codeExecutionResult

    // END UNION
    bool? thought = default,
    string? thoughtSignature = default,
    string? mediaResolution = default)
{
    public static implicit operator Part(string text) => new(text);

    public static implicit operator Part(Blob inlineData) => new(inlineData);

    public static implicit operator Part(FileData fileData) => new(fileData);

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Text { get; } = text;

    [JsonPropertyName("inlineData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Blob? InlineData { get; } = inlineData;

    [JsonPropertyName("fileData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public FileData? FileData { get; } = fileData;

    [JsonPropertyName("thought")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? Thought { get; } = thought;

    [JsonPropertyName("thoughtSignature")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ThoughtSignature { get; } = thoughtSignature;

    [JsonPropertyName("mediaResolution")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MediaResolution { get; } = mediaResolution;

    public Part(string text, bool? thought = default, string? thoughtSignature = default, string? mediaResolution = default)
        : this(text, default, default, thought, thoughtSignature, mediaResolution)
    { }

    public Part(Blob inlineData, bool? thought = default, string? thoughtSignature = default, string? mediaResolution = default)
        : this(default, inlineData, default, thought, thoughtSignature, mediaResolution)
    { }

    public Part(FileData fileData, bool? thought = default, string? thoughtSignature = default, string? mediaResolution = default)
        : this(default, default, fileData, thought, thoughtSignature, mediaResolution)
    { }
}
