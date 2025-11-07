using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class ListTemplateOverride(FirstRowOption firstRowOption, FieldSelector? secondRowOption)
{
    [JsonPropertyName("firstRowOption")]
    public FirstRowOption FirstRowOption { get; } = firstRowOption;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("secondRowOption")]
    public FieldSelector? SecondRowOption { get; } = secondRowOption;
}
