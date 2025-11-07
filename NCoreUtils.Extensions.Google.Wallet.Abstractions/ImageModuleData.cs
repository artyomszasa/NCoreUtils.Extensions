using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class ImageModuleData(Image mainImage, string? id = default)
{
    [JsonPropertyName("mainImage")]
    public Image MainImage { get; } = mainImage;

    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? Id { get; } = id;
}
