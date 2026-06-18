using NCoreUtils.Proto;
using HttpMethod = NCoreUtils.Proto.HttpMethod;

namespace NCoreUtils.Google.Drive.Proto;

[ProtoInfo(typeof(IDriveApiV3), Path = "")]
[ProtoMethodInfo(nameof(IDriveApiV3.ListMinimalFilesAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Get, Path = "drive/v3/files")]
[ProtoMethodInfo(nameof(IDriveApiV3.CreateMetadataFileAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "drive/v3/files")]
[ProtoMethodInfo(nameof(IDriveApiV3.SimpleUploadFileAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "upload/drive/v3/files")]
[ProtoMethodInfo(nameof(IDriveApiV3.MultipartUploadFileAsync), Input = InputType.Custom, HttpMethod = HttpMethod.Post, Path = "upload/drive/v3/files")]
[ProtoMethodInfo(nameof(IDriveApiV3.InitializeResumableUploadAsync), Input = InputType.Custom, Output = OutputType.Custom, HttpMethod = HttpMethod.Post, Path = "upload/drive/v3/files")]
public sealed partial class DriveApiV3Info { }