using NCoreUtils.Proto;

namespace NCoreUtils.Google.Proto;

[ProtoInfo(typeof(IGoogleStorageAdminApiV1), Path = "storage/v1")]
[ProtoMethodInfo(nameof(IGoogleStorageAdminApiV1.GetBucketAsync), Path = "b", Input = InputType.Custom)]
[ProtoMethodInfo(nameof(IGoogleStorageAdminApiV1.InsertBucketAsync), Path = "b", Input = InputType.Custom)]
[ProtoMethodInfo(nameof(IGoogleStorageAdminApiV1.DeleteBucketAsync), Path = "b", Input = InputType.Custom)]
public partial class GoogleStorageAdminApiV1Info { }