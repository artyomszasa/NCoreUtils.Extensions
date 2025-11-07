using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Wallet;

public class GenericClass(
    string id,
    ClassTemplateInfo? classTemplateInfo = default,
    IReadOnlyList<ImageModuleData>? imageModulesData = default,
    IReadOnlyList<TextModuleData>? textModulesData = default,
    LinksModuleData? linksModuleData = default,
    bool? enableSmartTap = default,
    IReadOnlyList<string>? redemptionIssuers = default,
    string? multipleDevicesAndHoldersAllowedStatus = default,
    string? viewUnlockRequirement = default)
// TODO: callbackOptions
// TODO: securityAnimation
// TODO: messages
// TODO: appLinkData
// TODO: valueAddedModuleData
// TODO: merchantLocations
{
    [JsonPropertyName("id")]
    public string Id { get; } = id;

    [JsonPropertyName("classTemplateInfo")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public ClassTemplateInfo? ClassTemplateInfo { get; } = classTemplateInfo;

    [JsonPropertyName("imageModulesData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<ImageModuleData>? ImageModulesData { get; } = imageModulesData;

    [JsonPropertyName("textModulesData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<TextModuleData>? TextModulesData { get; } = textModulesData;

    [JsonPropertyName("linksModuleData")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public LinksModuleData? LinksModuleData { get; } = linksModuleData;

    [JsonPropertyName("enableSmartTap")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? EnableSmartTap { get; } = enableSmartTap;

    [JsonPropertyName("redemptionIssuers")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IReadOnlyList<string>? RedemptionIssuers { get; } = redemptionIssuers;

    [JsonPropertyName("multipleDevicesAndHoldersAllowedStatus")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MultipleDevicesAndHoldersAllowedStatus { get; } = multipleDevicesAndHoldersAllowedStatus;

    [JsonPropertyName("viewUnlockRequirement")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? ViewUnlockRequirement { get; } = viewUnlockRequirement;
}
