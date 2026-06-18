using NCoreUtils.Proto;
using HttpMethod = NCoreUtils.Proto.HttpMethod;

namespace NCoreUtils.Google.Gmail.Proto;

[ProtoInfo(typeof(IGmailApiV1), Path = "")]
[ProtoMethodInfo(nameof(IGmailApiV1.DeleteMessageAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Delete, Path = "gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.GetMessageAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Get, Path = "gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.ImportMessageAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "upload/gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.InsertMessageAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "upload/gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.ListMessagesAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Get, Path = "gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.ModifyMessageAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.SendMessageAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "upload/gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.TrashMessageAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.UntrashMessageAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.GetMessageAttachmentAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Get, Path = "gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.ListHistoryAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Get, Path = "gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.CreateLabelAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "gmail/v1/users")]
[ProtoMethodInfo(nameof(IGmailApiV1.ListLabelsAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Get, Path = "gmail/v1/users")]
public sealed partial class GmailApiV1Info { }