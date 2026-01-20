using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NCoreUtils.Google;

[method: JsonConstructor]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct TranslateTextGlossaryConfig(string glossary, bool? ignoreCase = default)
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TranslateTextGlossaryConfig(string glossary) => new(glossary);

    [JsonPropertyName("glossary")]
    public string Glossary { get; } = glossary;

    [JsonPropertyName("ignoreCase")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? IgnoreCase { get; } = ignoreCase;
}
