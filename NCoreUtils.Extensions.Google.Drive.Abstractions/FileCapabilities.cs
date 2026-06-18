using System.Text.Json.Serialization;

namespace NCoreUtils.Google.Drive;

public sealed class FileCapabilities(
    bool? canChangeViewersCanCopyContent = default,
    bool? canMoveChildrenOutOfDrive = default,
    bool? canReadDrive = default,
    bool? canEdit = default,
    bool? canCopy = default,
    bool? canComment = default,
    bool? canAddChildren = default,
    bool? canDelete = default,
    bool? canDownload = default,
    bool? canListChildren = default,
    bool? canRemoveChildren = default,
    bool? canRename = default,
    bool? canTrash = default,
    bool? canReadRevisions = default,
    bool? canReadTeamDrive = default,
    bool? canMoveTeamDriveItem = default,
    bool? canChangeCopyRequiresWriterPermission = default,
    bool? canMoveItemIntoTeamDrive = default,
    bool? canUntrash = default,
    bool? canModifyContent = default,
    bool? canMoveItemWithinTeamDrive = default,
    bool? canMoveItemOutOfTeamDrive = default,
    bool? canDeleteChildren = default,
    bool? canMoveChildrenOutOfTeamDrive = default,
    bool? canMoveChildrenWithinTeamDrive = default,
    bool? canTrashChildren = default,
    bool? canMoveItemOutOfDrive = default,
    bool? canAddMyDriveParent = default,
    bool? canRemoveMyDriveParent = default,
    bool? canMoveItemWithinDrive = default,
    bool? canShare = default,
    bool? canMoveChildrenWithinDrive = default,
    bool? canModifyContentRestriction = default,
    bool? canAddFolderFromAnotherDrive = default,
    bool? canChangeSecurityUpdateEnabled = default,
    bool? canAcceptOwnership = default,
    bool? canReadLabels = default,
    bool? canModifyLabels = default,
    bool? canModifyEditorContentRestriction = default,
    bool? canModifyOwnerContentRestriction = default,
    bool? canRemoveContentRestriction = default,
    bool? canDisableInheritedPermissions = default,
    bool? canEnableInheritedPermissions = default,
    bool? canChangeItemDownloadRestriction = default,
    bool? canStartApproval = default)
{
    [JsonPropertyName("canChangeViewersCanCopyContent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanChangeViewersCanCopyContent { get; } = canChangeViewersCanCopyContent;

    [JsonPropertyName("canMoveChildrenOutOfDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveChildrenOutOfDrive { get; } = canMoveChildrenOutOfDrive;

    [JsonPropertyName("canReadDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanReadDrive { get; } = canReadDrive;

    [JsonPropertyName("canEdit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanEdit { get; } = canEdit;

    [JsonPropertyName("canCopy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanCopy { get; } = canCopy;

    [JsonPropertyName("canComment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanComment { get; } = canComment;

    [JsonPropertyName("canAddChildren")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanAddChildren { get; } = canAddChildren;

    [JsonPropertyName("canDelete")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanDelete { get; } = canDelete;

    [JsonPropertyName("canDownload")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanDownload { get; } = canDownload;

    [JsonPropertyName("canListChildren")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanListChildren { get; } = canListChildren;

    [JsonPropertyName("canRemoveChildren")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanRemoveChildren { get; } = canRemoveChildren;

    [JsonPropertyName("canRename")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanRename { get; } = canRename;

    [JsonPropertyName("canTrash")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanTrash { get; } = canTrash;

    [JsonPropertyName("canReadRevisions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanReadRevisions { get; } = canReadRevisions;

    [JsonPropertyName("canReadTeamDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanReadTeamDrive { get; } = canReadTeamDrive;

    [JsonPropertyName("canMoveTeamDriveItem")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveTeamDriveItem { get; } = canMoveTeamDriveItem;

    [JsonPropertyName("canChangeCopyRequiresWriterPermission")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanChangeCopyRequiresWriterPermission { get; } = canChangeCopyRequiresWriterPermission;

    [JsonPropertyName("canMoveItemIntoTeamDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveItemIntoTeamDrive { get; } = canMoveItemIntoTeamDrive;

    [JsonPropertyName("canUntrash")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanUntrash { get; } = canUntrash;

    [JsonPropertyName("canModifyContent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanModifyContent { get; } = canModifyContent;

    [JsonPropertyName("canMoveItemWithinTeamDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveItemWithinTeamDrive { get; } = canMoveItemWithinTeamDrive;

    [JsonPropertyName("canMoveItemOutOfTeamDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveItemOutOfTeamDrive { get; } = canMoveItemOutOfTeamDrive;

    [JsonPropertyName("canDeleteChildren")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanDeleteChildren { get; } = canDeleteChildren;

    [JsonPropertyName("canMoveChildrenOutOfTeamDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveChildrenOutOfTeamDrive { get; } = canMoveChildrenOutOfTeamDrive;

    [JsonPropertyName("canMoveChildrenWithinTeamDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveChildrenWithinTeamDrive { get; } = canMoveChildrenWithinTeamDrive;

    [JsonPropertyName("canTrashChildren")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanTrashChildren { get; } = canTrashChildren;

    [JsonPropertyName("canMoveItemOutOfDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveItemOutOfDrive { get; } = canMoveItemOutOfDrive;

    [JsonPropertyName("canAddMyDriveParent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanAddMyDriveParent { get; } = canAddMyDriveParent;

    [JsonPropertyName("canRemoveMyDriveParent")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanRemoveMyDriveParent { get; } = canRemoveMyDriveParent;

    [JsonPropertyName("canMoveItemWithinDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveItemWithinDrive { get; } = canMoveItemWithinDrive;

    [JsonPropertyName("canShare")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanShare { get; } = canShare;

    [JsonPropertyName("canMoveChildrenWithinDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanMoveChildrenWithinDrive { get; } = canMoveChildrenWithinDrive;

    [JsonPropertyName("canModifyContentRestriction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanModifyContentRestriction { get; } = canModifyContentRestriction;

    [JsonPropertyName("canAddFolderFromAnotherDrive")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanAddFolderFromAnotherDrive { get; } = canAddFolderFromAnotherDrive;

    [JsonPropertyName("canChangeSecurityUpdateEnabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanChangeSecurityUpdateEnabled { get; } = canChangeSecurityUpdateEnabled;

    [JsonPropertyName("canAcceptOwnership")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanAcceptOwnership { get; } = canAcceptOwnership;

    [JsonPropertyName("canReadLabels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanReadLabels { get; } = canReadLabels;

    [JsonPropertyName("canModifyLabels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanModifyLabels { get; } = canModifyLabels;

    [JsonPropertyName("canModifyEditorContentRestriction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanModifyEditorContentRestriction { get; } = canModifyEditorContentRestriction;

    [JsonPropertyName("canModifyOwnerContentRestriction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanModifyOwnerContentRestriction { get; } = canModifyOwnerContentRestriction;

    [JsonPropertyName("canRemoveContentRestriction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanRemoveContentRestriction { get; } = canRemoveContentRestriction;

    [JsonPropertyName("canDisableInheritedPermissions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanDisableInheritedPermissions { get; } = canDisableInheritedPermissions;

    [JsonPropertyName("canEnableInheritedPermissions")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanEnableInheritedPermissions { get; } = canEnableInheritedPermissions;

    [JsonPropertyName("canChangeItemDownloadRestriction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanChangeItemDownloadRestriction { get; } = canChangeItemDownloadRestriction;

    [JsonPropertyName("canStartApproval")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool? CanStartApproval { get; } = canStartApproval;
}