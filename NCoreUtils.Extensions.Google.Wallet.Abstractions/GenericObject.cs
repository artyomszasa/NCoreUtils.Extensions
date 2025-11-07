using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class GenericObject(
    string id,
    string classId,
    string genericType,
    LocalizedString cardTitle,
    LocalizedString header,
    LocalizedString? subheader = default,
    Image? logo = default,
    string? hexBackgroundColor = default,
    Barcode? barcode = default,
    Image? heroImage = default,
    TimeInterval? validTimeInterval = default,
    IReadOnlyList<ImageModuleData>? imageModulesData = default,
    IReadOnlyList<TextModuleData>? textModulesData = default,
    LinksModuleData? linksModuleData = default,
    GroupingInfo? groupingInfo = default,
    string? smartTapRedemptionValue = default,
    string? state = default,
    bool? hasUsers = default,
    Image? wideLogo = default,
    IReadOnlyList<string>? linkedObjectIds = default)
// TODO: notifications
// TODO: appLinkData
// TODO: rotatingBarcode
// TODO: messages
// TODO: passConstraints
// TODO: saveRestrictions
// TODO: valueAddedModuleData
// TODO: merchantLocations
{
    [JsonPropertyName("id")]
    public string Id { get; } = id;

    [JsonPropertyName("classId")]
    public string ClassId { get; } = classId;

    [JsonPropertyName("genericType")]
    public string GenericType { get; } = genericType;

    [JsonPropertyName("cardTitle")]
    public LocalizedString CardTitle { get; } = cardTitle;

    [JsonPropertyName("header")]
    public LocalizedString Header { get; } = header;

    [JsonPropertyName("subheader")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LocalizedString? Subheader { get; } = subheader;

    [JsonPropertyName("logo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Image? Logo { get; } = logo;

    [JsonPropertyName("hexBackgroundColor")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? HexBackgroundColor { get; } = hexBackgroundColor;

    [JsonPropertyName("barcode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Barcode? Barcode { get; } = barcode;

    [JsonPropertyName("heroImage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Image? HeroImage { get; } = heroImage;

    [JsonPropertyName("validTimeInterval")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public TimeInterval? ValidTimeInterval { get; } = validTimeInterval;

    [JsonPropertyName("imageModulesData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<ImageModuleData>? ImageModulesData { get; } = imageModulesData;

    [JsonPropertyName("textModulesData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<TextModuleData>? TextModulesData { get; } = textModulesData;

    [JsonPropertyName("linksModuleData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LinksModuleData? LinksModuleData { get; } = linksModuleData;

    [JsonPropertyName("groupingInfo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public GroupingInfo? GroupingInfo { get; } = groupingInfo;

    [JsonPropertyName("smartTapRedemptionValue")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? SmartTapRedemptionValue { get; } = smartTapRedemptionValue;

    [JsonPropertyName("state")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? State { get; } = state;

    [JsonPropertyName("hasUsers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? HasUsers { get; } = hasUsers;

    [JsonPropertyName("wideLogo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Image? WideLogo { get; } = wideLogo;

    [JsonPropertyName("linkedObjectIds")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? LinkedObjectIds { get; } = linkedObjectIds;
}